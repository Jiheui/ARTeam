using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoNotDestroy : MonoBehaviour
{

    public Text keyid;
    void Awake()
    {
        

        DontDestroyOnLoad(keyid);
    }
}
