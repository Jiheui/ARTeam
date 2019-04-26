using UnityEngine;
using System.Collections;
/**this script is used for "Making thumbnail fullsize", 
   Makes sure that panel that will display full size of a poster to be inactive at the start.
    

    Author: Ji Heui Yu
    Date: 20/04/2019
**/
public class ActiveFullPoster : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        //Panel inactive
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
