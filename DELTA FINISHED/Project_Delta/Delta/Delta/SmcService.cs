using System;
using System.Net;
using System.IO;

namespace Delta
{
	/// <summary>
	/// Description of SmcService.
	/// </summary>
	public class SmcService
	{
		
		public void Set_Variable(String variable, String var_value, String smc_control, String var_type){
			
			Request_Variable(variable,smc_control);
			
			String setreq = "<?xml version=\"1.0\" encoding=\"utf-8\"?><SOAP-ENV:Envelope " +
							"xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
							"xmlns:SOAP-ENC=\"http://schemas.xmlsoap.org/soap/encoding/\" " +
							"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
							"xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
							"xmlns:ns=\"urn:rtWatchIo\">" +
							"<SOAP-ENV:Body><ns:setVariables><watchIoName>"+smc_control+"</watchIoName>" +
							"<watchIoName>"+smc_control+"</watchIoName>" +
							"<variables xsi:type=\"ns:Variables\">" +
							"<item xsi:type=\"ns:"+var_type+"\">" +
							"<name>"+variable+"</name>" +
							"<value>"+var_value+"</value>" +
							"<description></description></item></variables></ns:setVariables></SOAP-ENV:Body></SOAP-ENV:Envelope>";
			
			Request_Soap(setreq);
			Deselect_Variable(variable,smc_control);
		}
		
		public String Request_Variable(String variable,String smc_control){
				
			String soapreq = "<?xml version=\"1.0\" encoding=\"utf-8\"?><SOAP-ENV:Envelope " +
									"xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
									"xmlns:SOAP-ENC=\"http://schemas.xmlsoap.org/soap/encoding/\" " +
									"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
									"xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
									"xmlns:ns=\"urn:rtWatchIo\">" +
									"<SOAP-ENV:Body><ns:registerVariables><watchIoName>"+smc_control+"</watchIoName>" +
									"<variables xsi:type=\"ns:Variables\"><item xsi:type=\"ns:Variable\">" +
									"<name>"+variable+"</name><description></description></item></variables>" +
									"</ns:registerVariables></SOAP-ENV:Body></SOAP-ENV:Envelope>";
				
			String result = Request_Soap(soapreq);
						
			String xml_value = "";
			
			try{
				xml_value = result.Substring(result.IndexOf("<value>")+7, 
			                                (result.IndexOf("</value>") - result.IndexOf("<value>"))-7);
			}catch(Exception){
				xml_value = "Cannot obtain the value."; // Could occur if the service is on but the SmCServer is off.
			}
				
			return xml_value;
		}
		
		public void Deselect_Variable(String variable,String smc_control){
			
			String soapreq = "<?xml version=\"1.0\" encoding=\"utf-8\"?><SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
									"xmlns:SOAP-ENC=\"http://schemas.xmlsoap.org/soap/encoding/\" " +
									"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
									"xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
									"xmlns:ns=\"urn:rtWatchIo\">" +
								 	"<SOAP-ENV:Body><ns:unregisterVariables><watchIoName>"+smc_control+"</watchIoName>" +
									"<variables xsi:type=\"ns:Variables\"><item xsi:type=\"ns:Variable\">" +
									"<name>"+variable+"</name><description></description></item></variables>" +
									"</ns:unregisterVariables></SOAP-ENV:Body></SOAP-ENV:Envelope>";
			
			Request_Soap(soapreq);		
		}		
		
		private String Request_Soap(String soap){
				
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:2202");
			request.Method = "POST";
			request.ContentType = "text/xml";
				
			String result = "";
			
			using (var streamWriter = new StreamWriter(request.GetRequestStream())){
				streamWriter.Write(soap);
				streamWriter.Flush();
			}
				
			var response = (HttpWebResponse) request.GetResponse();
				
			using (var streamReader = new StreamReader(response.GetResponseStream())){
				result = streamReader.ReadToEnd();
			}
				
			return result;
		}	
	}
}
