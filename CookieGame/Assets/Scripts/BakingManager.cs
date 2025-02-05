using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakingManager : MonoBehaviour
{
    [SerializeField] private Transform timeDial;
    [SerializeField] private Transform tempDial;

    [SerializeField] private float timeDialMaxAngle = 180f;
    [SerializeField] private float tempDialMaxAngle = 180f;

    private int currentTime = 0;
    private int currentTemp = 100;
    void Start()
    {
        UpdateTimeDial();
        UpdateTempDial();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(t.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == timeDial)
                    {
                        currentTime += 5;
                        if (currentTime > 30)
                        {
                            currentTime = 0;
                        }
                        UpdateTimeDial();
                    }
                    else if (hit.transform == tempDial)
                    {
                        currentTemp += 10;
                        if (currentTemp > 230)
                        {
                            currentTemp = 100;
                        }
                        UpdateTempDial();
                    }
                }
            }
        }
    }

    private void UpdateTimeDial()
    {
        float ratio = (float)currentTime / 30f;
        float angle = ratio * timeDialMaxAngle;
        timeDial.localEulerAngles = new Vector3(0f, 0f, -angle);
    }

    private void UpdateTempDial()
    {
        float ratio = (float)(currentTemp - 100) / 130f;
        float angle = ratio * tempDialMaxAngle;
        tempDial.localEulerAngles = new Vector3(0f, 0f, -angle);
    }
}