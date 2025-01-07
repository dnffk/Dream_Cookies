using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round1Manager : MonoBehaviour
{
    private GameObject foodItem;
    private Vector3 offset;
    private float zDistance = 5.0f;

    [SerializeField] private float moveSpeed = 10f;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        Ray ray = Camera.main.ScreenPointToRay(touch.position);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 100f))
                        {
                            foodItem = hit.collider.gameObject;

                            Vector3 itemPos = foodItem.transform.position;
                            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(
                                new Vector3(touch.position.x, touch.position.y, zDistance)
                            );
                            offset = itemPos - touchWorldPos;
                        }
                        break;
                    }

                case TouchPhase.Moved:
                    {
                        if (foodItem != null)
                        {
                            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(
                                new Vector3(touch.position.x, touch.position.y, zDistance)
                            );
                            Vector3 targetPos = touchWorldPos + offset;

                            foodItem.transform.position = Vector3.Lerp(
                                foodItem.transform.position,
                                targetPos,
                                Time.deltaTime * moveSpeed
                            );
                        }
                        break;
                    }

                case TouchPhase.Ended:
                    {
                        if (foodItem != null)
                        {
                            Ray ray = new Ray(foodItem.transform.position, Vector3.down);
                            RaycastHit hit;
                            bool isBowlHit = false;

                            if (Physics.Raycast(ray, out hit, 10f))
                            {
                                if (hit.collider != null && hit.collider.CompareTag("Bowl"))
                                {
                                    isBowlHit = true;
                                }
                            }

                            if (!isBowlHit)
                            {
                                Destroy(foodItem);
                            }
                        }
                        foodItem = null;
                        break;
                    }
            }
        }
    }
}