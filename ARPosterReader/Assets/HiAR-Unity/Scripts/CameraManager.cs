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

	public GameObject zoomObject;
	private Camera zoomCamera;
	private AudioListener zoomAudioListener;

	private CameraState cameraState = CameraState.HiAR;

	public float zoomSpeed = 0.9f;

	void Start () {
		HiAREngineBehaviour = HiARCameraManager.GetComponent<HiAREngineBehaviour>();
		HiARMainCamera = HiARCameraManager.GetComponentInChildren<Camera>();
		HiARAudioListener = HiARCameraManager.GetComponentInChildren<AudioListener>();

		zoomCamera = zoomObject.GetComponent<Camera>();
		zoomAudioListener = zoomObject.GetComponent<AudioListener>();
	}

	void Update () {
		if (cameraState == CameraState.Zoom) {
			if (Input.mouseScrollDelta.y > 0) {
				zoomCamera.orthographicSize -= zoomSpeed;
			}
			else if (Input.mouseScrollDelta.y < 0) {
				zoomCamera.orthographicSize += zoomSpeed;
			}

			if (zoomCamera.orthographicSize < 0) {
				zoomCamera.orthographicSize = 0.1f;
			}
			else if (zoomCamera.orthographicSize > 20) {
				zoomCamera.orthographicSize = 20;
			}
		}
	}

	new void SendMessage(string str) {
		if (HiARBackgroundCamera == null) {
			HiARBackgroundCamera = GameObject.Find ("camera background").GetComponent<Camera> ();
		}
		cameraState = (cameraState == CameraState.HiAR) ? CameraState.Zoom : CameraState.HiAR;
		SetCameras(cameraState);
		Debug.Log ("Test " + str);
	}

	void SetCameras(CameraState state){
		bool isHiAR = state == CameraState.HiAR;

		HiAREngineBehaviour.enabled = isHiAR;
		HiARMainCamera.enabled = isHiAR;
		HiARBackgroundCamera.enabled = isHiAR;
		HiARAudioListener.enabled = isHiAR;

		zoomCamera.enabled = !isHiAR;
		zoomAudioListener.enabled = !isHiAR;
	}
}
