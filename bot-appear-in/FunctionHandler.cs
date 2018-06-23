using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Net;

namespace Function
{
	public class MessageFormat
	{
		public string username{get;set;}="Appear.in";
		public string response_type {get;set;}="in_channel";
		public string text{get;set;}="";
	}
	public class FunctionHandler
	{
		Dictionary<string,string> qs=new Dictionary<string,string>();
		public string QueryString(string key)
		{
			return qs.ContainsKey(key)?qs[key]:null;
		}
		public void Handle(string input)
		{
			
			string qse=Environment.GetEnvironmentVariable("Http_Query")??"";
			qse.Split('&').ToList().ForEach(li=>{string[] parts=li.Split('=');qs.Add(parts[0],parts.Length==2?WebUtility.UrlDecode(li.Split('=')[1]):null);});
			Dictionary<string,string> Form=new Dictionary<string,string>();
			input.Split('&').ToList().ForEach(li=>Form.Add(li.Split('=')[0],WebUtility.UrlDecode(li.Split('=')[1])));
			MessageFormat msg=new MessageFormat();
			Random rnd=new Random();

			int randAnimal=rnd.Next(Constants.Animals.Length);
			int randAdjective=rnd.Next(Constants.Adjectives.Length);
			string ret="Has started a video conference in room: {3}{2}{0}-{1}";
			string prefix=QueryString("prefix");
			if(!String.IsNullOrEmpty(prefix))
				prefix+="-";

			msg.text=String.Format(ret,Constants.Adjectives[randAdjective],Constants.Animals[randAnimal],prefix,"https://appear.in/");
			msg.username=Form["user_name"];

			Console.WriteLine(JsonConvert.SerializeObject(msg));
		}
	}
}
