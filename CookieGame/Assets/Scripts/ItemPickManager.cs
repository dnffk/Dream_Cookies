using UnityEngine;

public class ItemPickManager : MonoBehaviour
{
    [Header("Mode Settings")]
    [SerializeField] private bool isDecoTime;
    [SerializeField] private MixManager MixManager; // 아이템 픽업 스크립트

    [Header("Common Raycast Settings")]
    [SerializeField] private LayerMask whatIsItem; // 아이템 찾기용 레이어마스크 (없으면 빼도 됨)

    [Header("Effect Pick (복사본) Settings")]
    [SerializeField] private GameObject ItemEffect; // "아이템이 선택되었다" 이펙트 프리팹

    private GameObject copyItem;   // 선택된 ‘원본’ 아이템 (레이캐스트로 감지)
    private GameObject dragCopy;       // 드래그용 복사본
    private GameObject copyeffect; // “선택 이펙트” 오브젝트

    [Header("Drag Prefabs")]
    [SerializeField] private GameObject eggDragPrefab;
    [SerializeField] private GameObject ButterDragPrefab;
    [SerializeField] private GameObject SugarSaltDragPrefab;
    [SerializeField] private GameObject FlourPowderDragPrefab;
    [SerializeField] private GameObject EffectDragPrefab;

    private GameObject foodItem; // 직접 드래그 중인 아이템(원본)
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

        // 선택 이펙트 생성
        if (ItemEffect != null)
        {
            copyeffect = Instantiate(ItemEffect, copyItem.transform.position, Quaternion.identity);
        }

        // 드래그용 복사본 생성
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

        // 복사본 오브젝트의 레이어를 "Ignore Raycast" (또는 다른 미사용 레이어)로 변경
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

        // 드래그 시작 시 스케일 업
        foodItem.transform.localScale = beforeScale * pickupScale;

        // 이동 오프셋 계산
        Vector2 itemPos = foodItem.transform.position;
        offset = itemPos - worldPos;
    }

    private void Moved(Vector2 worldPos)
    {
        if (foodItem != null)
        {
            // 목표 위치 = 터치/마우스 세계좌표 + offset
            Vector2 targetPos = worldPos + offset;

            // 부드럽게 이동
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
