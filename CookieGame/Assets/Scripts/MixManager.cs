using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MixManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bowl2D bowl;              // Bowl 스크립트 (충돌 감지)
    [SerializeField] private ItemPickManager itemPick; // 아이템 픽업 스크립트
    [SerializeField] private TMP_Text requestText;     // 지시 사항 출력할 텍스트
    [SerializeField] private GameObject nextButton;    // 장면 전환 버튼
    [SerializeField] private Slider slider;    // progressbar

    [Header("Steps Settings")]
    [SerializeField] private List<MixStep> steps;    // 단계를 Inspector에서 세팅

    private int currentStepIndex = 0;                  // 현재 단계 인덱스
    private float currentMixTime = 0f;                 // 현재 단계에서 누적된 섞기 시간
    private bool isBowlTouchStarted = false;           // Bowl 위에서 터치가 시작되었는지
    private float outbowlTime = 0f;
    private float outbowlyesTime = 1f;

    private void Start()
    {
        if (steps != null && steps.Count > 0) // steps[0]에서 가져옴
        {
            requestText.text = steps[currentStepIndex].instruction;
        }

        slider.value = 0f;
        slider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (currentStepIndex >= steps.Count)
        {
            return;
        }

        var step = steps[currentStepIndex];

        if (step.isMixStep)
        {
            if (!slider.gameObject.activeSelf)
            {
                slider.gameObject.SetActive(true);
            }

            if (IsMixingOverTime(step.mixTime))
            {
                GoToNextStep();
            }

            ProgressBar(step.mixTime);
        }
        else
        {
            if (slider.gameObject.activeSelf)
            {
                slider.gameObject.SetActive(false);
            }

            if (CheckAllRequiredItems(step.requiredTags))
            {// requiredTags의 모든 아이템이 Bowl에 들어왔는지 확인
                DestroyItems(step.requiredTags.ToArray());  // Bowl 안에 있는 해당 태그 아이템들 파괴
                GoToNextStep();
            }
        }
    }

    private void GoToNextStep()
    {// 진짜 개복잡하네 대가리터지긋다
        // 다음 단계 인덱스를 steps 에서 가져온다
        int nextIndex = steps[currentStepIndex].nextStepIndex;

        currentStepIndex = nextIndex;

        if (currentStepIndex >= steps.Count) // 다음 단계 인덱스가 steps 범위를 벗어난 경우
        {
            if (requestText) requestText.text = "";
            if (nextButton) nextButton.SetActive(true);
            if (itemPick) itemPick.gameObject.SetActive(true);
            return;
        }

        var step = steps[currentStepIndex];
        if (requestText) requestText.text = step.instruction;

        if (step.isMixStep)
        {
            if (itemPick) itemPick.gameObject.SetActive(false);
            currentMixTime = 0f;
        }
        else
        {
            if (itemPick) itemPick.gameObject.SetActive(true);
        }
    }

    private bool CheckAllRequiredItems(List<string> requiredTags)
    {
        var step = steps[currentStepIndex];

        if (step.isLastStep)
        {
            bool hasStrawberry = bowl.HasItem("Strawberry powder");
            bool hasChoco = bowl.HasItem("Choco powder");
            bool hasGreenTea = bowl.HasItem("Green tea powder");

            if (bowl.HasItem("Flour") && bowl.HasItem("Powder") && (hasStrawberry || hasChoco || hasGreenTea))
            {
                if (hasStrawberry)
                {
                    CheckItemManager.Instance.UseItem(0);
                }
                else if (hasChoco)
                {
                    CheckItemManager.Instance.UseItem(1);
                }
                else if (hasGreenTea)
                {
                    CheckItemManager.Instance.UseItem(2);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            foreach (var tagName in requiredTags)
            {
                if (!bowl.HasItem(tagName))
                {
                    return false; // 하나라도 없으면 실패
                }
            }
            return true;
        }
    }

    private bool IsMixingOverTime(float reqTime)
    {
        if (IsDraggingOnBowl())
        {
            currentMixTime += Time.deltaTime;

            outbowlTime = 0f;

            if (currentMixTime >= reqTime)
            {
                return true; // 섞기 완료
            }
        }
        else
        {
            outbowlTime += Time.deltaTime;

            if(outbowlTime > outbowlyesTime)
            {
                currentMixTime = 0f;
            }
        }
        return false;
    }

    private bool IsDraggingOnBowl()
    {// 섞기 과정이 Bowl 오브젝트 위에서 진행되고 있는지
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (IsTouchOnBowl(touch))
                    {
                        isBowlTouchStarted = true;
                        currentMixTime = 0f;
                    }
                    else
                    {
                        isBowlTouchStarted = false;
                    }
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (isBowlTouchStarted)
                        return true;
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isBowlTouchStarted = false;
                    break;
            }
        }
        return false;
    }

    private bool IsTouchOnBowl(Touch t)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(t.position);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null && hit.collider.CompareTag("Bowl"))
        {
            return true;
        }
        return false;
    }

    private void DestroyItems(params string[] tags)
    {
        // Bowl에서 해당 태그들에 해당하는 아이템 전부 찾기
        List<GameObject> foundItems = bowl.GetAllItemsByTags(tags);

        foreach (var itemObj in foundItems)
        {
            bowl.itemsInBowl.Remove(itemObj);
            // 오브젝트 파괴
            Destroy(itemObj);
        }
    }

    private void ProgressBar(float reqTime)
    {
        slider.value = currentMixTime / reqTime;

        if(slider.value >= 1)
        {
            slider.gameObject.SetActive(false);
        }
    }
}