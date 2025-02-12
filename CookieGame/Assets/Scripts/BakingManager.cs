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
    [SerializeField] private GameObject nextButton;    // ��� ��ȯ ��ư

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
        //ó���� ���쿡 ��Ű �ִ� �ִϸ��̼� ������
        BakingTime();
        progressSlider.gameObject.SetActive(true);
    }

    private void BakingTime()
    {// ��Ű ���� ���� ���� ( ������ / Ÿ�̸�(?) )
        // ��ġ ��ġ�� �����尩 ����ٴϸ� ���� ��

        if (!isBakingActive) return; // �� �̻� ������� ����

        //7 �� Ÿ�̸� ����
        timer += Time.deltaTime;
        if (timer >= playTime)
        {
            // 7�ʰ� �������Ƿ� ���� ���� �� ��� �Ǵ�
            isBakingActive = false;
            JudgeZoneTime();
            return;
        }

        // ������ �ڵ� ��������
        handleValue -= handleDecreaseSpeed * Time.deltaTime;
        handleValue = Mathf.Clamp(handleValue, 0f, 1.0f);

        // ��ġ �� ������ �ö󰡱�
        if (Input.touchCount > 0)
        {
            handleValue += handleIncreaseAmount * Time.deltaTime;
            handleValue = Mathf.Clamp(handleValue, 0f, 1.0f);
        }

        // ���� ������ ���� ���� �ش� ���� �ð� ����
        AccumulateZoneTime(handleValue, Time.deltaTime);

        // Slider UI ������Ʈ
        progressSlider.value = handleValue;
    }

    private void AccumulateZoneTime(float value, float deltaTime)
    {
        if ((value >= 0.85f && value <= 1.0f) || (value >= 0.0f && value < 0.3f))
        {// ���� ���� �ҵ���
            // ���� ������ ���� ��
            ovenRenderer.material.color = redColor;
            fire1.SetActive(true);
            fire2.SetActive(true);
            fire3.SetActive(true);
            RedTime += deltaTime;
        }
        else if ((value >= 0.7f && value < 0.85f) || (value >= 0.3f && value < 0.5f))
        {// ��Ȳ ���� ��¦ �ҵ���
            // ���� ������ ��¦ ���� ��
            ovenRenderer.material.color = orangeColor;
            fire1.SetActive(true);
            fire2.SetActive(false);
            fire3.SetActive(false);
            OrangeTime += deltaTime;
        }
        else if (value >= 0.5f && value < 0.7f)
        {// ���� ���� ���� ����
            ovenRenderer.material.color = Color.white;
            fire1.SetActive(false);
            fire2.SetActive(false);
            fire3.SetActive(false);
            greenTime += deltaTime;
        }
    }

    private void JudgeZoneTime()
    {
        // �ð� Ȯ��
        float maxTime = Mathf.Max(RedTime, OrangeTime, greenTime);

        // ���� ���� �ӹ� ���� ã��
        string resultZone;
        if (Mathf.Approximately(maxTime, RedTime))
            resultZone = "���� ����";
        else if (Mathf.Approximately(maxTime, OrangeTime))
            resultZone = "��Ȳ ����";
        else
            resultZone = "�ʷ� ����";

        if (resultZone == "�ʷ� ����")
        {
            Debug.Log("��Ű�� �����ϴ�...");
            // �� ������ ��Ű �ִϸ��̼�
        }
        else if(resultZone == "���� ����")
        {
            Debug.Log("��Ű�� �����ϴ�...");
            // Ÿ���� ��Ű �ִϸ��̼�
        }
        else
        {
            Debug.Log("��Ű�� �� �;����ϴ�...");
            // �� ���� ��Ű �ִϸ��̼�
        }

        nextButton.SetActive(true);
    }
}