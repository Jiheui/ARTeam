using System;
using hiscene;
using UnityEngine;

//[RequireComponent(typeof(HiARBaseObjectMovement))]
public class ImageTargetBehaviour : ImageTarget, ITrackableEventHandler, ILoadBundleEventHandler 
{

    private void Start()
    {
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
        }
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public virtual void OnTargetTracked(RecoResult recoResult, Matrix4x4 pose) { }

    public virtual void OnTargetLost(RecoResult recoResult)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
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
