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
		public string username { get; set; }="Flipper (not the dolphin)";
	}
	public class FunctionHandler
	{
		public void Handle(string input)
		{
			MattermostMessage ret=new MattermostMessage();
			Random rnd=new Random((int)DateTime.Now.Ticks);

			int val0=rnd.Next(0,100);
			int val1=rnd.Next(0,100);
			int val2=rnd.Next(0,100);
			ret.text=String.Format("Flip 1 is: {0}",val0<50?"Tails":"Heads");
			ret.text+=String.Format("\nFlip 2 is: {0}",val1<50?"Tails":"Heads");
			ret.text+=String.Format("\nFlip 3 is: {0}",val2<50?"Tails":"Heads");
			Console.WriteLine(JsonConvert.SerializeObject(ret));
		}
	}
}
