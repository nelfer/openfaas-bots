using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Function
{
	public class MattermostMessage
	{
		public string response_type { get; set; }="in_channel";
		public string text { get; set; }
		public string username { get; set; }="Ben Dover";
	}
	public class FunctionHandler
	{
		public void Handle(string input)
		{
			Request.SetContext(input);
			
			Dictionary<string,List<string>> responses=new Dictionary<string,List<string>>();
			responses.Add("greets",new List<string>{"Hello {0}! Welcome to {1}","Hey {0}! What brings you to {1}?","This is {1}. We've been expecting you {0}","Sup {0}?","Mi casa ({1}) es su casa {0}","The people of {1} greet you {0}","Welcome to Walmart... \\*ahem\\* I mean {1}.","Top of the morning!","Good day to you sir!..or madam? I really don't know what you are {0}","Hola {0}","Salutations!","Aloha!"});
			responses.Add("laughs",new List<string>	{"hahaha!","LOL that's funny","jajajajaja siii","ROFL","haha good one","That's really funny {0}"});
			responses.Add("questions",new List<string>	{"What?","How? I don't know...","Why?","When?","That is the question {0}!","IDK","Hmmm","Wait! What?"});
			responses.Add("thanks",new List<string>	{"You're welcome","No problem","De nada","Don't mention it","No, thank you!","You thank me, I thank you. It's a thank you loop","You got it!"});
			responses.Add("rude",new List<string>	{"You shut up!","Me?","What? what? what?","Whatever","Sureeeee","I don't remember anyone asking your opinion","I have better things to do than listening to you","Talk to the hand!","Bless your heart!","If I had eyes, I will be rolling them right now","No wonder everyone talks behind your back","Woah! Do you hear that? That's the sound of me not caring","Surprise me. Say something intelligent.","I'm busy. Go away.","Sorry, I don't speak whatever language that is.","Not too many people like you, do they?","K.","Awww! Are you having a bad day?"});
			responses.Add("bye",new List<string>	{"Byeee!","Bye bye!","Toodles!","See you later alligator!","Hasta la vista!","You'll be back","Goodbye!","Have a good day {0}","Adios!","Ciao!","Au revoir!","Ta-ta!","Take it easy","Aloha!","Farewell","Later!","Until next time","Goodnight"});
			string type=(Request.QueryString["type"]??"greets").ToLower();
			if(!responses.ContainsKey(type))
				type="greets";
			Random rnd=new Random((int)DateTime.Now.Ticks);
			int pos=rnd.Next(0,responses[type].Count);
			MattermostMessage ret=new MattermostMessage{text=String.Format(responses[type][pos],Request.Form["user_name"],Request.Form["channel_name"])};
			Console.WriteLine(JsonConvert.SerializeObject(ret));
		}
	}
}
