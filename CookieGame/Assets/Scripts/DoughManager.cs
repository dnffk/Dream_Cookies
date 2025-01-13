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

    // ���� ���� �ܰ�
    public DoughState currentState = DoughState.AddButterSugar;

    // Bowl �� ��ġ �Ǻ�
    private bool isBowlTouchStarted = false;

    void Start()
    {
        if (instruction) instruction.text = "Butter, Sugar Add";
        if (nextButton) nextButton.SetActive(false);

        // **Add** �ܰ��̹Ƿ�, ������ �巡�� ��� �ѵα�
        if (itemPick) itemPick.gameObject.SetActive(true);
    }

    void Update()
    {
        switch (currentState)
        {
            case DoughState.AddButterSugar:
                // ��� ���?
                if (bowl.HasItem("Butter") && bowl.HasItem("Sugar"))
                {
                    // ���� ���� �ܰ�� ��ȯ
                    currentState = DoughState.MixButterSugar;
                    if (instruction) instruction.text = "Good\nMixButterSugar (3sec)";
                    currentMixTime = 0f;

                    // ��� �ֱⰡ �������� ������ �巡�� ��Ȱ��
                    if (itemPick) itemPick.gameObject.SetActive(false);
                }
                break;

            case DoughState.MixButterSugar:
                if (IsMixingOverTime())
                {
                    // ���� �Ϸ� �� ���� �ܰ�
                    currentState = DoughState.AddEggSalt;
                    if (instruction) instruction.text = "Now add Egg, Salt";
                    currentMixTime = 0f;

                    // ���� 'Add' �ܰ� �����̹Ƿ� ������ �巡�� �ٽ� Ȱ��
                    if (itemPick) itemPick.gameObject.SetActive(true);
                }
                break;

            case DoughState.AddEggSalt:
                if (bowl.HasItem("Egg") && bowl.HasItem("Salt"))
                {
                    currentState = DoughState.MixEggSalt;
                    if (instruction) instruction.text = "Good\nMixEggSalt (3sec)";
                    currentMixTime = 0f;

                    // ��� �ֱ� �� �� �巡�� ��Ȱ��
                    if (itemPick) itemPick.gameObject.SetActive(false);
                }
                break;

            case DoughState.MixEggSalt:
                if (IsMixingOverTime())
                {
                    currentState = DoughState.AddPowderFlourPlus;
                    if (instruction) instruction.text = "Good\nAdd Flour + Powder + SpecialPowder";
                    currentMixTime = 0f;

                    // ���� Add �ܰ� �� �巡�� Ȱ��
                    if (itemPick) itemPick.gameObject.SetActive(true);
                }
                break;

            case DoughState.AddPowderFlourPlus:
                if (bowl.HasItem("Flour") && bowl.HasItem("Powder") && CheckAnyOnePowder())
                {
                    currentState = DoughState.MixPowderFlourPlus;
                    if (instruction) instruction.text = "LastMix (3sec)\nLet's Go";
                    currentMixTime = 0f;

                    // ��� �ֱ� �Ϸ� �� �巡�� ��Ȱ��
                    if (itemPick) itemPick.gameObject.SetActive(false);
                }
                break;

            case DoughState.MixPowderFlourPlus:
                if (IsMixingOverTime())
                {
                    if (instruction) instruction.text = "You Win!";
                    currentState = DoughState.Finished;

                    if (nextButton) nextButton.SetActive(true);

                    // ��� ���� ������ ���� ������ �巡�� Ȱ�� �� �ص� ��
                }
                break;

            case DoughState.Finished:
                // do nothing
                break;
        }
    }

    private bool IsMixingOverTime()
    {
        // Bowl ������ �巡�� Ȯ��
        if (IsDraggingOnBowl())
        {
            currentMixTime += Time.deltaTime;
            if (currentMixTime >= requiredMixTime)
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
