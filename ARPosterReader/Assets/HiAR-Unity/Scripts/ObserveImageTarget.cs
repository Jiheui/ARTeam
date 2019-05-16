using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObserveImageTarget : MonoBehaviour {

	public GameObject imageTargeter;
	private ImageTargetBehaviour targetBehaviour;
	private Button button;
    private Image buttonImage;

	void Start () {
		targetBehaviour = imageTargeter.GetComponent<ImageTargetBehaviour>();
        buttonImage = this.transform.GetChild(0).GetComponent<Image>();// GetComponent<Button>();
	}

    public void UpdateTargetBehaviour()
    {
        /*if (imageTargeter == null)
        {
            this.gameObject.SetActive(false);
            return;
        }*/
        targetBehaviour = imageTargeter.GetComponent<ImageTargetBehaviour>();
        this.gameObject.GetComponent<Button>().interactable = targetBehaviour.IsTargetFound();
        if (targetBehaviour.IsTargetFound())
        {
            buttonImage.color = new Color32(255, 255, 225, 255);
        }
        else
        {
            buttonImage.color = new Color32(255, 255, 225, 0);
        }
        //this.gameObject.SetActive(targetBehaviour.IsTargetFound());
    }

    void Update () {
		//button.interactable = targetBehaviour.IsTargetFound();
        //button.gameObject.SetActive(targetBehaviour.IsTargetFound());
    }
}
