using Models;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/**this script is used to show image, 
   When a user touch on thumbnail of a poster in myfavourite scene, it will bring up a larger version of
    thumbnail.

    Author: Ji Heui Yu
    Date: 26/04/2019
**/
public class Button_action : MonoBehaviour {
    public Sprite spr;

    void Start()
    {
        this.gameObject.SetActive (false);
    }

    //Image type only
    public void ShowPhoto(){
        Image img = this.GetComponentInChildren<Image>();
        img.sprite = spr;
    }

}