using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObserveImageTarget : MonoBehaviour {

	public GameObject imageTargeter;
	private ImageTargetBehaviour targetBehaviour;
	private Button button;

	void Start () {
		targetBehaviour = imageTargeter.GetComponent<ImageTargetBehaviour>();
		//button = GetComponent<Button>();
	}

    public void UpdateTargetBehaviour()
    {
        /*if (imageTargeter == null)
        {
            this.gameObject.SetActive(false);
            return;
        }*/
        targetBehaviour = imageTargeter.GetComponent<ImageTargetBehaviour>();
        this.gameObject.SetActive(targetBehaviour.IsTargetFound());
    }

    void Update () {
		//button.interactable = targetBehaviour.IsTargetFound();
        //button.gameObject.SetActive(targetBehaviour.IsTargetFound());
    }
}
