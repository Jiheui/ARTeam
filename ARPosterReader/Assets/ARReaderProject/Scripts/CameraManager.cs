using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState{
	AR,
	Zoom
}

public class CameraManager : MonoBehaviour {

    //private HiAREngineBehaviour HiAREngineBehaviour;
    public Camera ARCamera;
    public Camera zoomCamera;
    private GameObject aimObject = null;

    public GameObject aimImageTarget = null;

    private CameraState cameraState = CameraState.AR;

	public float zoomSpeed = 0.9f;
    
    private Touch oldTouch1; // store the finger touch point
    private Touch oldTouch2; // store the finger touch point
    
    void Start () {
        zoomCamera.enabled = false;
    }

	void Update () {
		if (cameraState == CameraState.Zoom) {
			if (Input.mouseScrollDelta.y > 0) {
                Debug.Log("Enter");
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

                for (var i = 0; i < aimObject.transform.childCount; i++)
                {
                    aimObject.transform.GetChild(i).transform.Rotate(direction * deltaPos.x, Space.World);

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
        if (aimImageTarget == null)
        {
            return;
        }
        cameraState = (cameraState == CameraState.AR) ? CameraState.Zoom : CameraState.AR;
		SetCameras(cameraState);
		Debug.Log ("Test " + str);
	}

	void SetCameras(CameraState state){
        bool isAR = state == CameraState.AR;

        if (!isAR)
        {
            aimObject = Instantiate(aimImageTarget.transform.GetChild(2)).gameObject;
            aimObject.transform.parent = zoomCamera.transform.GetChild(0).gameObject.transform;
            aimObject.transform.localPosition = new Vector3(0, 0, 0);
            aimObject.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        }
        else
        {
            if(aimObject != null)
                Destroy(aimObject);
        }

        ARCamera.enabled = isAR;
        zoomCamera.enabled = !isAR;
	}
}
