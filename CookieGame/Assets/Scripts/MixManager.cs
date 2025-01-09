using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MixManager : MonoBehaviour
{
    [SerializeField] private float mixingTime = 5f; // 5�� �巡�� �� ���� �Ϸ�
    private float realmixingTime = 0f;

    public List<GameObject> iteminBowl = new List<GameObject>();
    private int currentItemCount = 0;

    private bool isfineMix = false; // ��ᰡ ����� �����־� ���� ���� ������ �����Ѱ�

    private int mixCount = 0; // ���� �Ϸ� Ƚ��
    public TMP_Text WinText;
    public TMP_Text CountText;

    void Start()
    {

    }

    void Update()
    {
        if (Input.touchCount > 0) // ��ġ �Է��� 0 �̻��� ���-��� �ϳ� �̻��� ��ġ�� ������ ���
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved) // �հ��� �����̴� ��
            {
                // ���������� 2D collider ����� �� �ֵ��� ����
                Vector2 wp = Camera.main.ScreenToWorldPoint(touch.position);
                Ray2D ray = new Ray2D(wp, Vector2.zero);
                RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit2D.collider != null && hit2D.collider.CompareTag("Bowl")) // Bowl ������ ��ġ�� ������ ���
                {
                    // ��ᰡ ������� �Ǻ�
                    if (CheckBowlItem())
                    {
                        // 5�� ����
                        realmixingTime += Time.deltaTime;

                        if (realmixingTime >= mixingTime)
                        {
                            mixCount++; // ���� �Ϸ� �� count ����
                            realmixingTime = 0f; // ���� �ð� �ʱ�ȭ
                            isfineMix = false;

                            if (mixCount >= 3) // �� 3���� ���� ���� ������
                            {
                                WinText.text = "you win!"; // win text -> ���� �������� �̵�
                            }
                        }
                    }
                }
                else
                {
                    realmixingTime = 0f; // Bowl �ƴ� ���� �������� �� ��� Ÿ�̸� �ʱ�ȭ
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) //�׸� �ȿ� ��ᰡ ���Դ��� Ȯ��
    {
        if (!iteminBowl.Contains(other.gameObject))
        {
            iteminBowl.Add(other.gameObject); // ������ ������Ʈ�� list�� �߰�
            currentItemCount++; // ������ ��� ���� ���
            CountText.text = $"now: {currentItemCount}";
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (iteminBowl.Contains(other.gameObject))
        {
            iteminBowl.Remove(other.gameObject);
            currentItemCount--;
        }
    }

    private bool CheckBowlItem()
    {
        if (currentItemCount == 2) // 2���� ��� �� ��� ���� 1
        {
            return isfineMix = true;
        }
        else if (currentItemCount == 4) // 4���� ��� �� ��� ���� 2
        {
            return isfineMix = true;
        }
        else if (currentItemCount == 7) // 7���� ��� �� ��� ���� 3
        {
            return isfineMix = true;
        }
        return isfineMix;
    }
}