using System.Collections;
using UnityEngine;
using Vuforia;
using UnityEngine.Networking;
using Models;

public class CustomCloudHandler : MonoBehaviour, IObjectRecoEventHandler
{
    public ImageTargetBehaviour ImageTargetTemplate;

    private CloudRecoBehaviour mCloudRecoBehaviour;

    private GameObject mBundleInstance;

    private bool mIsScanning = false;

    public GameObject GraphTemplate;

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

        Poster p = new Poster();
        p.targetid = mTargetId;
        p.GetPoster();

        newImageTarget.GetComponent<CustomImageTargetBehaviour>().setPoster(p);

        if (p.type == 2) // 2 is graph type
        {
            Option opt = new Option();
            opt.targetid = mTargetId;
            opt.GetOptions();

            GameObject gmGraph = OnNewSearchGraph();
            foreach (Option op in opt.options)
            {
                Material mat = new Material(Shader.Find("Standard"));
                float r = Random.Range(0f, 1f);
                float g = Random.Range(0f, 1f);
                float b = Random.Range(0f, 1f);
                float a = 0.9f;
                mat.SetVector("_Color", new Color(r,g,b,a));
                mat.SetFloat("_Glossiness", 1.0f);
                //mat.SetVector("_ColorTo", Color.yellow);
                gmGraph.GetComponent<BarChartFeed>().setBars(op.value, op.key, mat);
            }
            gmGraph.transform.parent = newImageTarget.transform;
            gmGraph.transform.localPosition = new Vector3(-2f, -5, -1);
            gmGraph.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            return;
        }
        StartCoroutine(DownloadAndCache(p,newImageTarget));
    }

    IEnumerator DownloadAndCache(Poster p,GameObject ImageTargetObject)
    {
        while (!Caching.ready)
            yield return null;

        string assetUrl = p.model;

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
        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();

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