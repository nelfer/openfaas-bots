using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace Function
{
	public class CommentIn
	{
		public string jira { get; set; }
		public string issue { get; set; }
		public string body { get; set; }
		public string revision { get; set; }
	}

	public class CommentOut
	{
		public string issue { get; set; }
		public string body { get; set; }
	}
	public class RevField
	{
		public string customfield_11700 { get; set; }
	}

	public class BodyUpdate
	{
		public RevField fields { get; set; }
	}
	public class FunctionHandler
	{
		public void Handle(string input)
		{
			string URLTemplate="{0}/rest/api/2/issue/{1}/comment";
			string UpdateURLTemplate="{0}/rest/api/2/issue/{1}";

			string jiraURL;
			string jiraUser;
			string jiraPassword;
			CommentIn cm=new CommentIn();
			try
			{
				cm=JsonConvert.DeserializeObject<CommentIn>(input);
				if(cm==null)throw new SystemException("Null parameter");
			}
			catch (Exception ex)
			{
				throw new SystemException("Error. Parsign parameter: "+ex.Message);
			}

			try
			{
				if(String.IsNullOrEmpty(cm.jira))
				{
					jiraURL=File.ReadAllText("/var/openfaas/secrets/JIRA-URL");
					jiraUser=File.ReadAllText("/var/openfaas/secrets/JIRA-USER");
					jiraPassword=File.ReadAllText("/var/openfaas/secrets/JIRA-PASSWORD");
				}
				else
				{
					jiraURL=File.ReadAllText(String.Format("/var/openfaas/secrets/{0}-JIRA-URL",cm.jira));
					jiraUser=File.ReadAllText(String.Format("/var/openfaas/secrets/{0}-JIRA-USER",cm.jira));
					jiraPassword=File.ReadAllText(String.Format("/var/openfaas/secrets/{0}-JIRA-PASSWORD",cm.jira));
				}
			}
			catch (Exception ex)
			{
				throw new SystemException("Error. You might not have the secrets: "+ex.Message);
			}
			WebClient wc=new WebClient();
			
			string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(jiraUser + ":" + jiraPassword));
			wc.Encoding=Encoding.UTF8;
			wc.Headers.Add("Authorization", "Basic " + svcCredentials);
			wc.Headers.Add("Content-Type","application/json");
			try
			{
				CommentOut comment=new CommentOut{issue=cm.issue,body=cm.body};
				wc.UploadString(String.Format(URLTemplate,jiraURL,cm.issue),JsonConvert.SerializeObject(comment));
			}
			catch (System.Exception ex)
			{
				System.Console.WriteLine("Error sending request: {0}\n\n{1}",ex.Message,ex.StackTrace);
			}
			
			if(!String.IsNullOrEmpty(cm.revision))
			{
				WebClient wc2=new WebClient();
				wc2.Encoding=Encoding.UTF8;
				wc2.Headers.Add("Authorization", "Basic " + svcCredentials);
				wc2.Headers.Add("Content-Type","application/json");
				BodyUpdate data=new BodyUpdate();
				data.fields=new RevField{customfield_11700=cm.revision};
				string result="None";
				string urlUpdate=String.Format(UpdateURLTemplate,jiraURL,cm.issue);
				string JsonData=JsonConvert.SerializeObject(data);
				try
				{
					result=wc2.UploadString(urlUpdate,"PUT",JsonData);
				}
				catch (WebException ex)
				{
					if (ex.Response != null)
					{ 
						string response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
						Console.WriteLine("Response: {0}",response);
					}
				}
				catch (System.Exception ex)
				{
					System.Console.WriteLine("Error sending revision to Jira [{4}]: {0}\n\n{1}\n\n[{2}]\n\nResult: {3}",
						ex.Message,ex.StackTrace,JsonData,result,urlUpdate);
				}
			}
			Console.WriteLine("Your comment posted. I think.");
		}
	}
}
