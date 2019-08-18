using System.Reflection;

public class Tools
{
	public static string serverIP {get { return "ar.shmily.me";}}
	public static string serverPort {get { return "8080";}}

	public string Server { get { return serverIP + ":" + serverPort;}}
}