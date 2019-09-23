using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace Function
{
	public class Comment
	{
		public string issue { get; set; }
		public string body { get; set; }
	}
	public class FunctionHandler
	{
		public void Handle(string input)
		{
			string URLTemplate="{0}/rest/api/2/issue/{1}/comment";
			string jiraURL;
			string jiraUser;
			string jiraPassword;
			Comment cm=new Comment();
			try
			{
				cm=JsonConvert.DeserializeObject<Comment>(input);
				if(cm==null)throw new SystemException("Null parameter");
			}
			catch (Exception ex)
			{
				throw new SystemException("Error. Parsign parameter: "+ex.Message);
			}

			try
			{
				jiraURL=File.ReadAllText("/var/openfaas/secrets/JIRA-URL");
				jiraUser=File.ReadAllText("/var/openfaas/secrets/JIRA-USER");
				jiraPassword=File.ReadAllText("/var/openfaas/secrets/JIRA-PASSWORD");
			}
			catch (Exception ex)
			{
				throw new SystemException("Error. You might not have the secrets: "+ex.Message);
			}
			WebClient wc=new WebClient();
			
			string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(jiraUser + ":" + jiraPassword));

			wc.Headers.Add("Authorization", "Basic " + svcCredentials);
			wc.Headers.Add("Content-Type","application/json");
			try
			{
				wc.UploadString(String.Format(URLTemplate,jiraURL,cm.issue),JsonConvert.SerializeObject(cm));
			}
			catch (System.Exception ex)
			{
				System.Console.WriteLine("Error sending request: {0}\n\n{1}",ex.Message,ex.StackTrace);
			}
			
			Console.WriteLine("Your comment posted. I think.");
		}
	}
}
