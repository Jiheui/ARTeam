using System.Reflection;

public class Tools
{
	public static string serverIP {get { return "shmily.me";}}
	public static string serverPort {get { return "8080";}}

	public string Server { get { return serverIP + ":" + serverPort;}}

	//public string MakeJsonStringFromClass<T>(T t) {
	//	if(t == null) return "{}";
	//
	//	FieldInfo[] properties =t.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
	//	var json = "{";
	//	for(int i = 0; i < properties.Length; i++)
	//	{
	//		json += i == 0 ? "" : ",";
	//		json += "\"" + properties [i].Name + "\":";
	//		if(properties[i] is string){
	//			json += "\"" + properties [i].GetValue (t) + "\"";
	//		} else {
	//			json += properties [i].GetValue (t);
	//		}
	//	}
	//	return json + "}";
	//}
}