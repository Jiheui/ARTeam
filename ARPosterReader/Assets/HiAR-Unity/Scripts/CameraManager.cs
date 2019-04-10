using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CameraState{
	HiAR,
	Zoom
}

public class CameraManager : MonoBehaviour {

	public GameObject HiARCamera;
	public GameObject zoomCamera;

	private CameraState cameraState = CameraState.HiAR;

	private AudioListener HiARAudioListener;
	private AudioListener zoomAudioListener;

	void Start () {
		HiARAudioListener = HiARCamera.GetComponent<AudioListener>();
		zoomAudioListener = zoomCamera.GetComponent<AudioListener>();

		SetCameras(cameraState);
	}

	new void SendMessage(string str) {
		cameraState = (cameraState == CameraState.HiAR) ? CameraState.Zoom : CameraState.HiAR;
		SetCameras(cameraState);
	}

	void SetCameras(CameraState state){
		bool isHiAR = state == CameraState.HiAR;

		HiARCamera.SetActive(isHiAR);
		HiARAudioListener.enabled = isHiAR;

		zoomCamera.SetActive(!isHiAR);
		zoomAudioListener.enabled = !isHiAR;
	}
}
