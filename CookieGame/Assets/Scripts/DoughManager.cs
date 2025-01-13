using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoughManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bowl2D bowl;
    [SerializeField] private ItemPickManager itemPick;
    [SerializeField] private TMP_Text instruction;
    [SerializeField] private GameObject nextButton;

    [Header("Mix Settings")]
    [SerializeField] private float requiredMixTime = 3f;
    private float currentMixTime = 0f;

    // 현재 반죽 단계
    public DoughState currentState = DoughState.AddButterSugar;

    // Bowl 위 터치 판별
    private bool isBowlTouchStarted = false;

    void Start()
    {
        if (instruction) instruction.text = "Butter, Sugar Add";
        if (nextButton) nextButton.SetActive(false);

        // **Add** 단계이므로, 아이템 드래그 기능 켜두기
        if (itemPick) itemPick.gameObject.SetActive(true);
    }

    void Update()
    {
        switch (currentState)
        {
            case DoughState.AddButterSugar:
                // 재료 충분?
                if (bowl.HasItem("Butter") && bowl.HasItem("Sugar"))
                {
                    // 이제 섞기 단계로 전환
                    currentState = DoughState.MixButterSugar;
                    if (instruction) instruction.text = "Good\nMixButterSugar (3sec)";
                    currentMixTime = 0f;

                    // 재료 넣기가 끝났으니 아이템 드래그 비활성
                    if (itemPick) itemPick.gameObject.SetActive(false);
                }
                break;

            case DoughState.MixButterSugar:
                if (IsMixingOverTime())
                {
                    // 섞기 완료 → 다음 단계
                    currentState = DoughState.AddEggSalt;
                    if (instruction) instruction.text = "Now add Egg, Salt";
                    currentMixTime = 0f;

                    // 다음 'Add' 단계 시작이므로 아이템 드래그 다시 활성
                    if (itemPick) itemPick.gameObject.SetActive(true);
                }
                break;

            case DoughState.AddEggSalt:
                if (bowl.HasItem("Egg") && bowl.HasItem("Salt"))
                {
                    currentState = DoughState.MixEggSalt;
                    if (instruction) instruction.text = "Good\nMixEggSalt (3sec)";
                    currentMixTime = 0f;

                    // 재료 넣기 끝 → 드래그 비활성
                    if (itemPick) itemPick.gameObject.SetActive(false);
                }
                break;

            case DoughState.MixEggSalt:
                if (IsMixingOverTime())
                {
                    currentState = DoughState.AddPowderFlourPlus;
                    if (instruction) instruction.text = "Good\nAdd Flour + Powder + SpecialPowder";
                    currentMixTime = 0f;

                    // 다음 Add 단계 → 드래그 활성
                    if (itemPick) itemPick.gameObject.SetActive(true);
                }
                break;

            case DoughState.AddPowderFlourPlus:
                if (bowl.HasItem("Flour") && bowl.HasItem("Powder") && CheckAnyOnePowder())
                {
                    currentState = DoughState.MixPowderFlourPlus;
                    if (instruction) instruction.text = "LastMix (3sec)\nLet's Go";
                    currentMixTime = 0f;

                    // 재료 넣기 완료 → 드래그 비활성
                    if (itemPick) itemPick.gameObject.SetActive(false);
                }
                break;

            case DoughState.MixPowderFlourPlus:
                if (IsMixingOverTime())
                {
                    if (instruction) instruction.text = "You Win!";
                    currentState = DoughState.Finished;

                    if (nextButton) nextButton.SetActive(true);

                    // 모든 반죽 끝나면 굳이 아이템 드래그 활성 안 해도 됨
                }
                break;

            case DoughState.Finished:
                // do nothing
                break;
        }
    }

    private bool IsMixingOverTime()
    {
        // Bowl 위에서 드래그 확인
        if (IsDraggingOnBowl())
        {
            currentMixTime += Time.deltaTime;
            if (currentMixTime >= requiredMixTime)
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
    {
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
                    return false;
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

    private bool CheckAnyOnePowder()
    {
        return (bowl.HasItem("Strawberry powder") ||
                bowl.HasItem("Green tea powder") ||
                bowl.HasItem("Choco powder"));
    }
}
