using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BakeManager : MonoBehaviour
{
    [Header("Steps Settings")]
    [SerializeField] private BakeStep[] steps; private int currentStepIndex = 0;
    [Header("References")]
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private GameObject nextButton;
    [Header("Dial")]
    [SerializeField] private Transform timeDial;
    [SerializeField] private Transform tempDial;
    [SerializeField] private float timeDialMaxAngle = 180f;
    [SerializeField] private float tempDialMaxAngle = 180f;

    [Header("Baking Rules")]
    [Tooltip("오븐에서 실제 대기할 시간(5초)")]
    [SerializeField] private float bakingWaitSeconds = 5f;
    [Tooltip("적정 구움 시간 (분) 범위")]
    [SerializeField] private int minTime = 10;
    [SerializeField] private int maxTime = 12;
    [Tooltip("적정 온도 범위")]
    [SerializeField] private int minTemp = 170;
    [SerializeField] private int maxTemp = 190;

    private int currentTimeValue = 0;
    private int currentTempValue = 100;

    private Vector2 dragStartPos;

    void Start()
    {
        if (steps != null && steps.Length > 0)
        {
            SetInstruction(steps[currentStepIndex].instruction);
        }
        if (nextButton) nextButton.SetActive(false);

        UpdateTimeDial();
        UpdateTempDial();
        //ovenDoorAnimator.SetFloat("doorOpen", 0f);
    }

    void Update()
    {
        if (currentStepIndex >= steps.Length) return;

        BakeStep step = steps[currentStepIndex];
        
        if (step.isSetTimeDialStep)
        {
            HandleDialInput(true, false);
        }
        else if (step.isSetTempDialStep)
        {
            HandleDialInput(false, true);
        }
        else if (step.isWaitBakeStep)
        {
            StartCoroutine(BakeProcess());
        }
        else if (step.isRemoveCookieStep)
        {
            HandleRemoveCookie();
        }
    }

    #region --------------- 단계 완료 & 다음 단계 이동 ---------------
    private void GoToNextStep()
    {
        int nextIndex = steps[currentStepIndex].nextStepIndex;
        currentStepIndex = nextIndex;

        if (currentStepIndex >= steps.Length)
        {
            SetInstruction("쿠키 굽기 완료!");
            if (nextButton) nextButton.SetActive(true);
            return;
        }

        SetInstruction(steps[currentStepIndex].instruction);
    }
    #endregion

    #region --------------- UI 안내문구 ---------------
    private void SetInstruction(string text)
    {
        if (instructionText != null) instructionText.text = text;
    }
    #endregion

    #region --------------- 3,4) 시간/온도 다이얼 설정 ---------------
    private void HandleDialInput(bool isTime, bool isTemp)
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
                    if (isTime && hit.transform == timeDial)
                    {
                        currentTimeValue += 5;
                        if (currentTimeValue > 30) currentTimeValue = 0;
                        UpdateTimeDial();
                    }
                    if (isTemp && hit.transform == tempDial)
                    {
                        currentTempValue += 10;
                        if (currentTempValue > 230) currentTempValue = 100;
                        UpdateTempDial();
                    }
                }
            }

            if (Input.touchCount > 1)
            {
                GoToNextStep();
            }
        }
    }

    private void UpdateTimeDial()
    {
        float ratio = (float)currentTimeValue / 30f;
        float angle = ratio * timeDialMaxAngle;
        timeDial.localEulerAngles = new Vector3(0f, 0f, -angle);
    }

    private void UpdateTempDial()
    {
        float ratio = (float)(currentTempValue - 100) / 130f; float angle = ratio * tempDialMaxAngle;
        tempDial.localEulerAngles = new Vector3(0f, 0f, -angle);
    }
    #endregion

    #region --------------- 5) 굽기 (5초 대기) ---------------
    private IEnumerator BakeProcess()
    {
        BakeStep step = steps[currentStepIndex];
        step.isWaitBakeStep = false;
        steps[currentStepIndex] = step;

        Debug.Log("쿠키 굽기 시작! (약 " + bakingWaitSeconds + "초 대기)");
        yield return new WaitForSeconds(bakingWaitSeconds);
        Debug.Log("굽기 완료!");

        if (currentTimeValue < minTime || currentTempValue < minTemp)
        {
            Debug.Log("쿠키가 덜 익었어요!");
        }
        else if (currentTimeValue > maxTime || currentTempValue > maxTemp)
        {
            Debug.Log("쿠키가 탔습니다!");
        }
        else
        {
            Debug.Log("쿠키가 맛있게 구워졌습니다!");
        }

        GoToNextStep();
    }
    #endregion

    #region --------------- 7) 쿠키 꺼내기 ---------------
    private void HandleRemoveCookie()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
        }
        if (Input.GetMouseButtonUp(0))
        {
            //cookieAnimator.SetTrigger("TakeCookieOut");
            GoToNextStep();
        }
#else
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                            }
            else if (t.phase == TouchPhase.Ended)
            {
                cookieAnimator.SetTrigger("TakeCookieOut");
                GoToNextStep();
            }
        }
#endif
    }
    #endregion
}