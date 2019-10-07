using Models;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
/**this script is used for "favorite", 
    When first clicking the button, it turns red, and then is added to favourites
    the second click of the button, it turns grey, and then is removed from favourites
    The button state keeps alternating

    Author: Daniel
    Date: 11/04/2019
    Yufei Update in 28/04/2019
**/
public class UpdateFavouriteButton : MonoBehaviour {
    
    public Button favouriteButton = null;

    public Image favouriteImage = null;

    public Text keyId;

    bool isFavor = false;

    public Sprite disable = null;

    public Sprite like = null;

    public Sprite dislike = null;

    public void Start()
    {
        // change the favourite button status default set to disable
        favouriteImage.sprite = disable;
        bool hasPoster = !(string.IsNullOrEmpty(keyId.text));
        if (hasPoster)
            favouriteImage.color = new Color32(255, 255, 225, 255);
        else
            favouriteImage.color = new Color32(255, 255, 225, 0);
        //favouriteButton.interactable = false;
    }

    // Update the text on the Button
    public void changeText()
	{
        bool hasPoster = !(string.IsNullOrEmpty(keyId.text));

        // disable button if no poster detected
        if (!hasPoster)
        {
            favouriteImage.sprite = disable;
            favouriteButton.interactable = false;
            favouriteImage.color = new Color(255, 255, 255, 0);
        }
        else
        {
            // Check poster is in the favourite list
            Loom.RunAsync(() => {
                Favourite check = new Favourite();
                check.userid = storeLoginSessionId.loginId;
                Thread thread = new Thread(new ParameterizedThreadStart(getFavoriteList));
                thread.Start(check);
            });
        }
	}

    // Get the Favourite Poster List in async way
    void getFavoriteList(object favourite)
    {
        Favourite check = favourite as Favourite;

        check.GetFavourites();
        Favourite[] favorList = check.favourites;
        foreach (Favourite favor in favorList)
        {
            if (favor.targetid.Equals(keyId.text))
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
        bool hasPoster = !(string.IsNullOrEmpty(keyId.text));

        favouriteButton.interactable = true;
        favouriteButton.gameObject.SetActive(true);
        isFavor = status;
        favouriteImage.sprite = status ? like : dislike;
        if(hasPoster)
            favouriteImage.color = new Color32(255, 255, 225, 255);
        else
            favouriteImage.color = new Color32(255, 255, 225, 0);
    }

    // When button click send the like or dislike request
    public void updateCurPosterFavourite()
    {
        bool hasPoster = !(string.IsNullOrEmpty(keyId.text));
        if (hasPoster)
        {
            Favourite favourite = new Favourite();
            favourite.userid = storeLoginSessionId.loginId;
            favourite.targetid = keyId.text;

            if (isFavor)
            {
                favourite.Dislike();
            }
            else
            {
                favourite.Like();
            }
            changeText();
        }
    }
}
