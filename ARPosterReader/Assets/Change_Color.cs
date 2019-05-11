using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_Color : MonoBehaviour {

    Material[] materialsArray;
    Color finalColor;
    Material material;
    int i = 0;
    float coutdown = 15;

    // Use this for initialization
    void Start () {
        this.materialsArray = this.GetComponent<Renderer>().materials;
        finalColor = new Color(0, 1, 0);
        foreach (Material material in materialsArray)
        {
            if (material.name == "CarBody (Instance)")
            {
                this.material = material;
            }
        }

        this.material.SetColor("_Color", finalColor);
    }
	
	// Update is called once per frame
	void Update () {

        coutdown -= Time.deltaTime;
        if(coutdown <= 0)
        {
            Debug.Log("Get In");
            if (i == 0)
            {
                this.material.SetColor("_Color", Color.green);
            }
            else if (i == 1)
            {
                this.material.SetColor("_Color", Color.yellow);
            }
            else
            {
                this.material.SetColor("_Color", Color.white);
            }
            i++;
            i = i % 3;
            coutdown = 15;
        }

        
    }
}
