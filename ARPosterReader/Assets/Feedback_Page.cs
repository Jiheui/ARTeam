using UnityEngine;
using UnityEngine.UI;
using Models;
using System.Collections;

public class Feedback_Page : MonoBehaviour {
	public InputField _feedback;
    public Image toast;
    public Text toastText;
    
	public void AddtoDatabase()
	{
		string email = storeLoginSessionId.email;

		int id = storeLoginSessionId.loginId;
		string username = storeLoginSessionId.name;
		string f = _feedback.text;

        if(id==-1 || string.IsNullOrEmpty(_feedback.text))
        {
            showToast("Feedback sent failed", 2);
            return;
        }
        
		// Debug.Log (f);    
		Feedback feedback = new Feedback();
		feedback.userid = id;
		//feedback.userid = 666;
		feedback.username = storeLoginSessionId.name;
		feedback.email = storeLoginSessionId.email;
		feedback.content = f;
		feedback.SendFeedback();
        _feedback.text = "";
        
        showToast("Feedback sent successful",2);
    }

    void showToast(string text,int duration)
    {
        StartCoroutine(showToastCOR(text, duration));
    }

    private IEnumerator showToastCOR(string text,
        int duration)
    {
        Color orginalColor = toastText.color;

        toastText.text = text;
        toastText.enabled = true;

        //Fade in
        yield return fadeInAndOut( true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut( false, 0.5f);

        toastText.enabled = false;
        toastText.color = orginalColor;
    }

    IEnumerator fadeInAndOut( bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = new Color(0.7f,0.7f,0.7f);
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            toast.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }
}

