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

    private float RedTime = 0f;  // 0.8 ~ 1.0
    private float OrangeTime = 0f;  // 0.6 ~ 0.8
    private float greenTime = 0f;  // 0.4 ~ 0.6

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
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            // 끝났을 때 한 번만 처리
            if (t.phase == TouchPhase.Ended)
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(t.position);
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject == Timer)
                {
                    // 회전 +90
                    StartCoroutine(RotateDialCoroutine(90f, 0.3f));
                }
            }
        }
    }

    private void BakingTime()
    {// 쿠키 굽기 과정 진행 ( 게이지 / 타이머(?) )
        if (!isBakingActive) return;

        timer += Time.deltaTime;
        if (timer >= playTime)
        {
            isBakingActive = false;
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
            if (t.phase == TouchPhase.Began || t.phase == TouchPhase.Ended)
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(t.position);
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

                if (hit.collider != null)
                {
                    // 맞은 것이 Oven 오브젝트인지 확인
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
        // 이미 회전 중이라면 바로 종료
        if (isRotating) yield break;
        isRotating = true;

        float startZ = Timer.transform.eulerAngles.z;
        float targetZ = startZ + angleDelta;

        // 0~360 범위를 벗어나는 경우 보정
        if (targetZ >= 360f)
            targetZ -= 360f;
        else if (targetZ < 0f)
            targetZ += 360f;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            // Mathf.LerpAngle을 쓰면 350 -> 10도 같은 상황에서도 부드럽게 보간됩니다.
            float newZ = Mathf.LerpAngle(startZ, targetZ, t);
            Timer.transform.eulerAngles = new Vector3(0f, 0f, newZ);

            yield return null;
        }
        // 마지막 각도를 정확히 맞춰줌
        Timer.transform.eulerAngles = new Vector3(0f, 0f, targetZ);

        // 회전 끝난 후, finalAngle에 따른 playTime 적용
        float finalAngle = Timer.transform.eulerAngles.z;
        switch (Mathf.RoundToInt(finalAngle))
        {
            case 0:
                playTime = 0f;
                break;
            case 90:
                playTime = 10f;
                break;
            case 180:
                playTime = 20f;
                break;
            case 270:
                playTime = 30f;
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

        // 마지막에 딱 0도로 맞춤
        Timer.transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

}