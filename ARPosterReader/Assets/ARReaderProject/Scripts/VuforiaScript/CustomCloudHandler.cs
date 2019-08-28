using System.Collections;
using UnityEngine;
using Vuforia;
using UnityEngine.Networking;
using Models;
using ChartAndGraph;

public class CustomCloudHandler : MonoBehaviour, IObjectRecoEventHandler
{
    public ImageTargetBehaviour ImageTargetTemplate;

    private CloudRecoBehaviour mCloudRecoBehaviour;

    private GameObject mBundleInstance;

    private bool mIsScanning = false;

    public GameObject GraphTemplate;

    //private string mTargetMetadata = "";

    public void OnInitialized(TargetFinder targetFinder)
    {
        Debug.Log("Cloud Reco initialized");
    }

    public void OnInitError(TargetFinder.InitState initError)
    {
        Debug.Log("Cloud Reco init error " + initError.ToString());
    }

    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        Debug.Log("Cloud Reco update error " + updateError.ToString());

    }

    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        GameObject newImageTarget = Instantiate(ImageTargetTemplate.gameObject) as GameObject;
        GameObject augmentation = null;
        if (augmentation != null)
            augmentation.transform.SetParent(newImageTarget.transform);

        if (ImageTargetTemplate)
        {
            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            ImageTargetBehaviour imageTargetBehaviour = (ImageTargetBehaviour)tracker.GetTargetFinder<ImageTargetFinder>().EnableTracking(targetSearchResult, newImageTarget);
        }

        if (!mIsScanning)
        {
            mCloudRecoBehaviour.CloudRecoEnabled = true;
        }
        TargetFinder.CloudRecoSearchResult cloudRecoSearchResult = (TargetFinder.CloudRecoSearchResult)targetSearchResult;
        string mTargetId = cloudRecoSearchResult.UniqueTargetId;
        Debug.Log("The result id is " + mTargetId);
        if (mTargetId.Equals("05289705e0124f63b913a9c169d35243"))
        {
            GameObject gmGraph = OnNewSearchGraph();
            Material mat = new Material(Shader.Find("Standard"));
            mat.SetVector("_Color", Color.red);
            mat.SetFloat("_Glossiness", 1.0f);
            //mat.SetVector("_ColorTo", Color.yellow);
            gmGraph.GetComponent<BarChartFeed>().setBars(4, "People", mat);

            Material mat2 = new Material(Shader.Find("Standard"));
            mat2.SetVector("_Color", Color.yellow);
            mat2.SetFloat("_Glossiness", 1.0f);
            gmGraph.GetComponent<BarChartFeed>().setBars(7, "Dogs", mat2);

            gmGraph.transform.parent = newImageTarget.transform;
            gmGraph.transform.localPosition = new Vector3(-2f, -5, -1);
            gmGraph.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            return ;
        }

        
        StartCoroutine(DownloadAndCache(mTargetId,newImageTarget));
        
        Debug.Log("Finished");

    }

    IEnumerator DownloadAndCache(string mTargetId ,GameObject ImageTargetObject)
    {
        while (!Caching.ready)
            yield return null;

        Debug.Log("The result id is " + mTargetId);

        Poster p = new Poster();
        p.targetid = mTargetId;
        p.GetPoster();

        string assetUrl = p.model;

        ImageTargetObject.GetComponent<CustomImageTargetBehaviour>().setPoster(p);

        Debug.Log("The asset url is " + assetUrl);

        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(assetUrl))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Get downloaded asset bundle
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                mBundleInstance = Instantiate(bundle.LoadAsset(bundle.GetAllAssetNames()[0])) as GameObject;
                mBundleInstance.transform.localPosition = new Vector3(0, 0, +200);
                mBundleInstance.transform.localScale = new Vector3(10f, 10f, 10f);
                mBundleInstance.transform.parent = ImageTargetObject.transform;
                
                //mBundleInstance.transform.localPosition = new Vector3(0.0f, 0.15f, 0.0f);
                //mBundleInstance.transform.gameObject.SetActive(true);
            }
        }
    }

    public void OnStateChanged(bool scanning)
    {
        mIsScanning = scanning;

        if (scanning)
        {
            // clear all known trackables
            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

            tracker.GetTargetFinder<ImageTargetFinder>().ClearTrackables(false);
        }
    }

    public GameObject OnNewSearchGraph(){
        GameObject newGMGraph = Instantiate(GraphTemplate.gameObject) as GameObject;
        return newGMGraph;
    }

    // Start is called before the first frame update
    void Start()
    {
        CloudRecoBehaviour mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();

        if (mCloudRecoBehaviour)
        {
            mCloudRecoBehaviour.RegisterEventHandler(this);

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}