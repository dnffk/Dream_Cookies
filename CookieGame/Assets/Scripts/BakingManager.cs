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
    [SerializeField] private GameObject nextButton;    // ��� ��ȯ ��ư
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
    private bool isRotating = false;  // �ߺ� ȸ�� ������
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

            // ������ �� �� ���� ó��
            if (t.phase == TouchPhase.Ended)
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(t.position);
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject == Timer)
                {
                    // ȸ�� +90
                    StartCoroutine(RotateDialCoroutine(90f, 0.3f));
                }
            }
        }
    }

    private void BakingTime()
    {// ��Ű ���� ���� ���� ( ������ / Ÿ�̸�(?) )
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

        AccumulateZoneTime(handleValue, Time.deltaTime);   // ���� ������ ���� ���� �ش� ���� �ð� ����

        progressSlider.value = handleValue; // Slider UI ������Ʈ
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
            resultZone = "���� ����";
        else if (Mathf.Approximately(maxTime, OrangeTime))
            resultZone = "��Ȳ ����";
        else
            resultZone = "�ʷ� ����";

        if (resultZone == "�ʷ� ����")
        {
            CheckItemManager.Instance.UseItem(ItemName.normal);
            Debug.Log("��Ű�� �� �;���");
            // �� ������ ��Ű �ִϸ��̼�
        }
        else if(resultZone == "���� ����")
        {
            CheckItemManager.Instance.UseItem(ItemName.burn);
            Debug.Log("��Ű�� ����");
            // Ÿ���� ��Ű �ִϸ��̼�
        }
        else
        {
            CheckItemManager.Instance.UseItem(ItemName.less);
            Debug.Log("��Ű�� �� �;���");
            // �� ���� ��Ű �ִϸ��̼�
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
                    // ���� ���� Oven ������Ʈ���� Ȯ��
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
        // �̹� ȸ�� ���̶�� �ٷ� ����
        if (isRotating) yield break;
        isRotating = true;

        float startZ = Timer.transform.eulerAngles.z;
        float targetZ = startZ + angleDelta;

        // 0~360 ������ ����� ��� ����
        if (targetZ >= 360f)
            targetZ -= 360f;
        else if (targetZ < 0f)
            targetZ += 360f;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            // Mathf.LerpAngle�� ���� 350 -> 10�� ���� ��Ȳ������ �ε巴�� �����˴ϴ�.
            float newZ = Mathf.LerpAngle(startZ, targetZ, t);
            Timer.transform.eulerAngles = new Vector3(0f, 0f, newZ);

            yield return null;
        }
        // ������ ������ ��Ȯ�� ������
        Timer.transform.eulerAngles = new Vector3(0f, 0f, targetZ);

        // ȸ�� ���� ��, finalAngle�� ���� playTime ����
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

        // ����ŷ ����
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

        // �������� �� 0���� ����
        Timer.transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

}