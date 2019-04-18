using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script is used for the animation of showing poster details in camera view
// written by Norton
public class ScrollUpAnim : MonoBehaviour
{
    private Vector2 initialPosition, targetPosition, terminalPosition; 
    public float SmoothRate = 1.0f;                                     //used to control the speed of animation
    private float halfWidth, halfHeight;                                

    private bool isAnimStart = false;                                   
    private bool isOpen = false;                                        //check the control is open or close
    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;                           //get the initial position of panel
        halfHeight = GameObject.Find("Details").GetComponent<RectTransform>().rect.height;         //get the height of the panel
        terminalPosition = initialPosition + new Vector2(0, halfHeight);
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimStart)   //starts the animation
        {
            transform.position = Vector2.Lerp(transform.position, targetPosition, SmoothRate * 10 * Time.deltaTime);

            if (Vector2.SqrMagnitude(new Vector2(transform.position.x, transform.position.y) - targetPosition) < 0.1)
            {
                transform.position = targetPosition;
                isAnimStart = false;
            }
        }
    }

    /// <summary>
    /// control the pamel is raising or not
    /// </summary>
    public void ShowOrHide()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            targetPosition = terminalPosition;
        }
        else
        {
            targetPosition = initialPosition;
        }
        isAnimStart = true;
    }
}
