using System;

namespace Function
{
	public class FunctionHandler
	{
		public void Handle(string input)
		{
			Request.SetContext(input);
			bool isEncrypt=true;
			string decrypt=Request.QueryString["decrypt"]??"0";
			isEncrypt=decrypt!="1";
			Crypto cp=new Crypto();
			if(isEncrypt)
				Console.Write(cp.Encrypt(input));
			else
				Console.Write(cp.Decrypt(input));
		}
	}
}
