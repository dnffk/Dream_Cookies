using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MixManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bowl2D bowl;              // Bowl 스크립트 (충돌 감지)
    [SerializeField] private ItemPickManager itemPick; // 아이템 픽업 스크립트
    [SerializeField] private TMP_Text requestText;     // 지시 사항 출력할 텍스트
    [SerializeField] private GameObject nextButton;    // 장면 전환 버튼

    [Header("Steps Settings")]
    [SerializeField] private List<MixStep> steps;    // 단계를 Inspector에서 세팅

    private int currentStepIndex = 0;                  // 현재 단계 인덱스
    private float currentMixTime = 0f;                 // 현재 단계에서 누적된 섞기 시간
    private bool isBowlTouchStarted = false;           // Bowl 위에서 터치가 시작되었는지

    private void Start()
    {
        // steps[0]에서 가져옴
        if (steps != null && steps.Count > 0)
        {
            requestText.text = steps[currentStepIndex].instruction;
        }

        // nextButton 비활성화
        if (nextButton) nextButton.SetActive(false);
    }

    private void Update()
    {
        if (currentStepIndex >= steps.Count)
        {
            // 모든 단계가 끝나면 더 이상 진행하지 않음
            return;
        }

        var step = steps[currentStepIndex];

        // 섞기 단계
        if (step.isMixStep)
        {
            // 섞기 단계가 필요한 경우
            if (IsMixingOverTime(step.mixTime))
            {
                GoToNextStep();
            }
        }
        else
        {
            // 재료 추가 단계
            if (CheckAllRequiredItems(step.requiredTags))
            {// requiredTags의 모든 아이템이 Bowl에 들어왔는지 확인
                DestroyItems(step.requiredTags.ToArray());  // Bowl 안에 있는 해당 태그 아이템들 파괴

                // 다음 단계로
                GoToNextStep();
            }
        }
    }

    private void GoToNextStep()
    {
        // 다음 단계 인덱스를 steps에서 가져오기
        int nextIndex = steps[currentStepIndex].nextStepIndex;

        currentStepIndex = nextIndex;  // 현재 단계 인덱스 갱신

        if (currentStepIndex >= steps.Count) // 다음 단계 인덱스가 steps 범위를 벗어난 경우
        {
            if (requestText) requestText.text = "You Win!";
            if (nextButton) nextButton.SetActive(true);
            if (itemPick) itemPick.gameObject.SetActive(true);
            return;
        }

        // 텍스트 갱신
        var step = steps[currentStepIndex];
        if (requestText) requestText.text = step.instruction;

        // 믹싱 단계 들어갈 때는 ItemPick 비활성화, 재료 추가 단계 들어갈 때는 활성화
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
        foreach (var tagName in requiredTags) // requiredTags 중 하나라도 Bowl에 없으면 false
        {
            // 특수 파우더 처리 필요

            if (!bowl.HasItem(tagName))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsMixingOverTime(float reqTime)
    {
        if (IsDraggingOnBowl())
        {
            currentMixTime += Time.deltaTime;
            if (currentMixTime >= reqTime)
            {
                return true; // 섞기 완료
            }
        }
        else
        {
            currentMixTime = 0f;
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
            // Bowl 리스트에서 제거
            bowl.itemsInBowl.Remove(itemObj);
            // 오브젝트 파괴
            Destroy(itemObj);
        }
    }
}
