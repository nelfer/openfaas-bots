using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;

static public class Request
{
	static public StringDictionary QueryString=new StringDictionary();
	static public StringDictionary Form=new StringDictionary();

	static public void SetContext(string input)
	{
		string qse=Environment.GetEnvironmentVariable("Http_Query")??"";
		qse.Split('&').ToList().ForEach(li=>{string[] parts=li.Split('=');QueryString.Add(parts[0],parts.Length==2?WebUtility.UrlDecode(li.Split('=')[1]):null);});
		input.Split('&').ToList().ForEach(li=>{string[] parts=li.Split('=');Form.Add(parts[0],parts.Length==2?WebUtility.UrlDecode(li.Split('=')[1]):null);});
	}
}