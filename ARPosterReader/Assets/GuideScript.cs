using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideScript : MonoBehaviour
{
    public GameObject guidepanel;

    private static bool isFirst = true;
    // Start is called before the first frame update
    void Start()
    {
        if (isFirst)
        {
            guidepanel.SetActive(true);
            isFirst = false;
        }
        else
        {
            guidepanel.SetActive(false);
        }
    }

    public void hideGuidePanel()
    {
        guidepanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
