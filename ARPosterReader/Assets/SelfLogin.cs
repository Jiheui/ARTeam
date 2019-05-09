using UnityEngine;
using UnityEngine.UI;
using Models;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;

public static class storeLoginSessionId{
    public static int loginId = -1;
    public static string name = null;
    public static string email = "";
}

public class SelfLogin : MonoBehaviour {


	public InputField _email;
	public InputField _password;


	public void checkPassword()
	{
		User u = new User ();
		u.username = _email.text;
		u.password = _password.text;
		u.Login();
        if (u.authenticated)
        {
			string path = "Assets/HiAR-Unity/Resources/User_Info/User.txt";

			StreamWriter writer = new StreamWriter(path,true);
			string uID = u.id.ToString();
			writer.Write (uID);
			writer.Close();

			//Re-import the file to update the reference in the editor
			AssetDatabase.ImportAsset(path);
			TextAsset asset = (TextAsset)Resources.Load(path);
            storeLoginSessionId.loginId = u.id;
            if (string.IsNullOrEmpty(u.name))
            {
                storeLoginSessionId.name = u.username;
            }
            else
            {
                storeLoginSessionId.name = u.name;
            }
            
            storeLoginSessionId.email = u.email;
            SceneManager.LoadScene("HiARRobot");
        }
        else
        {
            Debug.Log("Login Failed");
        }
	}

	public void saveUserInFile(int userID){
		
	}

}
