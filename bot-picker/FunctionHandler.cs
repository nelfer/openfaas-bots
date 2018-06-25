﻿using System;
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
		public string username { get; set; }="Picky Picker";
	}
	public class FunctionHandler
	{
		public void Handle(string input)
		{
			MattermostMessage ret=new MattermostMessage();
			input=input.Replace("\n","").Replace("\r","");
			Dictionary<string,string> Form=new Dictionary<string, string>();
			input.Split('&').ToList().ForEach(li=>Form.Add(li.Split('=')[0],WebUtility.UrlDecode(li.Split('=')[1])));
			
			Random rnd=new Random((int)DateTime.Now.Ticks);
			string list;
			list = Form["text"]??"nothing";
			List<string> vals=new List<string>(list.Split(','));

			int val0=rnd.Next(0,vals.Count);
			ret.text=String.Format("From the options: {0}",list);
			ret.text+=String.Format("\nI pick for you: **{0}**",vals[val0].Trim());

			Console.WriteLine(JsonConvert.SerializeObject(ret));
		}
	}
}
