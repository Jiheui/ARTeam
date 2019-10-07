using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ObserveImageTarget : MonoBehaviour {

	public GameObject imageTargeter;
	private CustomImageTargetBehaviour targetBehaviour;
	private Button button;

	void Start () {
	}

    public void UpdateTargetBehaviour()
    {
        if (imageTargeter == null)
        {
            //buttonImage.color = new Color32(255, 255, 225, 0);
            return;
        }
        targetBehaviour = imageTargeter.GetComponent<CustomImageTargetBehaviour>();
        this.gameObject.GetComponent<Button>().interactable = targetBehaviour.IsTargetFound();
    }
}
