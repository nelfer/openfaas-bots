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
			string jiraURL=File.ReadAllText("/var/openfaas/secrets/JIRA-URL");
			string jiraUser=File.ReadAllText("/var/openfaas/secrets/JIRA-USER");
			string jiraPassword=File.ReadAllText("/var/openfaas/secrets/JIRA-PASSWORD");
			WebClient wc=new WebClient();
			Comment cm=JsonConvert.DeserializeObject<Comment>(input);

			string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(jiraUser + ":" + jiraPassword));

			wc.Headers.Add("Authorization", "Basic " + svcCredentials);
			wc.Headers.Add("Content-Type","application/json");
			wc.UploadString(String.Format(URLTemplate,jiraURL,cm.issue),JsonConvert.SerializeObject(cm));

			Console.WriteLine("Your comment posted. I think.");
		}
	}
}
