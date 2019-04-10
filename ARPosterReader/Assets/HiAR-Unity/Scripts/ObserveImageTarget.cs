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
		button = GetComponent<Button>();
	}

	void Update () {
		button.interactable = targetBehaviour.IsTargetFound();
	}
}
