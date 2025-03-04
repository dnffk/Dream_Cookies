using UnityEngine;

public class ItemPickManager : MonoBehaviour
{
    [Header("Mode Settings")]
    [SerializeField] private bool isDecoTime;
    [SerializeField] private MixManager MixManager; // ������ �Ⱦ� ��ũ��Ʈ

    [Header("Common Raycast Settings")]
    [SerializeField] private LayerMask whatIsItem; // ������ ã��� ���̾��ũ (������ ���� ��)

    [Header("Effect Pick (���纻) Settings")]
    [SerializeField] private GameObject ItemEffect; // "�������� ���õǾ���" ����Ʈ ������

    private GameObject copyItem;   // ���õ� �������� ������ (����ĳ��Ʈ�� ����)
    private GameObject dragCopy;       // �巡�׿� ���纻
    private GameObject copyeffect; // ������ ����Ʈ�� ������Ʈ

    [Header("Drag Prefabs")]
    [SerializeField] private GameObject eggDragPrefab;
    [SerializeField] private GameObject ButterDragPrefab;
    [SerializeField] private GameObject SugarSaltDragPrefab;
    [SerializeField] private GameObject FlourPowderDragPrefab;
    [SerializeField] private GameObject EffectDragPrefab;

    private GameObject foodItem; // ���� �巡�� ���� ������(����)
    private Vector2 offset;
    private Vector2 beforeScale;
    [Header("Item Pick Settings")]
    [SerializeField] private float pickupScale = 1.2f;
    [SerializeField] private float moveSpeed = 10f;
    
    bool UseCopyLogic => (MixManager.isMixTime || isDecoTime);

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (UseCopyLogic)
            {
                CopyBegan(mouseWorldPos);
            }
            else
            {
                Began(mouseWorldPos);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (UseCopyLogic)
            {
                CopyMoved(mouseWorldPos);
            }
            else
            {
                Moved(mouseWorldPos);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (UseCopyLogic)
            {
                CopyEnded();
            }
            else
            {
                Ended();
            }
        }

#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(touch.position);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        if (UseCopyLogic)
                            CopyBegan(worldPos);
                        else
                            Began(worldPos);
                        break;
                    }

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    {
                        if (UseCopyLogic)
                            CopyMoved(worldPos);
                        else
                            Moved(worldPos);
                        break;
                    }

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        if (UseCopyLogic)
                            CopyEnded();
                        else
                            Ended();
                        break;
                    }
            }
        }
#endif
    }

    private void CopyBegan(Vector2 worldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100f, whatIsItem);
        if (hit.collider == null) return;

        copyItem = hit.collider.gameObject;

        // ���� ����Ʈ ����
        if (ItemEffect != null)
        {
            copyeffect = Instantiate(ItemEffect, copyItem.transform.position, Quaternion.identity);
        }

        // �巡�׿� ���纻 ����
        if (copyItem.CompareTag("Egg"))
        {
            dragCopy = Instantiate(eggDragPrefab, copyItem.transform.position, Quaternion.identity);
        }
        else if (copyItem.CompareTag("Butter"))
        {
            dragCopy = Instantiate(ButterDragPrefab, copyItem.transform.position, Quaternion.identity);
        }
        else if (copyItem.CompareTag("Sugar") || copyItem.CompareTag("Salt"))
        {
            dragCopy = Instantiate(SugarSaltDragPrefab, copyItem.transform.position, Quaternion.identity);
        }
        else if (copyItem.CompareTag("Flour") || copyItem.CompareTag("Powder"))
        {
            dragCopy = Instantiate(FlourPowderDragPrefab, copyItem.transform.position, Quaternion.identity);
        }
        else if (copyItem.CompareTag("Strawberry powder")
              || copyItem.CompareTag("Choco powder")
              || copyItem.CompareTag("Green tea powder"))
        {
            dragCopy = Instantiate(EffectDragPrefab, copyItem.transform.position, Quaternion.identity);
        }

        // ���纻 ������Ʈ�� ���̾ "Ignore Raycast" (�Ǵ� �ٸ� �̻�� ���̾�)�� ����
        dragCopy.layer = LayerMask.NameToLayer("Ignore Raycast");
    }


    private void CopyMoved(Vector2 worldPos)
    {
        if (dragCopy != null)
        {
            dragCopy.transform.position = worldPos;
        }
    }

    public void CopyEnded()
    {
        if (copyeffect != null) Destroy(copyeffect);

        copyItem = null;
        dragCopy = null;
        copyeffect = null;
    }

    private void Began(Vector2 worldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100f, whatIsItem);
        if (hit.collider == null) return;

        foodItem = hit.collider.gameObject;
        beforeScale = foodItem.transform.localScale;

        // �巡�� ���� �� ������ ��
        foodItem.transform.localScale = beforeScale * pickupScale;

        // �̵� ������ ���
        Vector2 itemPos = foodItem.transform.position;
        offset = itemPos - worldPos;
    }

    private void Moved(Vector2 worldPos)
    {
        if (foodItem != null)
        {
            // ��ǥ ��ġ = ��ġ/���콺 ������ǥ + offset
            Vector2 targetPos = worldPos + offset;

            // �ε巴�� �̵�
            foodItem.transform.position = Vector2.Lerp(
                foodItem.transform.position,
                targetPos,
                Time.deltaTime * moveSpeed
            );
        }
    }

    private void Ended()
    {
        if (foodItem != null)
        {
            foodItem.transform.localScale = beforeScale;
        }

        foodItem = null;
    }

    private void Remove(GameObject copy)
    {
        var itemPick = copy.GetComponent<ItemPickManager>();
        if (itemPick) Destroy(itemPick);
    }
}
