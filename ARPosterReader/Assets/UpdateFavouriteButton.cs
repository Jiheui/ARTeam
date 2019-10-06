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

    Updated on 28/04/2019 by Yufei
**/
public class UpdateFavouriteButton : MonoBehaviour {
    
    public Button favouriteButton = null;
    public Image favouriteImage = null;
    public Text keyId;

    bool isFavor = false;

    public Sprite disable = null;
    public Sprite like = null;
    public Sprite dislike = null;

    // Change the favourite button status to disabled
    public void Start()
    {
        favouriteImage.sprite = disable;
        favouriteButton.interactable = false;
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

        // Here wait 500 milliseconds to ensure the update has happend in database
        //Thread.Sleep(500);
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

    // Set button details by status
    void changeButtonStatus(object stobj)
    {
        bool status = (bool)stobj;

        favouriteButton.interactable = true;
        favouriteButton.gameObject.SetActive(true);
        isFavor = status;
        favouriteImage.sprite = status ? like : dislike;
    }

    // When button clicked send the like or dislike request
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
