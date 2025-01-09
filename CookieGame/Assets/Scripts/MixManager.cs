using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MixManager : MonoBehaviour
{
    [SerializeField] private float mixingTime = 5f; // 5초 드래그 시 섞기 완료
    private float realmixingTime = 0f;

    public List<GameObject> iteminBowl = new List<GameObject>();
    private int currentItemCount = 0;

    private bool isfineMix = false; // 재료가 충분히 들어와있어 섞기 과정 진행이 가능한가

    private int mixCount = 0; // 섞기 완료 횟수
    public TMP_Text WinText;
    public TMP_Text CountText;

    void Start()
    {

    }

    void Update()
    {
        if (Input.touchCount > 0) // 터치 입력이 0 이상인 경우-적어도 하나 이상의 터치가 감지된 경우
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved) // 손가락 움직이는 중
            {
                // 마찬가지로 2D collider 사용할 수 있도록 변경
                Vector2 wp = Camera.main.ScreenToWorldPoint(touch.position);
                Ray2D ray = new Ray2D(wp, Vector2.zero);
                RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit2D.collider != null && hit2D.collider.CompareTag("Bowl")) // Bowl 위에서 터치가 감지된 경우
                {
                    // 재료가 충분한지 판별
                    if (CheckBowlItem())
                    {
                        // 5초 누적
                        realmixingTime += Time.deltaTime;

                        if (realmixingTime >= mixingTime)
                        {
                            mixCount++; // 섞기 완료 후 count 증가
                            realmixingTime = 0f; // 섞는 시간 초기화
                            isfineMix = false;

                            if (mixCount >= 3) // 총 3번의 섞기 과정 지나면
                            {
                                WinText.text = "you win!"; // win text -> 다음 과정으로 이동
                            }
                        }
                    }
                }
                else
                {
                    realmixingTime = 0f; // Bowl 아닌 곳을 스와이프 할 경우 타이머 초기화
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) //그릇 안에 재료가 들어왔는지 확인
    {
        if (!iteminBowl.Contains(other.gameObject))
        {
            iteminBowl.Add(other.gameObject); // 감지된 오브젝트를 list에 추가
            currentItemCount++; // 감지된 재료 개수 기록
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
        if (currentItemCount == 2) // 2개의 재료 들어간 경우 섞기 1
        {
            return isfineMix = true;
        }
        else if (currentItemCount == 4) // 4개의 재료 들어간 경우 섞기 2
        {
            return isfineMix = true;
        }
        else if (currentItemCount == 7) // 7개의 재료 들어간 경우 섞기 3
        {
            return isfineMix = true;
        }
        return isfineMix;
    }
}