using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingManager : MonoBehaviour
{
    [Header("���� Transform")]
    [SerializeField] private Transform dough;
    [SerializeField] private float scaleFactor = 0.001f;

    [Header("���� �ִ� ũ�� / �����ܰ� �Ӱ谪")]
    [SerializeField] private float maxScaleX = 35f;
    [SerializeField] private float maxScaleY = 20f;
    [SerializeField] private float thresholdScaleX = 35f;
    [SerializeField] private float thresholdScaleY = 20f;

    [Header("���� ��ũ��Ʈ & ������Ʈ")]
    [SerializeField] private Dough2D dough2d;
    [SerializeField] private CookieShapeManager CookieShape;
    public GameObject rollingPin;

    private Vector2 initialScale;

    private float totalDragDistance = 0f;

    void Start()
    {
        initialScale = dough.localScale;
    }

    void Update()
    {
        if (!dough2d.isCollidedPin) return;

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Moved)
            {
                // �� ������ �̵� �Ÿ��� ����
                float distance = t.deltaPosition.magnitude;
                totalDragDistance += distance;

                // ���� �Ÿ��� ���� �������� ���
                float newX = initialScale.x + totalDragDistance * scaleFactor;
                float newY = initialScale.y + totalDragDistance * (scaleFactor / 0.5f);

                // �ִ� ũ�� ����
                newX = Mathf.Clamp(newX, 1.3f, maxScaleX);
                newY = Mathf.Clamp(newY, 1.0f, maxScaleY);

                dough.localScale = new Vector2(newX, newY);

                // �Ӱ谪 üũ
                if (newX >= thresholdScaleX && newY >= thresholdScaleY)
                {
                    rollingPin.SetActive(false);
                    CookieShape.CookieCutterShow();
                }
            }
        }
    }
}
