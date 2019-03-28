using UnityEngine;

public class HiAREngineBehaviour : HiAREngine
{
    void LateUpdate()
    {
        if (Application.isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit(); // On Android, maps to "back" button.
        }
    }
}
