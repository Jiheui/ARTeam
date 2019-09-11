using Models;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HideButton : MonoBehaviour {
    
    public Button button = null;

    public Image image = null;

    bool recognized = false;

    public void Start()
    {
        // change the favourite button status default set to disable
        button.gameObject.SetActive(false);
    }

    // Update the text on the Button
    public void changeButtonStatus(bool b)
	{
        if(b) {
            button.gameObject.SetActive(true);
            button.interactable = true;
            image.color = new Color(255, 255, 255, 255);
        }

        if(b == false) {
            button.gameObject.SetActive(false);
            button.interactable = false;
            image.color = new Color(255, 255, 255, 0);
        }
	}
}
