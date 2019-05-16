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

	//public Text favouriteText = null;

    public Button favouriteButton = null;

    public Image favouriteImage = null;

    public Text keyGroup;

    public Text keyId;

    public bool isFavor = false;

    public Sprite disable = null;

    public Sprite like = null;

    public Sprite dislike = null;

    public void Start()
    {
        // change the favourite button status default set to disable
        //favouriteImage.sprite = disable;
        //favouriteButton.gameObject.SetActive(false);
    }

    // Update the text on the Button
    public void changeText()
	{
        bool hasPoster = !(string.IsNullOrEmpty(keyGroup.text) || string.IsNullOrEmpty(keyId.text));

        // disable button if no poster detected
        if (!hasPoster)
        {
            //favouriteText.text = "favourite";
            Debug.Log("Exec");
            favouriteImage.sprite = disable;
            //favouriteImage.gameObject.SetActive(false);
            //favouriteButton.gameObject.SetActive(false);
            favouriteButton.interactable = false;
        }
        else
        {
            /*if(storeLoginSessionId.loginId == -1)
            {
                return;
            }*/
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
            //favouriteText.text = "remove";
            favouriteImage.sprite = like;
            favouriteButton.interactable = true;
            //favouriteButton.gameObject.SetActive(true);
            isFavor = true;
        }
        else
        {
            //favouriteText.text = "favourite";
            favouriteImage.sprite = dislike;
            favouriteButton.interactable = true;
            //favouriteButton.gameObject.SetActive(true);
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
