using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoughManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bowl2D bowl; // Bowl 스크립트 ( 충돌 감지 )
    [SerializeField] private ItemPickManager itemPick; // item Pick Up 스크립트
    [SerializeField] private TMP_Text request; // 지시 사항 출력 텍스트
    [SerializeField] private GameObject nextButton; // 장면 전환 버튼

    [Header("Mix Settings")]
    [SerializeField] private float reqMixTime = 3f; // 요구 Mix 시간
    private float currentMixTime = 0f; // 진행 중인 Mix 시간

    public DoughState currentState = DoughState.AddButterSugar; // enum을 사용해 현재 반죽 단계 지정, 초기 단계

    private bool isBowlTouchStarted = false; // Bowl 위에서 터치가 시작되었는지

    void Start()
    {
        if (request) request.text = "Butter, Sugar Add";
        if (nextButton) nextButton.SetActive(false); // 장면 전환 버튼 비활

        if (itemPick) itemPick.gameObject.SetActive(true); // itemPick 스크립트 활성화
    }

    void Update()
    {
        switch (currentState)
        {
            case DoughState.AddButterSugar:
                if (bowl.HasItem("Butter") && bowl.HasItem("Sugar")) // 해당 태그를 가진 오브젝트를 가지고 있나?
                {
                    currentState = DoughState.MixButterSugar; // 가지고 있다면 MIx 단계로
                    if (request) request.text = "Good\nMixButterSugar (3sec)"; // 지시 사항 Mix로 변경 ,필요한 시간 출력
                    if (itemPick) itemPick.gameObject.SetActive(false); // Mix 과정 중에서는 item 옮길 수 없도록 스크립트 비활성화
                    
                    currentMixTime = 0f; // 현재 Mix된 시간
                }
                break;

            case DoughState.MixButterSugar:
                if (IsMixingOverTime())
                {
                    currentState = DoughState.AddEggSalt;
                    if (request) request.text = "Now add Egg, Salt";
                    if (itemPick) itemPick.gameObject.SetActive(true); // Add 단계로 들어갈 때 item pick 재활성화

                    currentMixTime = 0f;
                }
                break;

            case DoughState.AddEggSalt:
                if (bowl.HasItem("Egg") && bowl.HasItem("Salt"))
                {
                    currentState = DoughState.MixEggSalt;
                    if (request) request.text = "Good\nMixEggSalt (3sec)";
                    if (itemPick) itemPick.gameObject.SetActive(false);

                    currentMixTime = 0f;
                }
                break;

            case DoughState.MixEggSalt:
                if (IsMixingOverTime())
                {
                    currentState = DoughState.AddPowderFlourPlus;
                    if (request) request.text = "Good\nAdd FlourPowderSpecialPowder";
                    if (itemPick) itemPick.gameObject.SetActive(true);

                    currentMixTime = 0f;
                }
                break;

            case DoughState.AddPowderFlourPlus:
                if (bowl.HasItem("Flour") && bowl.HasItem("Powder") && CheckAnyOnePowder())
                {
                    currentState = DoughState.MixPowderFlourPlus;
                    if (request) request.text = "Good\nLastMix (3sec)";
                    if (itemPick) itemPick.gameObject.SetActive(false);

                    currentMixTime = 0f;
                }
                break;

            case DoughState.MixPowderFlourPlus:
                if (IsMixingOverTime())
                {
                    if (request) request.text = "You Win!";
                    currentState = DoughState.Finished;

                    if (nextButton) nextButton.SetActive(true);
                }
                break;

            case DoughState.Finished:
                if (itemPick) itemPick.gameObject.SetActive(true);
                break;
        }
    }

    private bool IsMixingOverTime()
    {
        if (IsDraggingOnBowl())
        {
            currentMixTime += Time.deltaTime;
            if (currentMixTime >= reqMixTime)
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