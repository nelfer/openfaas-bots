using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Net;

namespace Function
{
	public class MessageFormat
{
	public string username{get;set;}="Magic Ball";
	public string response_type {get;set;}="in_channel";
	public string text{get;set;}="";
}
	public class FunctionHandler
	{
		public void Handle(string input)
		{
			string[] possibilities=
			{
				"It is certain :+1:",
				"It is decidedly so :+1:",
				"Without a doubt :+1:",
				"Yes, definitely :+1:",
				"You may rely on it :+1:",
				"As I see it, yes :+1:",
				"Most likely :+1:",
				"Outlook good :+1:",
				"Yes :+1:",
				"Signs point to yes :+1:",
				"Yeah :+1:",
				"You betcha :+1:",
				"Of course :+1:",
				"That's right :+1:",
				"True :+1:",
				"As certain as death and taxes :+1:",
				"Oh yeah :+1:",
				"Hell yes :+1:",
				"Let me think about it.... Yes :+1:",
				"I don't see why not :+1:",

				"Reply hazy try again :unamused:",
				"Ask again later :unamused:",
				"Better not tell you now :unamused:",
				"Cannot predict now :unamused:",
				"Concentrate and ask again :unamused:",
				"Random prediction not available :unamused:",
				"Cannot compute, try again later :unamused:",
				"I don't know :unamused:",
				"Yes, I mean no, I don't know :unamused:",
				"Can you rephrase that? :unamused:",

				"Don't count on it :-1:",
				"My reply is no :-1:",
				"My sources say no :-1:",
				"Outlook not so good :-1:",
				"Very doubtful :-1:",
				"No :-1:",
				"Nope :-1:",
				"Oh god no :-1:",
				"Seriously? No :-1:",
				"Nah :-1:"
			};
			Dictionary<string,string> Form=new Dictionary<string,string>();
			input.Split('&').ToList().ForEach(li=>Form.Add(li.Split('=')[0],WebUtility.UrlDecode(li.Split('=')[1])));
			Random rnd=new Random();
			string Format="#### Question: {0}\n___\nMagicBall says: {1}";
			MessageFormat msg=new MessageFormat();
			
			string question=Form["text"];
			if(String.IsNullOrEmpty(question))
				question="What is this?";

			string answer=possibilities[rnd.Next(possibilities.Length)];
			
			msg.text=String.Format(Format,question,answer);
			msg.username=Form["user_name"];

			Console.WriteLine(JsonConvert.SerializeObject(msg));
		}
	}
}
