using UnityEngine;
using System.Collections;
/**this script is used for "Raycasting, recognizing whether an object has been selected by the user or not", 
    

    Author: Ji Heui Yu
    Date: 20/04/2019
**/
public class ActiveRayCast : MonoBehaviour
{
    public Camera c;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton (0)){
            RaycastHit hit;
            Ray ray = c.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray,out hit,100.0f)){
                Transform objectHit = hit.transform;

                objectHit.Translate(100, 0, 0);
            }
        }

    }
}
