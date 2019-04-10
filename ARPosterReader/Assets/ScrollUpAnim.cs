using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUpAnim : MonoBehaviour
{
    private Vector2 initialPosition, targetPosition, terminalPosition;  //用于存储菜单动画相关的位置
    public float SmoothRate = 1.0f;                                       //控制动画快慢的参数
    private float halfWidth, halfHeight;                                //用于计算动画移动目的地的参数

    private bool isAnimStart = false;                                   //控制是否开始动画
    private bool isOpen = false;                                        //控制打开还是关闭菜单
    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;                           //获取菜单的初始位置
        halfHeight = GetComponent<RectTransform>().rect.height;         //计算菜单的高
        terminalPosition = initialPosition + new Vector2(0, halfHeight/2);
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimStart)   //菜单动画开始执行
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
    /// 控制菜单弹出还是收回
    /// </summary>
    public void ShowOrHide()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            targetPosition = terminalPosition;   //菜单弹出
        }
        else
        {
            targetPosition = initialPosition;    //菜单收回
        }
        isAnimStart = true;
    }
}
