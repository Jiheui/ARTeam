using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class YNRayCast : MonoBehaviour
{

    GameObject Poster;
    public GameObject BarChart;
    int opYes = 0;
    int opNo = 0;

    // Use this for initialization
    void Start()
    {
        //Title = GameObject.Find("PosterTitle").GetComponent<Text>();
        //Poster = GameObject.Find("HECS_Poster").GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        //for (int i = 0; i < Input.touchCount; ++i){
        //Debug.Log("Start Ray Cast");
        //if (Input.GetTouch(i).phase == TouchPhase.Began){
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                //if (Input.GetMouseButton (0)){
                RaycastHit hit;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Ray ray = c.ScreenPointToRay(Input.GetTouch(i).position);
                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log("Start Ray Cast");
                    Collider objectHit2 = hit.collider;
                    GameObject g = objectHit2.gameObject;
                    Debug.Log("Hit  123 " + isYes);
                    //if (Title.name.Equals("HECS_Poster_Front")){
                    //}
                }
                //}
            }
        }
        */


                if (Input.GetMouseButtonDown(0)){
                    RaycastHit hit;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    //Ray ray = c.ScreenPointToRay(Input.GetTouch(i).position);
                    if (Physics.Raycast(ray, out hit))
                    {
                        //Debug.Log("Start Ray Cast");
                        Collider objectHit2 = hit.collider;
                        GameObject go = objectHit2.gameObject;
                        
                        if (go.name.Equals("Yes")){
                            Material mat = new Material(Shader.Find("Standard"));
                            float r = Random.Range(0f, 1f);
                            float g = Random.Range(0f, 1f);
                            float b = Random.Range(0f, 1f);
                            float a = 0.9f;
                            mat.SetVector("_Color", new Color(r, g, b, a));
                            mat.SetFloat("_Glossiness", 1.0f);
                            opYes += 1;
                            BarChart.GetComponent<BarChartFeed>().setBars(opYes, "Yes", mat);
                            return;
                        }
                        else if (go.name.Equals("No"))
                        {
                            Material mat = new Material(Shader.Find("Standard"));
                            float r = Random.Range(0f, 1f);
                            float g = Random.Range(0f, 1f);
                            float b = Random.Range(0f, 1f);
                            float a = 0.9f;
                            mat.SetVector("_Color", new Color(r, g, b, a));
                            mat.SetFloat("_Glossiness", 1.0f);
                            //mat.SetVector("_ColorTo", Color.yellow);
                            opNo += 1;
                            BarChart.GetComponent<BarChartFeed>().setBars(opNo, "No", mat);
                            return;
                }
                    }
                }
    }
}
