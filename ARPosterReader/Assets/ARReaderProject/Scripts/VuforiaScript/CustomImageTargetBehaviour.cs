using Models;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CustomImageTargetBehaviour : DefaultTrackableEventHandler
{
    private bool targetFound = false;

    public Text timeText;
    public Text addressText;
    public Text linkText;
    public Text addressURL;
    public Text keyId;
    public Text posterTitle;
    public Text relevantInfo;
    public CameraManager eventManager;
    public ObserveImageTarget zoomBtn;
    public UpdateFavouriteButton favouriteButton;
    public GameObject inputOpt;
    public GameObject inputOpts;
    public Transform parentTransForm;
    Image favImage;
    Poster poster;
    Image share;
    
    public void setPoster(Poster p)
    {
        poster = p;
        Debug.Log("From CustomImageTargetBehavoir"+ poster.targetid);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        zoomBtn.GetComponent<Button>().interactable = false;
        favouriteButton.gameObject.GetComponent<Button>().interactable = false;

        favImage = favouriteButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        favImage.color = new Color32(255, 255, 225, 0);

        
    }

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        targetFound = true;
        zoomBtn.imageTargeter = this.gameObject;
        zoomBtn.UpdateTargetBehaviour();
        //favouriteButton.changeText();
        eventManager.aimImageTarget = this.gameObject;
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

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
        Debug.Log("Enter Tracking Lost");
        base.OnTrackingLost();
        targetFound = false;
        zoomBtn.imageTargeter = null;
        zoomBtn.UpdateTargetBehaviour();
        clearDetail();
        favouriteButton.changeText();
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public bool IsTargetFound()
    {
        return targetFound;
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
        relevantInfo.text = detailPos.relevantinfo;
        
        if (storeLoginSessionId.loginId != -1)
        {
            favouriteButton.changeText();
        }

        InputOption inputOption = new InputOption();
        inputOption.targetid = detailPos.targetid;
        Debug.Log(detailPos.targetid);
        inputOption.GetInputOptions();

        Debug.Log(inputOption.questions.Length);
        if (inputOption.questions == null || inputOption.questions.Length == 0)
        {
            
            inputOpt.SetActive(false);
        }
        else
        {
            if(inputOption.questions[0].tid == 1) //text input
            {
                
                Text nameI = inputOpt.GetComponentsInChildren<Text>()[0];
                nameI.text = inputOption.questions[0].name;
                InputField iptf = inputOpt.GetComponentsInChildren<InputField>()[0];
                iptf.gameObject.SetActive(true);
                Dropdown dpd= inputOpt.GetComponentsInChildren<Dropdown>()[0];
                dpd.gameObject.SetActive(false);
                Toggle tg = inputOpt.GetComponentsInChildren<Toggle>()[0];
                tg.gameObject.SetActive(false);
            }else if(inputOption.questions[0].tid == 2)
            {
                
                Text nameI = inputOpt.GetComponentsInChildren<Text>()[0];
                nameI.text = inputOption.questions[0].name;
                InputField iptf = inputOpt.GetComponentsInChildren<InputField>()[0];
                iptf.gameObject.SetActive(false);
                Dropdown dpd = inputOpt.GetComponentsInChildren<Dropdown>()[0];
                dpd.gameObject.SetActive(true);
                Toggle tg = inputOpt.GetComponentsInChildren<Toggle>()[0];
                tg.gameObject.SetActive(false);
            }
            else
            {
                
                Text nameI = inputOpt.GetComponentsInChildren<Text>()[0];
                nameI.text = inputOption.questions[0].name;
                InputField iptf = inputOpt.GetComponentsInChildren<InputField>()[0];
                iptf.gameObject.SetActive(false);
                Dropdown dpd = inputOpt.GetComponentsInChildren<Dropdown>()[0];
                dpd.gameObject.SetActive(false);
                Toggle tg = inputOpt.GetComponentsInChildren<Toggle>()[0];
                tg.gameObject.SetActive(true);
            }

            //for (int i = 1; i < inputOption.questions.Length; i++)
            //{
                
            //    parentTransForm = inputOpt.transform.parent;
            //    inputOpts = GameObject.Instantiate(inputOpt, parentTransForm);
            //    if (inputOption.questions[i].tid == 1) //text input
            //    {
            //        Text[] nameIs = inputOpts.GetComponentsInChildren<Text>();
            //        foreach (Text nameI1 in nameIs)
            //        {
            //            nameI1.text = inputOption.questions[i].name;
            //        }
            //        InputField iptf = inputOpts.GetComponentsInChildren<InputField>()[0];
            //        iptf.gameObject.SetActive(true);
            //        Dropdown dpd = inputOpts.GetComponentsInChildren<Dropdown>()[0];
            //        dpd.gameObject.SetActive(false);
            //        Toggle tg = inputOpts.GetComponentsInChildren<Toggle>()[0];
            //        tg.gameObject.SetActive(false);
            //    }
            //    else if (inputOption.questions[i].tid == 2)
            //    {

            //        Text[] nameIs = inputOpts.GetComponentsInChildren<Text>();
            //        foreach (Text nameI1 in nameIs)
            //        {
            //            nameI1.text = inputOption.questions[i].name;
            //        }
            //        InputField iptf = inputOpts.GetComponentsInChildren<InputField>()[0];
            //        iptf.gameObject.SetActive(false);
            //        Dropdown dpd = inputOpts.GetComponentsInChildren<Dropdown>()[0];
            //        dpd.gameObject.SetActive(true);
            //        Toggle tg = inputOpts.GetComponentsInChildren<Toggle>()[0];
            //        tg.gameObject.SetActive(false);
            //    }
            //    else
            //    {

            //        Text[] nameIs = inputOpts.GetComponentsInChildren<Text>();
            //        foreach (Text nameI1 in nameIs)
            //        {
            //            nameI1.text = inputOption.questions[i].name;
            //        }
            //        InputField iptf = inputOpts.GetComponentsInChildren<InputField>()[0];
            //        iptf.gameObject.SetActive(false);
            //        Dropdown dpd = inputOpts.GetComponentsInChildren<Dropdown>()[0];
            //        dpd.gameObject.SetActive(false);
            //        Toggle tg = inputOpts.GetComponentsInChildren<Toggle>()[0];
            //        tg.gameObject.SetActive(true);
            //    }
            //}

        }
        

    }

    public void clearDetail()
    {
        posterTitle.text = "Title";
        timeText.text = "Time";
        addressText.text = "Address";
        linkText.text = "Web Link";
        addressURL.text = "";
        keyId.text = "";
        relevantInfo.text = "Relevant Information";
        inputOpt.GetComponentsInChildren<Text>()[0].text = "Name";
        
        if (favouriteButton != null)
        {
            favouriteButton.changeText();
        }
    }
}
