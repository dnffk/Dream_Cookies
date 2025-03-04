using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Collections;

public class BakingManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject Oven;
    [SerializeField] private GameObject Timer;
    [SerializeField] private GameObject nextButton;    // 장면 전환 버튼
    [SerializeField] private GameObject OvenButton;
    [SerializeField] private GameObject Hand;

    [Header("Handle Gauge Settings")]
    [SerializeField] private float handleDecreaseSpeed = 0.2f;
    [SerializeField] private float handleIncreaseAmount = 0.3f;
    [SerializeField] private float playTime = 0f;
    [SerializeField] private GameObject fire1;
    [SerializeField] private GameObject fire2;
    [SerializeField] private GameObject fire3;

    [SerializeField] private Color redColor = Color.red;
    [SerializeField] private Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

    // 구간별 시간 측정
    private float RedTime = 0f;     // 빨간 구간(혹은 매우 낮은 구간) 누적
    private float OrangeTime = 0f;  // 주황 구간 누적
    private float greenTime = 0f;   // 초록 구간 누적
    private float yellowTime = 0f;

    private float handleValue = 1f;
    private float timer = 0f;
    private bool isBakingActive = false;
    private bool isRotating = false;  // 중복 회전 방지용
    private Renderer ovenRenderer;

    void Start()
    {
        progressSlider.minValue = 0f;
        progressSlider.maxValue = 1.0f;
        progressSlider.value = handleValue;

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
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Ended)
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(t.position);
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject == Timer)
                {
                    StartCoroutine(RotateDialCoroutine(90f, 0.3f));
                }
            }
        }
    }

    private void BakingTime()
    {
        // 쿠키 굽기 과정 진행 (게이지 / 타이머)
        if (!isBakingActive) return;

        timer += Time.deltaTime;
        if (timer >= playTime)
        {
            // 베이킹이 끝나면 해당 구간에서 가장 오래 머문 구간 판단
            isBakingActive = false;
            CheckZoneTime();
            return;
        }

        // 오븐을 터치 중이면 게이지 증가, 아니면 감소
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

        // 현재 게이지 값에 따라 해당 구간 시간 누적
        AccumulateZoneTime(handleValue, Time.deltaTime);

        // 슬라이더 UI 업데이트
        progressSlider.value = handleValue;
    }

    private void AccumulateZoneTime(float value, float deltaTime)
    {
        // 빨간색 구간
        if (value >= 0.82f && value <= 1.0f)
        {
            ovenRenderer.material.color = redColor; 
            fire1.SetActive(true);
            fire2.SetActive(true);
            fire3.SetActive(true);
            RedTime += deltaTime;
        }
        // 주황색 구간
        else if (value >= 0.6f && value < 0.82f)
        {
            ovenRenderer.material.color = orangeColor;
            fire1.SetActive(true);
            fire2.SetActive(true);
            fire3.SetActive(false);
            OrangeTime += deltaTime;
        }
        // 노랑 구간
        else if (value >= 0.3f && value < 0.6f)
        {
            ovenRenderer.material.color = Color.white;
            fire1.SetActive(false);
            fire2.SetActive(false);
            fire3.SetActive(true);
            yellowTime += deltaTime;
        }
        // 초록 구간
        else if (value >= 0f && value < 0.3f)
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

        // 어떤 구간에서 가장 오래 머물렀는지 판별
        string resultZone;
        if (Mathf.Approximately(maxTime, RedTime))
            resultZone = "빨간 구간";
        else if (Mathf.Approximately(maxTime, OrangeTime))
            resultZone = "주황 구간";
        else if (Mathf.Approximately(maxTime, greenTime))
            resultZone = "초록 구간";
        else
            resultZone = "노란 구간";

        if (Mathf.Approximately(playTime, 6f))
        {
            if (resultZone == "주황 구간")
            {
                CheckItemManager.Instance.UseItem(ItemName.normal);
            }
            else if(resultZone == "빨간 구간")
            {
                CheckItemManager.Instance.UseItem(ItemName.burn);
            }
            else
            {
                CheckItemManager.Instance.UseItem(ItemName.less);
            }
        }
        else if (Mathf.Approximately(playTime, 8f))
        {
            if (resultZone == "노란 구간")
            {
                CheckItemManager.Instance.UseItem(ItemName.normal);
            }
            else if (resultZone == "빨간 구간" || resultZone == "주황 구간")
            {
                CheckItemManager.Instance.UseItem(ItemName.burn);
            }
            else
            {
                CheckItemManager.Instance.UseItem(ItemName.less);
            }
        }
        else if (Mathf.Approximately(playTime, 10f))
        {
            if (resultZone == "초록 구간")
            {
                CheckItemManager.Instance.UseItem(ItemName.normal);
            }
            else
            {
                CheckItemManager.Instance.UseItem(ItemName.burn);
            }
        }

        // 다음 단계 버튼 활성화
        nextButton.SetActive(true);
    }

    private bool IsTouchingOven()
    {
        Hand.SetActive(false);

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began || t.phase == TouchPhase.Ended)
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(t.position);
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

                if (hit.collider != null)
                {
                    // 터치 대상이 오븐인지 확인
                    if (hit.collider.gameObject == Oven)
                    {
                        Hand.SetActive(true);
                        Hand.transform.position = new Vector2(touchPos.x, touchPos.y);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    IEnumerator RotateDialCoroutine(float angleDelta, float duration)
    {
        if (isRotating) yield break;
        isRotating = true;

        float startZ = Timer.transform.eulerAngles.z;
        float targetZ = startZ + angleDelta;

        // 각도 보정
        if (targetZ >= 360f) targetZ -= 360f;
        else if (targetZ < 0f) targetZ += 360f;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float newZ = Mathf.LerpAngle(startZ, targetZ, t);
            Timer.transform.eulerAngles = new Vector3(0f, 0f, newZ);

            yield return null;
        }
        Timer.transform.eulerAngles = new Vector3(0f, 0f, targetZ);

        float finalAngle = Timer.transform.eulerAngles.z;
        switch (Mathf.RoundToInt(finalAngle))
        {
            case 0:
                playTime = 0f;
                break;
            case 90:
                playTime = 6f;
                break;
            case 180:
                playTime = 8f;
                break;
            case 270:
                playTime = 10f;
                break;
        }

        isRotating = false;
    }

    public void OnOvenButtonClicked()
    {
        if (playTime <= 0f)
        {
            return;
        }

        // 베이킹 시작
        isBakingActive = true;
        timer = 0f;

        StartCoroutine(RotateDialBackCoroutine(playTime));
    }

    private IEnumerator RotateDialBackCoroutine(float duration)
    {
        float startZ = Timer.transform.eulerAngles.z;
        float endZ = 0f;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float newZ = Mathf.LerpAngle(startZ, endZ, t);
            Timer.transform.eulerAngles = new Vector3(0f, 0f, newZ);

            yield return null;
        }
        // 마지막에 정확히 0도로 맞춤
        Timer.transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }
}
