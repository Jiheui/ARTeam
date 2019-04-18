﻿using System;
using System.Collections;
using System.Threading;
using hiscene;
using Models;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

//[RequireComponent(typeof(HiARBaseObjectMovement))]
public class ImageTargetBehaviour : ImageTarget, ITrackableEventHandler, ILoadBundleEventHandler 
{
	private bool targetFound = false;

    Text timeText;
    Text addressText;
    Text linkText;

    Action<object> showDetailAction;

    private void Start()
    {
        timeText = GameObject.Find("Time").GetComponent<Text>();
        addressText = GameObject.Find("Address").GetComponent<Text>();
        linkText = GameObject.Find("Web Link").GetComponent<Text>();
        showDetailAction = new Action<object>(showDetail);

        if (Application.isPlaying)
        {
            for (var i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(false);
        }
        else
        {
            for (var i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(true);
        }
        RegisterTrackableEventHandler(this);
        RegisterILoadBundleEventHandler(this);
    }

    public void OnLoadBundleStart(string url)
    {
        Debug.Log("load bundle start: " + url);
    }

    public void OnLoadBundleProgress(float progress)
    {
        Debug.Log("load bundle progress: " + progress);
    }

    public void OnLoadBundleComplete() { }

    public virtual void OnTargetFound(RecoResult recoResult)
    {
        if (recoResult.IsCloudReco)
        {
            downloadBundleFromHiAR(recoResult);
            recoResult.KeyGroup = "ARPosterSample";
        }
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
		targetFound = true;

        // The Method in Loom.Run Async can start a thread. And In the Thread, add the action that can only process on main thread.
        Loom.RunAsync(() => {
            Poster poster = new Poster();
            poster.keygroup = recoResult.KeyGroup;
            poster.keyid = recoResult.KeyId;
            poster.GetPoster();
            Thread thread = new Thread(new ParameterizedThreadStart(getPoster));
            thread.Start(poster);
        });
    }

    public void getPoster(object obj)
    {
        Poster poster = obj as Poster;
        poster.GetPoster();

        //The action added to Loom.QueueOnMainThread is run on Main Thread.
        Loom.QueueOnMainThread(showDetailAction, poster.detail);
    }

    public void showDetail(object detail)
    {
        string detailRaw = detail as string;
        string[] detailList = detailRaw.Split(';');

        timeText.text = detailList[0];
        addressText.text = detailList[1];
        linkText.text = detailList[2];
    }

    public void clearDetail()
    {
        timeText.text = "Time";
        addressText.text = "Address";
        linkText.text = "Web Link";
    }

    public void OpenURLOnClick(string urlToOpen)
    {
        bool fail = false;
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

        AndroidJavaObject launchIntent = null;
        try
        {
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", urlToOpen);
        }
        catch (System.Exception e)
        {
            fail = true;
        }

        if (fail)
        { //open app in store
            Application.OpenURL("https://google.com");
        }
        else //open the app
            ca.Call("startActivity", launchIntent);

        up.Dispose();
        ca.Dispose();
        packageManager.Dispose();
        launchIntent.Dispose();
    }

    public virtual void OnTargetTracked(RecoResult recoResult, Matrix4x4 pose) { }

    public virtual void OnTargetLost(RecoResult recoResult)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
		targetFound = false;

        clearDetail();
    }

	public bool IsTargetFound()
	{
		return targetFound;
	}

    public void OnLoadBundleError(Exception error)
    {
        Debug.Log("load bundle error: " + error.ToString());
    }

    public override void ConfigCloudObject(IHsGameObject target)
    {
        try
        {//兼容老版本识别包
            HiARObjectMonoBehaviour oldScript = target.HsGameObjectInstance.GetComponent<HiARObjectMonoBehaviour>();
            if (oldScript != null && target.HsGameObjectInstance.transform.childCount > 0)
            {
                GameObject child = target.HsGameObjectInstance.transform.GetChild(0).gameObject;
                VideoPlayerMonoBehaviour oldVideo = child.GetComponent<VideoPlayerMonoBehaviour>();
                if (oldVideo != null)
                {
                    child.AddComponent<VideoPlayerBehaviour>();
                    VideoPlayerBehaviour player = child.GetComponent<VideoPlayerBehaviour>();
                    player.m_isLocal = false;
                    player.m_webUrl = oldVideo.m_webUrl;
                    if (string.IsNullOrEmpty(player.m_webUrl))
                    {
                        player.m_isLocal = true;
                        player.m_localPath = oldVideo.m_localPath;
                    }
                }
                target.HsGameObjectInstance = child;
            }
        }
        catch (Exception e)
        {
            LogUtil.Log(e.ToString());
        }

        VideoPlayerBehaviour playerSrc = target.HsGameObjectInstance.GetComponent<VideoPlayerBehaviour>();
        if (playerSrc != null)
        {
            target.HsGameObjectInstance.name = "VideoPlayer";
            VideoPlayer.TransParentOptions option = playerSrc.TransParentOption;
            Material material = Resources.Load<Material>("Materials/VIDEO");
            switch (option)
            {
                case VideoPlayer.TransParentOptions.None:
                    if (playerSrc.IsTransparent)
                    {
                        material = Instantiate(Resources.Load<Material>("Materials/VIDEO"));
                        material.shader = Instantiate(Resources.Load<Shader>("Shaders/Transparent_Color"));
                    }
                    else
                    {
                        material.shader = Resources.Load<Shader>("Shaders/video");
                    }
                    break;
                case VideoPlayer.TransParentOptions.TransparentColor:
                    material = Instantiate(Resources.Load<Material>("Materials/VIDEO"));
                    material.shader = Instantiate(Resources.Load<Shader>("Shaders/Transparent_Color"));
                    break;
                case VideoPlayer.TransParentOptions.TransparentLeftAndRight:
                    material.shader = Resources.Load<Shader>("Shaders/TransparentVideo_LeftAndRight");
                    break;
                case VideoPlayer.TransParentOptions.TransparentUpAndDown:
                    material.shader = Resources.Load<Shader>("Shaders/TransparentVideo_UpAndDown");
                    break;
                default:
                    break;
            }
            playerSrc.PlayMaterial = material;
            if(playerSrc.IsTransparent || (playerSrc.TransParentOption == VideoPlayer.TransParentOptions.TransparentColor))
            {
                playerSrc.PlayMaterial.SetFloat("_DeltaColor", playerSrc.DeltaColor);
                playerSrc.PlayMaterial.SetColor("_MaskColor", playerSrc.MaskColor);
            }
        }

        Transform trans = target.HsGameObjectInstance.transform;
        //Vector3 scale = trans.localScale;
        Vector3 position = trans.position;
        Quaternion rotation = trans.rotation;

        trans.position = transform.position;
        trans.rotation = transform.rotation;

        trans.SetParent(transform);

        trans.localPosition = position;
        trans.localRotation = rotation;

        trans.gameObject.SetActive(true);
    }
}
