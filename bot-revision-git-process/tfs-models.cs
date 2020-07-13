using System;
using System.Collections.Generic;
public class TfsAuthor
{
	public string name { get; set; }
	public string email { get; set; }
	public DateTime date { get; set; }
}

public class TfsGitCommit
{
	public string commitId { get; set; }
	public string comment { get; set; }
	public string url {get;set;}
	public TfsAuthor author { get; set; }
}

public class TfsRepository
{
	public string id { get; set; }
	public string name { get; set; }
	public string remoteUrl { get; set; }
	public string defaultBranch { get; set; }
}
public class TfsResource
{
	public List<TfsGitCommit> commits { get; set; }
	public TfsRepository repository { get; set; }
}

public class TsfDetailedMessage
{
	public string text { get; set; }
	public string html { get; set; }
	public string markdown { get; set; }
}

public class TfsGitPost
{
	public string eventType { get; set; }
	public TfsResource resource { get; set; }
	public TsfDetailedMessage detailedMessage { get; set; }
}

