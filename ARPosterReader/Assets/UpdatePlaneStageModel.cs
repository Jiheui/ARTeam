using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UpdatePlaneStageModel : MonoBehaviour
{

    public GameObject plane;

    // Start is called before the first frame update
    void Start()
    {
        if (!string.IsNullOrEmpty(ThumbFull.planeDetectModelString))
        {
            Poster p = new Poster();
            p.targetid = ThumbFull.planeDetectModelString;
            p.GetPoster();
            StartCoroutine(DownloadAndCache(p,plane));
        }
    }

    IEnumerator DownloadAndCache(Poster p, GameObject ImageTargetObject)
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
                GameObject mBundleInstance = Instantiate(bundle.LoadAsset(bundle.GetAllAssetNames()[0])) as GameObject;
                mBundleInstance.transform.localPosition = new Vector3(0, 0.072f,0);
                mBundleInstance.transform.rotation = Quaternion.Euler(-90,0, 0);
                mBundleInstance.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                mBundleInstance.transform.parent = ImageTargetObject.transform;
                //mBundleInstance.SetActive(false);
                bundle.Unload(false);
                //mBundleInstance.transform.localPosition = new Vector3(0.0f, 0.15f, 0.0f);
                //mBundleInstance.transform.gameObject.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
