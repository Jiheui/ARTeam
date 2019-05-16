using Models;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**this script is used to show image, 
   When a user touch on thumbnail of a poster in myfavourite scene, it will bring up a larger version of
    thumbnail.

    Author: Ji Heui Yu
    Date: 26/04/2019
**/
public class Button_action : MonoBehaviour {

    public Image img;

    public Sprite OrangeS, PurpleS, BlueS;

    public void ShowPhoto(){

        string name = EventSystem.current.currentSelectedGameObject.name;
        if (name == "Orange"){
            img.sprite = OrangeS;
        }
        if (name == "Purple"){
            img.sprite = PurpleS;
        }
        if (name == "Blue"){
            img.sprite = BlueS;
        }
    }

}