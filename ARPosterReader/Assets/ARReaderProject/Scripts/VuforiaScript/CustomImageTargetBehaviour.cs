using Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class CustomImageTargetBehaviour : DefaultTrackableEventHandler
{
    private bool targetFound = false;

    Text timeText;
    Text addressText;
    Text linkText;
    Text addressURL;
    Text keyGroup;
    Text keyId;
    Text posterTitle;
    CameraManager eventManager;
    ObserveImageTarget zoomBtn;
    UpdateFavouriteButton favouriteButton;
    UnityEngine.UI.Image favImage;
    UnityEngine.UI.Image ZoomImage;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        posterTitle = GameObject.Find("PosterTitle").GetComponent<Text>();
        timeText = GameObject.Find("Time").GetComponent<Text>();
        addressText = GameObject.Find("Address").GetComponent<Text>();
        linkText = GameObject.Find("Web Link").GetComponent<Text>();
        addressURL = GameObject.Find("Address Url").GetComponent<Text>();
        keyGroup = GameObject.Find("KeyGroup").GetComponent<Text>();
        keyId = GameObject.Find("KeyId").GetComponent<Text>();
        eventManager = GameObject.Find("EventSystem").GetComponent<CameraManager>();
        zoomBtn = GameObject.Find("Zoom Button").GetComponent<ObserveImageTarget>();
        favouriteButton = GameObject.Find("Favourite").GetComponent<UpdateFavouriteButton>();
        zoomBtn.GetComponent<Button>().interactable = false;
        favouriteButton.gameObject.GetComponent<Button>().interactable = false;

        favImage = favouriteButton.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();
        favImage.color = new Color32(255, 255, 225, 0);

        ZoomImage = zoomBtn.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();
        ZoomImage.color = new Color32(255, 255, 225, 0);
    }

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        targetFound = true;
        zoomBtn.imageTargeter = this.gameObject;
        zoomBtn.UpdateTargetBehaviour();
        eventManager.aimImageTarget = this.gameObject;

        // The Method in Loom.Run Async can start a thread. And In the Thread, add the action that can only process on main thread.
        Loom.RunAsync(() => {
            Poster poster = new Poster();
            //poster.keygroup = recoResult.KeyGroup;
            //poster.keyid = recoResult.KeyId;
            Thread thread = new Thread(new ParameterizedThreadStart(getPoster));
            thread.Start(poster);
        });

    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        targetFound = false;
        //zoomBtn.imageTargeter = null;
        //zoomBtn.UpdateTargetBehaviour();
        clearDetail();
    }




    public void getPoster(object obj)
    {
        Poster poster = obj as Poster;
        poster.GetPoster();

        //The action added to Loom.QueueOnMainThread is run on Main Thread.
        Loom.QueueOnMainThread(showDetail, poster);
    }

    public void showDetail(object detail)
    {
        Poster detailPos = detail as Poster;

        posterTitle.text = detailPos.postitle;
        timeText.text = detailPos.posdate;
        addressText.text = detailPos.poslocation;
        linkText.text = detailPos.poslink;
        addressURL.text = detailPos.posmap;
        keyGroup.text = detailPos.keygroup;
        keyId.text = detailPos.keyid;

        GameObject favouriteButton = GameObject.Find("Favourite");
        if (favouriteButton != null && storeLoginSessionId.loginId != -1)
        {
            favouriteButton.GetComponent<UpdateFavouriteButton>().changeText();
        }
    }

    public void clearDetail()
    {
        posterTitle.text = "Title";
        timeText.text = "Time";
        addressText.text = "Address";
        linkText.text = "Web Link";
        addressURL.text = "";
        keyGroup.text = "";

        GameObject favouriteButton = GameObject.Find("Favourite");
        if (favouriteButton != null)
        {
            //favouriteButton.gameObject.SetActive(true);
            favouriteButton.GetComponent<UpdateFavouriteButton>().changeText();
            //favouriteButton.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
