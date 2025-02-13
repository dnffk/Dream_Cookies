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
    [SerializeField] private GameObject nextButton;    // ��� ��ȯ ��ư
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
        // ��ġ�� �ϳ� �̻� ���� ��
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            // ��ġ�� ����/�̵�/���� ������ ���� �˻� (ª�� �ǵ� Began���� �ν� ����)
            if (t.phase == TouchPhase.Began ||
                t.phase == TouchPhase.Moved ||
                t.phase == TouchPhase.Stationary)
            {
                // ȭ�� ��ġ ��ǥ�� ���� ��ǥ�� ��ȯ
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(t.position);

                // 2D Raycast�� �ش� ������ �˻�
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
                if (hit.collider != null)
                {
                    // ���� Collider�� Timer ������Ʈ���� Ȯ��
                    if (hit.collider.gameObject == Timer)
                    {
                        float currentZ = Timer.transform.eulerAngles.z;

                        currentZ += 90f;

                        if (currentZ >= 360f)
                        {
                            currentZ -= 360f;
                        }

                        // �� ���� ����
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
    {// ��Ű ���� ���� ���� ( ������ / Ÿ�̸�(?) )
     // ��ġ ��ġ�� �����尩 ����ٴϸ� ���� ��
        if (!isBakingActive) return;

        timer += Time.deltaTime; // 7�� ���� ����
        if (timer >= playTime)
        {
            isBakingActive = false; // 7�� ���� ���� ����
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
            // ª�� ��ġ�ص� �������� Began/Moved/Stationary ��� Ȯ��
            if (t.phase == TouchPhase.Began
                || t.phase == TouchPhase.Moved
                || t.phase == TouchPhase.Stationary)
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(t.position);

                // 2D Raycast
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
                if (hit.collider != null)
                {
                    // ���� ���� Oven ������Ʈ���� Ȯ��
                    if (hit.collider.gameObject == Oven)
                    {
                        // Hand ������Ʈ�� ���̰� �ϰ�, ��ġ ��ġ�� �̵�
                        Hand.SetActive(true);
                        // z���� Hand ������Ʈ�� ���� z ����
                        Hand.transform.position = new Vector3(touchPos.x, touchPos.y, Hand.transform.position.z);

                        // ������ ��ġ�� ������ ����
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public void OnOvenButtonClicked()
    {
        // ���� ��ư�� ������ ���� ������ ����
        progressSlider.gameObject.SetActive(true);
        isBakingActive = true;
    }
}