using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MixManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bowl2D bowl;              // Bowl ��ũ��Ʈ (�浹 ����)
    [SerializeField] private ItemPickManager itemPick; // ������ �Ⱦ� ��ũ��Ʈ
    [SerializeField] private TMP_Text requestText;     // ���� ���� ����� �ؽ�Ʈ
    [SerializeField] private GameObject nextButton;    // ��� ��ȯ ��ư

    [Header("Steps Settings")]
    [SerializeField] private List<MixStep> steps;    // �ܰ踦 Inspector���� ����

    private int currentStepIndex = 0;                  // ���� �ܰ� �ε���
    private float currentMixTime = 0f;                 // ���� �ܰ迡�� ������ ���� �ð�
    private bool isBowlTouchStarted = false;           // Bowl ������ ��ġ�� ���۵Ǿ�����

    private void Start()
    {
        // steps[0]���� ������
        if (steps != null && steps.Count > 0)
        {
            requestText.text = steps[currentStepIndex].instruction;
        }

        // nextButton ��Ȱ��ȭ
        if (nextButton) nextButton.SetActive(false);
    }

    private void Update()
    {
        if (currentStepIndex >= steps.Count)
        {
            // ��� �ܰ谡 ������ �� �̻� �������� ����
            return;
        }

        var step = steps[currentStepIndex];

        // ���� �ܰ�
        if (step.isMixStep)
        {
            // ���� �ܰ谡 �ʿ��� ���
            if (IsMixingOverTime(step.mixTime))
            {
                GoToNextStep();
            }
        }
        else
        {
            // ��� �߰� �ܰ�
            if (CheckAllRequiredItems(step.requiredTags))
            {// requiredTags�� ��� �������� Bowl�� ���Դ��� Ȯ��
                DestroyItems(step.requiredTags.ToArray());  // Bowl �ȿ� �ִ� �ش� �±� �����۵� �ı�

                // ���� �ܰ��
                GoToNextStep();
            }
        }
    }

    private void GoToNextStep()
    {
        // ���� �ܰ� �ε����� steps���� ��������
        int nextIndex = steps[currentStepIndex].nextStepIndex;

        currentStepIndex = nextIndex;  // ���� �ܰ� �ε��� ����

        if (currentStepIndex >= steps.Count) // ���� �ܰ� �ε����� steps ������ ��� ���
        {
            if (requestText) requestText.text = "You Win!";
            if (nextButton) nextButton.SetActive(true);
            if (itemPick) itemPick.gameObject.SetActive(true);
            return;
        }

        // �ؽ�Ʈ ����
        var step = steps[currentStepIndex];
        if (requestText) requestText.text = step.instruction;

        // �ͽ� �ܰ� �� ���� ItemPick ��Ȱ��ȭ, ��� �߰� �ܰ� �� ���� Ȱ��ȭ
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
        foreach (var tagName in requiredTags) // requiredTags �� �ϳ��� Bowl�� ������ false
        {
            // Ư�� �Ŀ�� ó�� �ʿ�

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
            // Bowl ����Ʈ���� ����
            bowl.itemsInBowl.Remove(itemObj);
            // ������Ʈ �ı�
            Destroy(itemObj);
        }
    }
}
