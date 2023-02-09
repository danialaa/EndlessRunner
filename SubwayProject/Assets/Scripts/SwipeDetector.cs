using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    [SerializeField]
    int minPixels;
    [SerializeField]
    PlayerController runner;

    Vector2 touchStartPosition;
    bool isTouching = false;

    void Update()
    {
        DetectTouch();
        Swipe();
        DetectTouch();
    }

    void DetectTouch()
    {
        if (!isTouching && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            isTouching = true;
            touchStartPosition = Input.touches[0].position;
        }
        else if (isTouching && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
        {
            isTouching = false;
        }
    }

    void Swipe()
    {
        if(isTouching)
        {
            if(Input.touches[0].position.x >= touchStartPosition.x + minPixels)
            {
                runner.SwitchLanes(false, true);
                isTouching = false;
            }
            else if(Input.touches[0].position.x <= touchStartPosition.x - minPixels)
            {
                runner.SwitchLanes(true, false);
                isTouching = false;
            }
            else if (Input.touches[0].position.y >= touchStartPosition.y + minPixels)
            {
                runner.Jump(true);
                isTouching = false;
            }
        }
    }
}
