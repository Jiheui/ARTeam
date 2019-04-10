using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState{
	HiAR,
	Zoom
}

public class CameraManager : MonoBehaviour {

	public GameObject HiARCameraManager;
	private HiAREngineBehaviour HiAREngineBehaviour;
	private Camera HiARMainCamera;
	private Camera HiARBackgroundCamera;
	private AudioListener HiARAudioListener;

	public GameObject zoomCamera;
	private AudioListener zoomAudioListener;

	private CameraState cameraState = CameraState.HiAR;

	void Start () {
		HiAREngineBehaviour = HiARCameraManager.GetComponent<HiAREngineBehaviour>();
		HiARMainCamera = HiARCameraManager.GetComponentInChildren<Camera>();
		HiARAudioListener = HiARCameraManager.GetComponentInChildren<AudioListener>();

		zoomAudioListener = zoomCamera.GetComponent<AudioListener>();
	}

	new void SendMessage(string str) {
		if (HiARBackgroundCamera == null) {
			HiARBackgroundCamera = GameObject.Find ("camera background").GetComponent<Camera> ();
		}
		cameraState = (cameraState == CameraState.HiAR) ? CameraState.Zoom : CameraState.HiAR;
		SetCameras(cameraState);
	}

	void SetCameras(CameraState state){
		bool isHiAR = state == CameraState.HiAR;

		HiAREngineBehaviour.enabled = isHiAR;
		HiARMainCamera.enabled = isHiAR;
		HiARBackgroundCamera.enabled = isHiAR;
		HiARAudioListener.enabled = isHiAR;

		zoomCamera.SetActive(!isHiAR);
		zoomAudioListener.enabled = !isHiAR;
	}
}
