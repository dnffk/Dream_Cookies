using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingManager : MonoBehaviour
{
    [SerializeField] private Transform dough;
    [SerializeField] private float scaleFactor = 0.001f;

    private bool isCollidedPin = false;
    private bool isTouchRolling = false;
    private Vector2 lastTouchPos;

    void Start()
    {
        isCollidedPin = false;
    }
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                if (isTouchRolling && isCollidedPin)
                {
                    lastTouchPos = t.position;
                }
            }
            else if (t.phase == TouchPhase.Moved)
            {
                Vector2 currentPos = t.position;
                float distance = (currentPos - lastTouchPos).magnitude;

                Vector2 newScale = dough.localScale;
                newScale.x += distance * scaleFactor;
                newScale.y += distance * scaleFactor / 2f;

                newScale.x = Mathf.Clamp(newScale.x, 0.5f, 3f);
                newScale.y = Mathf.Clamp(newScale.y, 0.3f, 1f);

                dough.localScale = newScale;

                lastTouchPos = currentPos;
            }
        }
    }

    private bool IsTouchOnRollingpin(Touch t)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(t.position);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null && hit.collider.CompareTag("Rolling pin"))
        {
            return true;
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Rolling pin"))
        {
            isCollidedPin = true;
            Debug.Log("isCollidedPin" + isCollidedPin);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Rolling pin"))
        {
            isCollidedPin = false;
            Debug.Log("isCollidedPin" + isCollidedPin);
        }
    }
}