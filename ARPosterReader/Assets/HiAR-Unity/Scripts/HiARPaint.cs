using UnityEngine;
using hiscene;

public class HiARPaint : MonoBehaviour
{
    private Camera mainCamera = null;
    private GameObject[] marks = null;
    private bool isInited;
    private bool fitRender = false;
    private Vector2 ratioForCamera2Screen;

    private HiAREngine engine = null;
    private Target hiarTarget = null;
    public Material currentMaterial = null;
    private Vector2 targetHalfSize;

    void Start()
    {
        isInited = false;
        fitRender = false;
        GetComponent<Renderer>().enabled = false;
        engine = FindObjectOfType<HiAREngine>();
        hiarTarget = GetComponentInParent<Target>();
        targetHalfSize = new Vector2(hiarTarget.PixelWidth, hiarTarget.PixelHeight) * 0.5f;
        if (currentMaterial == null)
        {
            currentMaterial = GetComponent<Renderer>().material;
        }            
        mainCamera = engine.GetComponentInChildren<HiARCamera>().GetComponent<Camera>();

        GameObject hiarMark = new GameObject("hiarMark");
        hiarMark.transform.parent = hiarTarget.transform;
        hiarMark.transform.localPosition = Vector3.zero;
        hiarMark.transform.localRotation = Quaternion.identity;
        hiarMark.transform.localScale = Vector3.one;

        marks = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            marks[i] = new GameObject("mark" + i);
            marks[i].transform.parent = hiarMark.transform;
            float x = (i % 2 == 0) ? -targetHalfSize.x : targetHalfSize.x;
            float y = (i < 2) ? -targetHalfSize.y : targetHalfSize.y;
            marks[i].transform.localPosition = new Vector3(x, 0, y);
            marks[i].transform.localRotation = Quaternion.identity;
            marks[i].transform.localScale = Vector3.one;
        }
    }

    void Init()
    {
        Texture2D texture = BackgroundCamera.Instance().CameraTexture;
        if (texture == null) return;
        currentMaterial.mainTexture = texture;

        float maxScale = Mathf.Max((float)Screen.width / texture.width, (float)Screen.height / texture.height);
        float t2sWidth = texture.width * maxScale;
        float t2sHeight = texture.height * maxScale;

        if (Screen.orientation == ScreenOrientation.Portrait && (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android))
        {
            maxScale = Mathf.Max((float)Screen.height / texture.width, (float)Screen.width / texture.height);
            t2sWidth = texture.width * maxScale;
            t2sHeight = texture.height * maxScale;
            ratioForCamera2Screen.y = Screen.height / t2sWidth;
            ratioForCamera2Screen.x = Screen.width / t2sHeight;
        }
        else
        {
            ratioForCamera2Screen.x = Screen.width / t2sWidth;
            ratioForCamera2Screen.y = Screen.height / t2sHeight;
        }

        if (engine.useSpliteScreen && engine.spliteFullScreen)
        {
            ratioForCamera2Screen.x = ratioForCamera2Screen.x / 2.0f;
        }

        isInited = true;
    }

    void OnEnable()
    {
        GetComponent<Renderer>().enabled = false;
    }

    void OnDisable()
    {
        isInited = false;
        fitRender = false;
    }

    void LateUpdate()
    {
        if (fitRender)
        {
            GetComponent<Renderer>().enabled = true;
        }
        if (!isInited)
        {
            Init();
        }
        UpdateMatrix();
        fitRender = true;   
    }

    void UpdateMatrix()
    {
        if (mainCamera == null) return;

        Vector3 p0, p1, p2, p3;
        p0 = CalculateUVCoordinate(mainCamera.WorldToViewportPoint(marks[0].transform.position));
        p1 = CalculateUVCoordinate(mainCamera.WorldToViewportPoint(marks[1].transform.position));
        p2 = CalculateUVCoordinate(mainCamera.WorldToViewportPoint(marks[2].transform.position));
        p3 = CalculateUVCoordinate(mainCamera.WorldToViewportPoint(marks[3].transform.position));

        currentMaterial.SetVector("p0", p0);
        currentMaterial.SetVector("p1", p1);
        currentMaterial.SetVector("p2", p2);
        currentMaterial.SetVector("p3", p3);
    }

    Vector3 CalculateUVCoordinate(Vector3 v)
    {
        float screenX = (1.0f - ratioForCamera2Screen.x) * 0.5f + v.x * ratioForCamera2Screen.x;
        float screenY = (1.0f - ratioForCamera2Screen.y) * 0.5f + v.y * ratioForCamera2Screen.y;

        if (Screen.orientation == ScreenOrientation.Portrait && (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android))
        {
            float y = 1 - screenY, x = 1 - screenX;
            if (HiAREngine.ContentFlipH)
            {
                y = 1 - y;
            }
            if (HiAREngine.ContentFlipV)
            {
                x = 1 - x;
            }
            return new Vector3(y * v.z, x * v.z, v.z);
        }
        else
        {
            float x = screenX, y = 1 - screenY;
            if (HiAREngine.ContentFlipH)
            {
                x = 1 - x;
            }
            if (HiAREngine.ContentFlipV)
            {
                y = 1 - y;
            }
            return new Vector3(x * v.z, y * v.z, v.z);
        }
    }

    void OnDestroy()
    {
        if (marks != null && marks.Length != 0)
        {
            for (int i = 0; i < marks.Length; i++)
            {
                DestroyImmediate(marks[i]);
            }
        }
    }
}