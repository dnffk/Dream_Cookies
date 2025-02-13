using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Xml.Serialization;

public class BakingManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text requestText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject Oven;
    [SerializeField] private GameObject Timer;
    [SerializeField] private GameObject nextButton;    // 장면 전환 버튼
    [SerializeField] private GameObject OvenButton;
    [SerializeField] private GameObject Hand;

    [Header("Handle Gauge Settings")]
    [SerializeField] private float handleDecreaseSpeed = 2.0f;
    [SerializeField] private float handleIncreaseAmount = 3.0f;
    [SerializeField] private float playTime = 0f;
    [SerializeField] private GameObject fire1;
    [SerializeField] private GameObject fire2;
    [SerializeField] private GameObject fire3;

    [SerializeField] private Color redColor = Color.red;
    [SerializeField] private Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

    private float RedTime = 0f;  // 0.8 ~ 1.0
    private float OrangeTime = 0f;  // 0.6 ~ 0.8
    private float greenTime = 0f;  // 0.4 ~ 0.6

    private float handleValue = 1f;
    private float timer = 0f;
    private bool isBakingActive = false;
    private Renderer ovenRenderer;

    void Start()
    {
        progressSlider.gameObject.SetActive(false);
        OvenButton.gameObject.SetActive(false);
        progressSlider.minValue = 0f;
        progressSlider.maxValue = 1.0f;
        progressSlider.value = handleValue;

        fire1.SetActive(false);
        fire2.SetActive(false);
        fire3.SetActive(false);

        ovenRenderer = Oven.GetComponent<Renderer>();
    }

    void Update()
    {
        IsTouchingTimer();
        if (isBakingActive)
        {
            BakingTime();
        }
    }

    private void IsTouchingTimer()
    {
        // 터치가 하나 이상 있을 때
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            // 터치가 시작/이동/유지 상태일 때만 검사 (짧은 탭도 Began에서 인식 가능)
            if (t.phase == TouchPhase.Began ||
                t.phase == TouchPhase.Moved ||
                t.phase == TouchPhase.Stationary)
            {
                // 화면 터치 좌표를 월드 좌표로 변환
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(t.position);

                // 2D Raycast로 해당 지점을 검사
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
                if (hit.collider != null)
                {
                    // 닿은 Collider가 Timer 오브젝트인지 확인
                    if (hit.collider.gameObject == Timer)
                    {
                        float currentZ = Timer.transform.eulerAngles.z;

                        currentZ += 90f;

                        if (currentZ >= 360f)
                        {
                            currentZ -= 360f;
                        }

                        // 새 각도 적용
                        Timer.transform.eulerAngles = new Vector3(0f, 0f, currentZ);

                        switch (Mathf.RoundToInt(currentZ))
                        {
                            case 0:
                                playTime = 5f;
                                break;
                            case 90:
                                playTime = 15f;
                                break;
                            case 180:
                                playTime = 30f;
                                break;
                            case 270:
                                playTime = 45f;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }

    private void BakingTime()
    {// 쿠키 굽기 과정 진행 ( 게이지 / 타이머(?) )
     // 터치 위치에 오븐장갑 따라다니면 좋을 듯
        if (!isBakingActive) return;

        timer += Time.deltaTime; // 7초 동안 진행
        if (timer >= playTime)
        {
            isBakingActive = false; // 7초 이후 게임 종료
            CheckZoneTime();
            return;
        }

        if (IsTouchingOven())
        {
            handleValue += handleIncreaseAmount * Time.deltaTime;
            handleValue = Mathf.Clamp(handleValue, 0f, 1.0f);
        }
        else
        {
            handleValue -= handleDecreaseSpeed * Time.deltaTime;
            handleValue = Mathf.Clamp(handleValue, 0f, 1.0f);
        }

        AccumulateZoneTime(handleValue, Time.deltaTime);   // 현재 게이지 값에 따라 해당 구간 시간 누적

        progressSlider.value = handleValue; // Slider UI 업데이트
    }

    private void AccumulateZoneTime(float value, float deltaTime)
    {
        if ((value >= 0.85f && value <= 1.0f) || (value >= 0.0f && value < 0.3f))
        {
            ovenRenderer.material.color = redColor;
            fire1.SetActive(true);
            fire2.SetActive(true);
            fire3.SetActive(true);
            RedTime += deltaTime;
        }
        else if ((value >= 0.7f && value < 0.85f) || (value >= 0.3f && value < 0.5f))
        {
            ovenRenderer.material.color = orangeColor;
            fire1.SetActive(true);
            fire2.SetActive(false);
            fire3.SetActive(false);
            OrangeTime += deltaTime;
        }
        else if (value >= 0.5f && value < 0.7f)
        {
            ovenRenderer.material.color = Color.white;
            fire1.SetActive(false);
            fire2.SetActive(false);
            fire3.SetActive(false);
            greenTime += deltaTime;
        }
    }

    private void CheckZoneTime()
    {
        float maxTime = Mathf.Max(RedTime, OrangeTime, greenTime);

        string resultZone;
        if (Mathf.Approximately(maxTime, RedTime))
            resultZone = "빨간 구간";
        else if (Mathf.Approximately(maxTime, OrangeTime))
            resultZone = "주황 구간";
        else
            resultZone = "초록 구간";

        if (resultZone == "초록 구간")
        {
            CheckItemManager.Instance.UseItem(ItemName.normal);
            Debug.Log("쿠키가 잘 익었따");
            // 잘 구워진 쿠키 애니메이션
        }
        else if(resultZone == "빨간 구간")
        {
            CheckItemManager.Instance.UseItem(ItemName.burn);
            Debug.Log("쿠키가 탔다");
            // 타버린 쿠키 애니메이션
        }
        else
        {
            CheckItemManager.Instance.UseItem(ItemName.less);
            Debug.Log("쿠키가 덜 익었다");
            // 덜 익은 쿠키 애니메이션
        }

        nextButton.SetActive(true);
    }
    private bool IsTouchingOven()
    {
        Hand.SetActive(false);

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            // 짧게 터치해도 잡히도록 Began/Moved/Stationary 모두 확인
            if (t.phase == TouchPhase.Began
                || t.phase == TouchPhase.Moved
                || t.phase == TouchPhase.Stationary)
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(t.position);

                // 2D Raycast
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
                if (hit.collider != null)
                {
                    // 맞은 것이 Oven 오브젝트인지 확인
                    if (hit.collider.gameObject == Oven)
                    {
                        // Hand 오브젝트를 보이게 하고, 터치 위치로 이동
                        Hand.SetActive(true);
                        // z값은 Hand 오브젝트의 기존 z 유지
                        Hand.transform.position = new Vector3(touchPos.x, touchPos.y, Hand.transform.position.z);

                        // 오븐을 터치한 것으로 간주
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public void OnOvenButtonClicked()
    {
        // 오븐 버튼을 누르면 굽기 과정을 시작
        progressSlider.gameObject.SetActive(true);
        isBakingActive = true;
    }
}