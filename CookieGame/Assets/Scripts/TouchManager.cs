using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private float moveSpeed = 0.001f;

    void Start()
    {

    }

    public void OnPointerDown(PointerEventData data)
    {
        Debug.Log("------Touch Start------");
    }

    public void OnPointerUp(PointerEventData data)
    {
        Debug.Log("------Touch End------");
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