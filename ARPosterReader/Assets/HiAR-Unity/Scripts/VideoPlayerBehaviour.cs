using UnityEngine;
using System;
using hiscene;

public class VideoPlayerBehaviour : VideoPlayer, ILoadVideoEventHandler
{
    private Material m_mError = null;
    private Texture2D m_tError = null;
    private Texture2D m_tLoad = null;
    private Material m_Loading = null;
    private bool isInLoading = false;
    new void Awake()
    {
        base.Awake();
        RegisterLoadVideoEventHandler(this);
        if (m_Loading == null) m_Loading = Resources.Load<Material>("Materials/HIAR_Loading");
        if (m_mError == null) m_mError = Resources.Load<Material>("Materials/HIAR_FAILED");
        if (m_tLoad == null) m_tLoad = Resources.Load<Texture2D>("Materials/loading");
        if (m_tError == null) m_tError = Resources.Load<Texture2D>("Materials/error");
        m_Loading.mainTexture = m_tLoad;
        m_Loading.SetTextureOffset("_MainTex", new Vector2(1, -0.2f));
        m_Loading.SetTextureScale("_MainTex", new Vector2(-1.0f, -0.2f));
    }

    new void Start()
    {
        base.Start();
    }

    public void OnLoadComplete()
    {
        isInLoading = false;
    }

    public void OnLoadError(Exception error)
    {
        isInLoading = false;
        ShowError();
    }

    public void OnLoadProgress(float progress)
    {
        Debug.Log("pro: " + progress);
    }

    public void OnLoadStart(string url)
    {
        GetComponent<Renderer>().material = m_Loading;
        GetComponent<Renderer>().enabled = true;
        isInLoading = true;
    }

    new void Update()
    {
        base.Update();
        if (isInLoading)
            UpdateLoading();
    }

    private void UpdateLoading()
    {
        int index = (int)(Time.time * 6.0f);
        index = index % 5;
        Vector2 size = new Vector2(-1.0f, -0.2f);
        float uIndex = 0;
        float vIndex = index % 5;
        Vector2 offset = new Vector2(1 - uIndex * size.x, size.y + vIndex * size.y);
        m_Loading.SetTextureOffset("_MainTex", offset);
        m_Loading.SetTextureScale("_MainTex", size);
    }
    void ShowError()
    {
        if (transform == null) return;
        if (m_tError == null) return;

        int width = m_tError.width;
        int height = m_tError.height;

        float x = transform.localScale.x;
        float z = transform.localScale.z;
        float min = x < z ? x : z;
        float nx = min == x ? x : (min * width / height);
        float nz = min == z ? z : (min * height / width);

        transform.localScale = new Vector3(nx, 0, nz);
        m_mError.mainTexture = m_tError;
        m_mError.SetTextureOffset("_MainTex", new Vector2(0f, 0f));
        m_mError.SetTextureScale("_MainTex", new Vector2(1f, 1f));
        GetComponent<Renderer>().material = m_mError;
        GetComponent<Renderer>().enabled = true;
    }

    new void OnDisable()
    {
        base.OnDisable();
        isInLoading = false;
    }

    public override void onOpened()
    {
        base.onOpened();
        if (!IsLocal && StreamVideo)
        {
            GetComponent<Renderer>().material = m_Loading;
            GetComponent<Renderer>().enabled = true;
            isInLoading = true;
        }
    }

    public override void onPrepared()
    {
        base.onPrepared();
        if (!IsLocal && StreamVideo)
        {
            isInLoading = false;
        }
    }

}
