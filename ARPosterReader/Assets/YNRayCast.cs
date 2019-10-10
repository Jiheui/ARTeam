using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class YNRayCast : MonoBehaviour
{
    public GameObject vote;
    public GameObject graph;

    public GameObject BarChart;

    // Use this for initialization
    void Start()
    {
        BarChart = this.transform.parent.transform.GetChild(3).gameObject;
        buildBarChart();
    }

    public void buildBarChart()
    {
        Option opt = new Option();
        opt.targetid = "566a50d0a56f40a6ac85c5da65b038ed";
        opt.GetOptions();

        foreach (Option o in opt.options)
        {
            Debug.Log("Enter");
            buildSlide(o.value, o.key);
        }
    }

    public void buildSlide(int value, string key)
    {
        Material mat = new Material(Shader.Find("Standard"));
        float r = UnityEngine.Random.Range(0f, 1f);
        float g = UnityEngine.Random.Range(0f, 1f);
        float b = UnityEngine.Random.Range(0f, 1f);
        float a = 0.9f;
        mat.SetVector("_Color", new Color(r, g, b, a));
        mat.SetFloat("_Glossiness", 1.0f);
        BarChart.GetComponent<BarChartFeed>().setBars(value, key, mat);
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    GameObject go = objectHit2.gameObject;
                    try
                    {
                        if (go.name.Equals("Yes") || go.name.Equals("No"))
                        {
                            Option opt = new Option();
                            opt.targetid = "566a50d0a56f40a6ac85c5da65b038ed";
                            opt.key = go.name;
                            opt.Incr();
                            graph.SetActive(true);
                            vote.SetActive(false);
                        }
                    }catch(Exception e)
                    {
                        buildBarChart();
                    }
                    

                    buildBarChart();
                }
                //}
            }
        }
        


        /*if (Input.GetMouseButtonDown(0)){
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Ray ray = c.ScreenPointToRay(Input.GetTouch(i).position);
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("Start Ray Cast");
                Collider objectHit2 = hit.collider;
                GameObject go = objectHit2.gameObject;


                    if (go.name.Equals("Yes"))
                    {
                        Option opt = new Option();
                        opt.targetid = "566a50d0a56f40a6ac85c5da65b038ed";
                        opt.key = "Yes";
                        opt.Incr();
                    }
                    else if (go.name.Equals("No"))
                    {
                        Option opt = new Option();
                        opt.targetid = "566a50d0a56f40a6ac85c5da65b038ed";
                        opt.key = "No";
                        opt.Incr();
                    }

                buildBarChart();
            }
        }*/
        
    }
}
