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

    public Text timeText;
    public Text addressText;
    public Text linkText;
    public Text addressURL;
    public Text keyId;
    public Text posterTitle;
    public CameraManager eventManager;
    public ObserveImageTarget zoomBtn;
    public UpdateFavouriteButton favouriteButton;
    UnityEngine.UI.Image favImage;
    UnityEngine.UI.Image ZoomImage;

    Poster poster;

    public void setPoster(Poster p)
    {
        poster = p;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
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
        if (poster != null)
        {
            Loom.RunAsync(() => {
                Thread thread = new Thread(new ParameterizedThreadStart(getPoster));
                thread.Start(poster);
            });
        }
        

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
        keyId.text = detailPos.targetid;

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
