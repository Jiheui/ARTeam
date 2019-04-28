using Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class Like_Button : MonoBehaviour {

	public Sprite red_heart;
	public Sprite white_heart;
    public string keyGroup;
    public string keyId;

	public void unlikePoster(){
		if (gameObject.GetComponent<Image> ().sprite == red_heart) {
            updateCurPosterFavourite(true);
        } else if (gameObject.GetComponent<Image> ().sprite == white_heart) {
            updateCurPosterFavourite(false);
        } else {
			print ("error!");
		}

	}

    // Update the text on the Button
    public void changeIcon()
    {
        Loom.RunAsync(() => {
            Favourite check = new Favourite();
            check.userid = storeLoginSessionId.loginId;
            Thread thread = new Thread(new ParameterizedThreadStart(getFavoriteList));
            thread.Start(check);
        });
    }

    // Get the Favourite Poster List in async way
    void getFavoriteList(object favourite)
    {
        Favourite check = favourite as Favourite;
        check.GetFavourites();
        Favourite[] favorList = check.favourites;
        foreach (Favourite favor in favorList)
        {
            if (favor.keygroup.Equals(keyGroup) && favor.keyid.Equals(keyId))
            {
                Loom.QueueOnMainThread(changeButtonStatus, true);
                return;
            }
        }
        Loom.QueueOnMainThread(changeButtonStatus, false);

    }

    // Set Button Text by Status
    void changeButtonStatus(object stobj)
    {
        bool status = (bool)stobj;

        if (status)
        {
            gameObject.GetComponent<Image>().sprite = red_heart;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = white_heart;
        }
    }

    // When button click send the like or dislike request
    public void updateCurPosterFavourite(bool isFavor)
    {
        Favourite favourite = new Favourite();
        favourite.userid = storeLoginSessionId.loginId;
        favourite.keygroup = keyGroup;
        favourite.keyid = keyId;

        if (isFavor)
        {
            favourite.Dislike();
        }
        else
        {
            favourite.Like();
        }
        changeIcon();
    }

}
