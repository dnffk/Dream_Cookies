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
    [SerializeField] private GameObject nextButton;    // 장면 전환 버튼

    [Header("Handle Gauge Settings")]
    [SerializeField] private float handleDecreaseSpeed = 2.0f;
    [SerializeField] private float handleIncreaseAmount = 0.1f;
    [SerializeField] private float playTime = 7f;
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
    private bool isBakingActive = true;
    private Renderer ovenRenderer;

    void Start()
    {
        //progressSlider.gameObject.SetActive(false);
        progressSlider.minValue = 0f;
        progressSlider.maxValue = 1.0f;
        progressSlider.value = handleValue;

        ovenRenderer = Oven.GetComponent<Renderer>();
    }

    void Update()
    {
        BakingTime();
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(t.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {

                }
            }
        }
    }

    private void OvenRunning()
    {
        //처음에 오븐에 쿠키 넣는 애니메이션 보여쥼
        BakingTime();
        progressSlider.gameObject.SetActive(true);
    }

    private void BakingTime()
    {// 쿠키 굽기 과정 진행 ( 게이지 / 타이머(?) )
        // 터치 위치에 오븐장갑 따라다니면 좋을 듯

        if (!isBakingActive) return; // 더 이상 기록하지 않음

        //7 초 타이머 증가
        timer += Time.deltaTime;
        if (timer >= playTime)
        {
            // 7초가 지났으므로 게임 종료 후 결과 판단
            isBakingActive = false;
            JudgeZoneTime();
            return;
        }

        // 게이지 자동 내려가기
        handleValue -= handleDecreaseSpeed * Time.deltaTime;
        handleValue = Mathf.Clamp(handleValue, 0f, 1.0f);

        // 터치 시 게이지 올라가기
        if (Input.touchCount > 0)
        {
            handleValue += handleIncreaseAmount * Time.deltaTime;
            handleValue = Mathf.Clamp(handleValue, 0f, 1.0f);
        }

        // 현재 게이지 값에 따라 해당 구간 시간 누적
        AccumulateZoneTime(handleValue, Time.deltaTime);

        // Slider UI 업데이트
        progressSlider.value = handleValue;
    }

    private void AccumulateZoneTime(float value, float deltaTime)
    {
        if ((value >= 0.85f && value <= 1.0f) || (value >= 0.0f && value < 0.3f))
        {// 빨간 구간 불덩이
            // 오븐 색상이 붉은 색
            ovenRenderer.material.color = redColor;
            fire1.SetActive(true);
            fire2.SetActive(true);
            fire3.SetActive(true);
            RedTime += deltaTime;
        }
        else if ((value >= 0.7f && value < 0.85f) || (value >= 0.3f && value < 0.5f))
        {// 주황 구간 살짝 불덩이
            // 오븐 색상이 살짝 붉은 색
            ovenRenderer.material.color = orangeColor;
            fire1.SetActive(true);
            fire2.SetActive(false);
            fire3.SetActive(false);
            OrangeTime += deltaTime;
        }
        else if (value >= 0.5f && value < 0.7f)
        {// 오븐 색상 변경 없음
            ovenRenderer.material.color = Color.white;
            fire1.SetActive(false);
            fire2.SetActive(false);
            fire3.SetActive(false);
            greenTime += deltaTime;
        }
    }

    private void JudgeZoneTime()
    {
        // 시간 확인
        float maxTime = Mathf.Max(RedTime, OrangeTime, greenTime);

        // 가장 많이 머문 구간 찾기
        string resultZone;
        if (Mathf.Approximately(maxTime, RedTime))
            resultZone = "빨간 구간";
        else if (Mathf.Approximately(maxTime, OrangeTime))
            resultZone = "주황 구간";
        else
            resultZone = "초록 구간";

        if (resultZone == "초록 구간")
        {
            Debug.Log("쿠키가 탔습니다...");
            // 잘 구워진 쿠키 애니메이션
        }
        else if(resultZone == "빨간 구간")
        {
            Debug.Log("쿠키가 탔습니다...");
            // 타버린 쿠키 애니메이션
        }
        else
        {
            Debug.Log("쿠키가 덜 익었습니다...");
            // 덜 익은 쿠키 애니메이션
        }

        nextButton.SetActive(true);
    }
}