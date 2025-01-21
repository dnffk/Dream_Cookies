using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MixManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bowl2D bowl;              // Bowl ��ũ��Ʈ (�浹 ����)
    [SerializeField] private ItemPickManager itemPick; // ������ �Ⱦ� ��ũ��Ʈ
    [SerializeField] private TMP_Text requestText;     // ���� ���� ����� �ؽ�Ʈ
    [SerializeField] private GameObject nextButton;    // ��� ��ȯ ��ư
    [SerializeField] private Slider slider;    // progressbar

    [Header("Steps Settings")]
    [SerializeField] private List<MixStep> steps;    // �ܰ踦 Inspector���� ����

    private int currentStepIndex = 0;                  // ���� �ܰ� �ε���
    private float currentMixTime = 0f;                 // ���� �ܰ迡�� ������ ���� �ð�
    private bool isBowlTouchStarted = false;           // Bowl ������ ��ġ�� ���۵Ǿ�����
    private float outbowlTime = 0f;
    private float outbowlyesTime = 1f;

    private void Start()
    {
        if (steps != null && steps.Count > 0) // steps[0]���� ������
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
            {// requiredTags�� ��� �������� Bowl�� ���Դ��� Ȯ��
                DestroyItems(step.requiredTags.ToArray());  // Bowl �ȿ� �ִ� �ش� �±� �����۵� �ı�
                GoToNextStep();
            }
        }
    }

    private void GoToNextStep()
    {// ��¥ �������ϳ� �밡�������ߴ�
        // ���� �ܰ� �ε����� steps ���� �����´�
        int nextIndex = steps[currentStepIndex].nextStepIndex;

        currentStepIndex = nextIndex;

        if (currentStepIndex >= steps.Count) // ���� �ܰ� �ε����� steps ������ ��� ���
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
                    return false; // �ϳ��� ������ ����
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
                return true; // ���� �Ϸ�
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
    {// ���� ������ Bowl ������Ʈ ������ ����ǰ� �ִ���
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
        // Bowl���� �ش� �±׵鿡 �ش��ϴ� ������ ���� ã��
        List<GameObject> foundItems = bowl.GetAllItemsByTags(tags);

        foreach (var itemObj in foundItems)
        {
            bowl.itemsInBowl.Remove(itemObj);
            // ������Ʈ �ı�
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