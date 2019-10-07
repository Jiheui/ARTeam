using UnityEngine;

public class ToggleSideMenu : MonoBehaviour
{

    public GameObject sideMenu;

    private bool visibility = false;

    // Start is called before the first frame update
    void Start()
    {
        sideMenu.SetActive(false);
    }

    public void ToggleVisibility()
    {
        visibility = !visibility;
        sideMenu.SetActive(visibility);
    }
}
