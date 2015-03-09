using System;
using System.Collections;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Delta
{

	public class Delta
	{
		// Recognition && Synthesision
		private SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
		private SpeechSynthesizer _synthesizer = new SpeechSynthesizer();
		// My SmcService
		private SmcService _smcService = new SmcService();
		// Grammar Stuff
		private Hashtable variables,numbers,extra;
		private GrammarBuilder standard,numeric;
		private MainForm gui;
		// For Digit Entry
		private bool digit_entry;
		private String set_variable;
		private String set_value;
		// Variables you can turn on/off
		private ArrayList switch_variables;
		// Variables you can set
		private ArrayList set_variables; 
		private TextBox console;
		
		public Delta(MainForm the_gui,TextBox console){
			this.gui = the_gui;
			this.console = console;
			digit_entry = false;
			set_variable= "";
			set_value = "";
			Fill_Variables();
			Setup_Speach();
		}
		// Setup the recognizer.
		public void Setup_Speach(){
			_recognizer.LoadGrammar(new Grammar(standard));
			_recognizer.SetInputToDefaultAudioDevice();
			
			_recognizer.SpeechRecognized+=_Recognized;
			_recognizer.SpeechHypothesized+= _HearingSpeach;
			_recognizer.RecognizeCompleted+=_RecognitionDone;
			_recognizer.SpeechRecognitionRejected+=_NotRecognized;	

			_recognizer.RecognizeAsync(RecognizeMode.Single);	
		}
		// Fill in the variables that we will need for the recognizer && synthesizer.
		public void Fill_Variables(){
			variables = new Hashtable();
			numbers = new Hashtable();
			extra = new Hashtable();
			
			// All the variables in a Hashtable for easy access to the Smc variable name and the Controller && variable type.
			variables.Add("draught",new String[]{"C.Coefs.Wind.Draught","SmcControl1"});
			variables.Add("radius", new String[]{"M.Radius.ObjEnabled","SmcModel","IntVariable"});
			variables.Add("heading sensor", new String[]{"M.Scenario.SensorOffset.Heading1SensorOffsetValue",
			              "SmcModel","FloatVariable"});
			variables.Add("latitude", new String[]{"M.Ship.Position.Lat","SmcModel","DoubleVariable"});
			variables.Add("longtitude", new String[]{"M.Ship.Position.Lon","SmcModel","DoubleVariable"});
			variables.Add("dp", new String[]{"C.Transfer.State.DpOnReqNew","SmcControl1","IntVariable"});
			variables.Add("dp auto surge", new String[]{"C.Transfer.State.DpSurgeAutoReqNew","SmcControl1","IntVariable"});
			variables.Add("dp auto sway", new String[]{"C.Transfer.State.DpSwayAutoReqNew","SmcControl1","IntVariable"});
			variables.Add("dp joystick surge", new String[]{"C.Transfer.State.DpSurgeJoystickReqNew","SmcControl1","IntVariable"});
			variables.Add("dp joystick sway", new String[]{"C.Transfer.State.DpSwayJoystickReqNew","SmcControl1","IntVariable"});
			variables.Add("dp joystick yaw", new String[]{"C.Transfer.State.DpYawJoystickReqNew","SmcControl1","IntVariable"});
			variables.Add("dp auto yaw", new String[]{"C.Transfer.State.DpYawAutoReqNew","SmcControl1","IntVariable"});
			variables.Add("optimal heading", new String[]{"C.Transfer.State.OptimalHeadingReqNew","SmcControl1","IntVariable"});

			// Extra variables
			extra.Add("on","1");
			extra.Add("off","0");
			extra.Add("1","on");
			extra.Add("0","off");
			extra.Add("on2","enabling");
			extra.Add("off2","disabling");
			extra.Add("on3","enabled");
			extra.Add("off3","disabled");
			// For the digit entry translation.
			numbers.Add("zero","0");
			numbers.Add("one","1");
			numbers.Add("two","2");
			numbers.Add("three","3");
			numbers.Add("four","4");
			numbers.Add("five","5");
			numbers.Add("six","6");
			numbers.Add("seven","7");
			numbers.Add("eight","8");
			numbers.Add("nine","9");
			numbers.Add("space"," ");
			numbers.Add("dot",".");
			// For seperation to determin which variable belongs to which type command (switch || set), they can all be read.
			set_variables 	 = new ArrayList();
			switch_variables = new ArrayList();
			
			switch_variables.Add("radius");
			switch_variables.Add("dp");
			switch_variables.Add("dp auto surge");
			switch_variables.Add("dp auto sway");
			switch_variables.Add("dp joystick surge");
			switch_variables.Add("dp joystick sway");
			switch_variables.Add("dp joystick yaw");
			switch_variables.Add("dp auto yaw");
			switch_variables.Add("optimal heading");
			
			set_variables.Add("heading sensor");
			set_variables.Add("latitude");
			set_variables.Add("longtitude");
			
			numeric = Create_NumericGB();
			standard= Create_Commands();
		}
		// If recognizer can recognize the command, determin the type of command.
		public void _Recognized(object sender, SpeechRecognizedEventArgs e){
			
			GUI_Background(System.Drawing.Color.DarkGreen);
			
			try{
				if(!digit_entry){
					if(e.Result.Text.Contains("what is the")){	
						_synthesizer.Speak(Read_Command(e));
					}else if(e.Result.Text.Contains("turn")){				
						_synthesizer.Speak(Turn_Command(e));
					}else if(e.Result.Text.Contains("set")){
						Set_Command(e);
					}else if(e.Result.Words[1].Text.Equals("exit") && e.Result.Words[2].Text.Equals("application")){
						Close_App();
					}	
				}else{
					Translate_Digits(e);
					GUI_Print(set_value);
				}
				
			}catch(System.Net.WebException){
				_synthesizer.Speak("Error, service is not available.");
			}catch(Exception){
				
			}
		}
		// If the recognizer does not recognize the command.
		public void _NotRecognized(object sender, SpeechRecognitionRejectedEventArgs e)
		{
			GUI_Background(System.Drawing.Color.DarkRed);
			_synthesizer.Speak("Unknown command");		  	
		}
		// While it hears speach...
		public void _HearingSpeach(object sender, SpeechHypothesizedEventArgs e){
			GUI_Background(System.Drawing.Color.Yellow);
		}
		// When finishing the recognition, recognize again. RecognizeMode.Single has been proven to be more accurate than 
		// RecognizeMode.Multi.
		public void _RecognitionDone(object sender, RecognizeCompletedEventArgs e){
			GUI_Background(System.Drawing.Color.Black);
			
			_recognizer.RecognizeAsync(RecognizeMode.Single);
		}
		
		public String Read_Command(SpeechRecognizedEventArgs e){
			String target = Extract_Variable(6,4,e);
			String[] var = (String[]) variables[target];
			
			String result = _smcService.Request_Variable(var[0],var[1]);
			String state = "off";
				
			_smcService.Deselect_Variable(var[0],var[1]); // Used to unregister a variable after using it.
			
			String response = "";
			
			if(result.Equals("1"))state="on";
			// Handle response.
			if(!result.Equals("Cannot obtain the value.")){
				if(switch_variables.Contains(target.Trim())){
					response = "The " + target + " is turned " + state;  
				}else{
					response = "The " + target.ToString() + " is " + result;
				}		
			}else{
				response = "Error, failed to retrieve the " + target + " variable.";
			}	
			
			return response;
		}
		// Switch commands, commands that an be turned on/of. Also checks if something is already turned on or off before acting.
		public String Turn_Command(SpeechRecognizedEventArgs e){
			
			String target = Extract_Variable(4,2,e);
			String[] var = (String[]) variables[target];
			
			String current_value = _smcService.Request_Variable(var[0],var[1]);
			String result = "";
			String onoff  = e.Result.Words[e.Result.Words.Count-1].Text;
			
			// If the var is not already turned on/off.
			if(!extra[onoff].Equals(current_value)){
				if(switch_variables.Contains(target.Trim())){
					_smcService.Set_Variable(var[0],extra[onoff].ToString(),var[1],var[2]);
					result = extra[onoff + "2"].ToString() + " " + target + ".";
				}else{
					result = "You cannot " + extra[onoff + "2"].ToString() + target + ".";
				}
			}else{
				result = "The variable " + target + " is already " + extra[onoff + "3"].ToString();
			}
			
			return result;
		}
		
		// Triggers the digit entry mode.
		public void Set_Command(SpeechRecognizedEventArgs e){
			
			String target = Extract_Variable(4,2,e);
			String[] var = (String[]) variables[target];
				
			if(set_variables.Contains(target.Trim())){
				digit_entry = true;
				set_variable= target;
				
				_recognizer.UnloadAllGrammars(); 				// Unload all grammars so it cannot compare to any commands.
				_recognizer.LoadGrammar(new Grammar(numeric));  // Load the numeric grammar so it can only compare to digit_entry variables.
				
				_synthesizer.Speak("Which value?");
			}else{
				_synthesizer.Speak("The variable " + target + " cannot be set to a value.");
			}	
		}
		// Handles the digit_entry variable commands && translates the digit variables to the values of the numbers Hashtable.
		public void Translate_Digits(SpeechRecognizedEventArgs e){
			try{
				if(e.Result.Words[e.Result.Words.Count-1].Text == "correct"){
				
					if(set_value.Length>0){
						set_value = set_value.Remove(set_value.Length-1);
					}
					
				}else if(e.Result.Words[e.Result.Words.Count-1].Text == "clear"){
					set_value = "";
				}else if(e.Result.Words[e.Result.Words.Count-1].Text == "stop"){
					
					Enter_Digit_Entry(set_variable.Trim(),set_value.Trim());
					
				}else{
					set_value += e.Result.Text;
				
					foreach(DictionaryEntry pair in numbers){
						set_value = set_value.Replace(pair.Key.ToString(),pair.Value.ToString());
					}	
				}
			}catch(Exception){
				
			}
		}
		// Does a Set_Variable command with the given value. 
		public void Enter_Digit_Entry(String variable,String value){
			_recognizer.RecognizeAsyncStop();
			_recognizer.LoadGrammar(new Grammar(standard));
			digit_entry = false;
			set_value = "";
			GUI_Print("");
			
			
			if(value.Length>0){
				
				String[] target = (String[]) variables[variable];
				
				if(value.Contains(" ")){
					_synthesizer.Speak("Error, " + variable + " needs 1 entry.");
				}else{
					_smcService.Set_Variable(target[0],value,target[1],target[2]);
					_synthesizer.Speak("Setting " + variable + " to " + value);
				}
			}else{
				_synthesizer.Speak("Setting " + variable + " has been canceled");
			}
		}
		// Extract the variable from a command. The variable can consist of multiple words.
		public String Extract_Variable(int length,int start,RecognitionEventArgs e){
			
			String target = "";
			
			if(e.Result.Words.Count > length){
				for(int i = start; i < e.Result.Words.Count-1; i++){
					target += e.Result.Words[i].Text + " ";
				}
			}else{
				target = e.Result.Words[start].Text;
			}
			
			return target.Trim();
		}		
		
		// For GUI SECTION
		public delegate void PrintCallback(String msg);
		
		public void Print(String msg){
			console.Text = msg;
		}

		public void GUI_Print(String msg){
			console.Invoke(new PrintCallback(this.Print),new Object[]{msg}); // For printing on the TextBox screen on the GUI.
		}
				
		public void GUI_Background(System.Drawing.Color color){
			gui.Invoke(new Change_BackgroundCallback(this.Change_Background),new object[]{color}); // Changing GUI background.
		}
			
		public delegate void Change_BackgroundCallback(System.Drawing.Color color);
		public void Change_Background(System.Drawing.Color color)
		{
			gui.BackColor = color;
			console.BackColor = color;
			gui.Update();
		}		
		
		// Grammar Builder Section
		
		//Standard commands
		public GrammarBuilder Create_Commands(){
		
			GrammarBuilder gb = new GrammarBuilder();
			gb.Append("delta");
			gb.Append(new Choices("turn","set","what is the","exit"));
			gb.Append(new Choices("draught","radius","heading sensor","dp","application",
			                      "latitude","longtitude","dp","dp auto surge",
			                      "dp auto sway","dp joystick surge","dp joystick sway","dp joystick yaw",
			                      "dp auto yaw","optimal heading"));
			gb.Append(new Choices("on","off"));
			return gb;
		}
		// Digit_Entry commands.
		public GrammarBuilder Create_NumericGB(){
			GrammarBuilder gb = new GrammarBuilder();
			
			gb.Append(new Choices("zero","one","two","three","four","five","six","seven","eight","nine",
			                      "dot","stop","correct","space","clear","stop"));
			return gb;
		}
		// Closing the application.
		public void Close_App(){
			Environment.Exit(0);			
		}
	}
}
