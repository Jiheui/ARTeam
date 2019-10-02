using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using static CustomCloudHandler;

public class ReportSystem : MonoBehaviour
{
    public Toggle isViolence;
    public Toggle isEroticism;
    public Toggle isFraud;

    // record which toggle is on, 1 is violence, 2 is eroticisim, 3 is fraud
    public int whichIsOn;
    // the text report
    public InputField report;

    // check activity toggle
    public int ActivityToggle()
    {
        int i = 0;
        if (isViolence.isOn)
        {
            i = 1;
            Debug.Log("isViolence is on"); 
        }
        else if (isEroticism.isOn)
        {
            i = 2;
            Debug.Log("isEroticism is on");
        }
        else if (isFraud.isOn)
        {
            i = 3;
            Debug.Log("isEroticism is on");
        }
        return i;
    }

    public void onSubmit()
    {

        whichIsOn = ActivityToggle();
        
        Report r = new Report();
       


        r.targetid = posterSessionId.posterId;
        r.userid = storeLoginSessionId.loginId;
        r.detail = report.text;




        // did not click any toggle
        if (whichIsOn == 0)
        {
            r.type = whichIsOn;
            Debug.Log("which is on:   " + whichIsOn);
            Debug.Log("Report text is:   "+ report.text);
        }
        else
        {
            r.type = whichIsOn;
            // DataBase operation
            Debug.Log("which is on:   " + whichIsOn);
            Debug.Log("Report text is:   " + report.text);


        }
        r.SendReport();


    }

}
