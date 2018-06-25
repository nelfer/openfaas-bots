using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Function
{
	public class MattermostMessage
	{
		public string response_type { get; set; }="in_channel";
		public string text { get; set; }
		public string username { get; set; }="Walmart Greeter";
	}
	public class FunctionHandler
	{
		public void Handle(string input)
		{
			List<string> greets=new List<string>
			{
				"Hello {0}! Welcome to {1}",
				"Hey {0}! What brings you to {1}?",
				"This is {1}. We've been expecting you {0}",
				"Sup {0}?",
				"Mi casa ({1}) es su casa {0}",
				"The people of {1} greet you {0}",
				"Welcome to Walmart... **ahem** I mean {1}."
			};
			Random rnd=new Random((int)DateTime.Now.Ticks);
			int pos=rnd.Next(0,greets.Count);
			input=input.Replace("\n","").Replace("\r","");
			Dictionary<string,string> Form=new Dictionary<string, string>();
			input.Split('&').ToList().ForEach(li=>Form.Add(li.Split('=')[0],WebUtility.UrlDecode(li.Split('=')[1])));
			MattermostMessage ret=new MattermostMessage{text=String.Format(greets[pos],Form["user_name"],Form["channel_name"])};
			Console.WriteLine(JsonConvert.SerializeObject(ret));
		}
	}
}
