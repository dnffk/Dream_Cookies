using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Networking;

public class BakingManager : MonoBehaviour
{
    [Header("Steps Settings")]
    [SerializeField] private List<BakeStep> steps;    // 단계를 Inspector에서 세팅

    [SerializeField] private TMP_Text requestText;
    [SerializeField] private Transform timeDial;
    [SerializeField] private Transform tempDial;

    [SerializeField] private float timeDialMaxAngle = 180f;

    private int currentStepIndex = 0;
    private int currentTime = 0;
    private int currentTemp = 100;
    void Start()
    {
        if (steps != null && steps.Count > 0)
        {
            requestText.text = steps[currentStepIndex].instruction;
        }

        UpdateTimeDial();
    }

    void Update()
    {
        if (currentStepIndex >= steps.Count)
        {
            return;
        }

        var step = steps[currentStepIndex];

        if(step.isOpenDoorStep)
        {
            OpenClose();
        }
        else if (step.isSetTimeDialStep)
        {
            UpdateTimeDial();
        }
        else if (step.isWaitBakeStep)
        {
            BakingTime();
        }
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

    private void OpenClose()
    {

    }

    private void BakingTime()
    {

    }
}