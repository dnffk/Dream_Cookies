using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoughManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bowl2D bowl; // Bowl ��ũ��Ʈ ( �浹 ���� )
    [SerializeField] private ItemPickManager itemPick; // item Pick Up ��ũ��Ʈ
    [SerializeField] private TMP_Text request; // ���� ���� ��� �ؽ�Ʈ
    [SerializeField] private GameObject nextButton; // ��� ��ȯ ��ư

    [Header("Mix Settings")]
    [SerializeField] private float reqMixTime = 3f; // �䱸 Mix �ð�
    private float currentMixTime = 0f; // ���� ���� Mix �ð�

    public DoughState currentState = DoughState.AddButterSugar; // enum�� ����� ���� ���� �ܰ� ����, �ʱ� �ܰ�

    private bool isBowlTouchStarted = false; // Bowl ������ ��ġ�� ���۵Ǿ�����

    void Start()
    {
        if (request) request.text = "Butter, Sugar Add";
        if (nextButton) nextButton.SetActive(false); // ��� ��ȯ ��ư ��Ȱ

        if (itemPick) itemPick.gameObject.SetActive(true); // itemPick ��ũ��Ʈ Ȱ��ȭ
    }

    void Update()
    {
        switch (currentState)
        {
            case DoughState.AddButterSugar:
                if (bowl.HasItem("Butter") && bowl.HasItem("Sugar")) // �ش� �±׸� ���� ������Ʈ�� ������ �ֳ�?
                {
                    currentState = DoughState.MixButterSugar; // ������ �ִٸ� MIx �ܰ��
                    if (request) request.text = "Good\nMixButterSugar (3sec)"; // ���� ���� Mix�� ���� ,�ʿ��� �ð� ���
                    if (itemPick) itemPick.gameObject.SetActive(false); // Mix ���� �߿����� item �ű� �� ������ ��ũ��Ʈ ��Ȱ��ȭ
                    
                    currentMixTime = 0f; // ���� Mix�� �ð�
                }
                break;

            case DoughState.MixButterSugar:
                if (IsMixingOverTime())
                {
                    currentState = DoughState.AddEggSalt;
                    if (request) request.text = "Now add Egg, Salt";
                    if (itemPick) itemPick.gameObject.SetActive(true); // Add �ܰ�� �� �� item pick ��Ȱ��ȭ

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
                return true; // ���� �Ϸ�
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