using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    private float moveSpeed = 0.001f;

    void Start()
    {

    }

    void Update()
    {
        foreach(Touch touch in Input.touches)
        {
            if(touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDelta = touch.deltaPosition;

                if (touchDelta.x != 0 || touchDelta.y != 0)
                {
                    transform.Translate(-touchDelta.x * moveSpeed, touchDelta.y * moveSpeed, 0);
                }
            }
        }
    }
}