using System;
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
			int totalThrows=5;
			MattermostMessage ret=new MattermostMessage();
			Random rnd=new Random((int)DateTime.Now.Ticks);
			List<string> throws=new List<string>();
			for (int cni = 0; cni < totalThrows; cni++)
			{
				throws.Add(rnd.Next(100)<50?"Tails":"Heads");
			}
			for (int pni = 0; pni < throws.Count; pni++)
			{
				ret.text+=String.Format("Flip {0} is: {1}\n",pni+1,throws[pni]);
			}
			ret.text+=String.Format("\n**{0} wins!**",throws.Count(it=>it=="Tails")>totalThrows/2?"Tails":"Heads");
			Console.WriteLine(JsonConvert.SerializeObject(ret));
		}
	}
}
