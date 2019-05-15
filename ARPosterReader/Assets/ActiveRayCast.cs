using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Models;
/**this script is used for "Raycasting, recognizing whether an object has been selected by the user or not", 


Author: Ji Heui Yu
Date: 20/04/2019
**/
public class ActiveRayCast : MonoBehaviour
{
    public Camera c;

    public GameObject Poster;

    Text Title;

    Button b;

    // Use this for initialization
    void Start()
    {
        Title = GameObject.Find("PosterTitle").GetComponent<Text>();
        //Poster = GameObject.Find("HECS_Poster").GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        //for (int i = 0; i < Input.touchCount; ++i){
            //if (Input.GetTouch(i).phase == TouchPhase.Began){
        if (Input.GetMouseButton (0)){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Ray ray = c.ScreenPointToRay(Input.GetTouch(i).position);
            if (Physics.Raycast(ray, out hit)){
                Collider objectHit2 = hit.collider;
                GameObject g = objectHit2.gameObject;
                if (Title.name.Equals("HECS_Poster_Front")){
                rotatePoster();
                }
            }
        //}
        }
    }

//Remove the button
    void Remove(GameObject g){
        Transform objectHit = g.transform;
        g.SetActive(false);
    }

//Rotate 180 degree.
    void rotatePoster(){
        Transform tr = Poster.transform;
        tr.Rotate(0, 0, 180);
        tr.Translate(-7,3,0);

    }
}
