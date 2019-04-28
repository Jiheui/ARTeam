using Models;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
/**this script is used for "favorite", 
    when first click the button, it changes to red, then add to favourite
    second click the botton, it turns into write, remove from favorite
    Take turns to happen this

    Author: Daniel
    Date: 11/04/2019
    Yufei Update in 28/04/2019
**/
public class UpdateFavouriteButton : MonoBehaviour {

	public Text favouriteText = null;

    public Button favouriteButton = null;

    public Text keyGroup;

    public Text keyId;

    public bool isFavor = false;

    Action<object> changeButtonText;

    public void Start()
    {
        // change the favourite button status default set to disable
        favouriteButton.interactable = false;
        changeButtonText = new Action<object>(changeButtonStatus);
    }

    // Update the text on the Button
    public void changeText()
	{
        bool hasPoster = !(string.IsNullOrEmpty(keyGroup.text) || string.IsNullOrEmpty(keyId.text));

        // disable button if no poster detected
        if (!hasPoster)
        {
            favouriteText.text = "favourite";
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
        Thread.Sleep(500);
        check.GetFavourites();
        Favourite[] favorList = check.favourites;
        foreach (Favourite favor in favorList)
        {
            if (favor.keygroup.Equals(keyGroup.text) && favor.keyid.Equals(keyId.text))
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
            favouriteText.text = "remove";
            favouriteButton.interactable = true;
            isFavor = true;
        }
        else
        {
            favouriteText.text = "favourite";
            favouriteButton.interactable = true;
            isFavor = false;
        }
    }

    // When button click send the like or dislike request
    public void updateCurPosterFavourite()
    {
        bool hasPoster = !(string.IsNullOrEmpty(keyGroup.text) || string.IsNullOrEmpty(keyId.text));
        if (hasPoster)
        {
            Favourite favourite = new Favourite();
            favourite.userid = storeLoginSessionId.loginId;
            favourite.keygroup = keyGroup.text;
            favourite.keyid = keyId.text;

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
