using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState{
	HiAR,
	Zoom
}

public class CameraManager : MonoBehaviour {

	public GameObject HiARCameraManager;
	//private HiAREngineBehaviour HiAREngineBehaviour;
	private Camera HiARMainCamera;
	private Camera HiARBackgroundCamera;
	private AudioListener HiARAudioListener;

	public GameObject zoomObject;
	private Camera zoomCamera;
	private AudioListener zoomAudioListener;

	private CameraState cameraState = CameraState.HiAR;

	public float zoomSpeed = 0.9f;
    
    private Touch oldTouch1; // store the finger touch point
    private Touch oldTouch2; // store the finger touch point

    public GameObject aimImageTarget = null;
    
    void Start () {
		/*HiAREngineBehaviour = HiARCameraManager.GetComponent<HiAREngineBehaviour>();
		HiARMainCamera = HiARCameraManager.GetComponentInChildren<Camera>();
		HiARAudioListener = HiARCameraManager.GetComponentInChildren<AudioListener>();

		zoomCamera = zoomObject.GetComponent<Camera>();
		zoomAudioListener = zoomObject.GetComponent<AudioListener>();*/
	}

	void Update () {
		if (cameraState == CameraState.Zoom) {
			if (Input.mouseScrollDelta.y > 0) {
				zoomCamera.orthographicSize -= zoomSpeed;
			}
			else if (Input.mouseScrollDelta.y < 0) {
				zoomCamera.orthographicSize += zoomSpeed;
			}

            if (aimImageTarget == null)
            {
                return;
            }
            
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 deltaPos = touch.deltaPosition;

                Vector3 direction = Vector3.right;

                bool isDown = Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y);

                if (isDown)
                {
                    direction = Vector3.down;
                }

                for (var i = 0; i < aimImageTarget.transform.childCount; i++)
                {
                    aimImageTarget.transform.GetChild(i).transform.Rotate(direction * deltaPos.x, Space.World);

                }
            }
            else if (Input.touchCount > 1)
            {
                //scale by touch
                Touch newTouch1 = Input.GetTouch(0);
                Touch newTouch2 = Input.GetTouch(1);

                if (newTouch2.phase == TouchPhase.Began)
                {
                    oldTouch2 = newTouch2;
                    oldTouch1 = newTouch1;
                    return;
                }
                float oldDistance = Vector2.Distance(oldTouch1.position, oldTouch2.position);
                float newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);
                //offset > 0 means to two touch expanding
                float offset = newDistance - oldDistance;

                if (offset > 0)
                {
                    zoomCamera.orthographicSize -= zoomSpeed/10;
                }

                if (offset < 0)
                {
                    zoomCamera.orthographicSize += zoomSpeed/10;
                }

                oldTouch1 = newTouch1;
                oldTouch2 = newTouch2;
            }
            
            if (zoomCamera.orthographicSize < 0) {
				zoomCamera.orthographicSize = 0.1f;
			}
			else if (zoomCamera.orthographicSize > 40) {
				zoomCamera.orthographicSize = 40;
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
		/*bool isHiAR = state == CameraState.HiAR;

		HiAREngineBehaviour.enabled = isHiAR;
		HiARMainCamera.enabled = isHiAR;
		HiARBackgroundCamera.enabled = isHiAR;
		HiARAudioListener.enabled = isHiAR;

        if(aimImageTarget != null)
        {
            zoomObject.transform.position = aimImageTarget.transform.position + new Vector3(0,0,-10);
        }

        zoomObject.SetActive(!isHiAR);
		zoomCamera.enabled = !isHiAR;
		zoomAudioListener.enabled = !isHiAR;*/
	}
}
