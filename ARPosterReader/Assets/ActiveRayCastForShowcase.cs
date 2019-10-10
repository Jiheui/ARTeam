using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Models;
/**this script is used for "Raycasting, recognizing whether an object has been selected by the user or not", 


Author: Ji Heui Yu
Date: 08/10/2019
**/
public class ActiveRayCastForShowcase : MonoBehaviour
{
    GameObject Poster;
    bool displayed;  

    // Use this for initialization
    void Start()
    {
        Poster = transform.parent.transform.GetChild(1).gameObject;
        displayed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Collider objectHit2 = hit.collider;
                GameObject g = objectHit2.gameObject;
                if (displayed)
                {
                    displayed = false;
                    hideAR();
                }
                else
                {
                    displayAR();
                    displayed = true;
                }
            }
        }
    }

    //Remove the button
    void Remove(GameObject g){
        Transform objectHit = g.transform;
        g.SetActive(false);
    }

    //Rotate 120 degree.
    void rotatePoster()
    {
        Transform tr = Poster.transform;
        tr.Rotate(0, 0, 120);
    }

    //Display the AR of the poster.
    void displayAR()
    {
        GameObject go = transform.parent.transform.GetChild(2).gameObject;
        go.SetActive(true);
    }

    //Hide the AR of the poster.
    void hideAR(){
        GameObject go = transform.parent.transform.GetChild(2).gameObject;
        go.SetActive(false);
    }
}
