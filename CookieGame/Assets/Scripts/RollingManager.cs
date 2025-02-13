using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingManager : MonoBehaviour
{
    [Header("���� Transform")]
    [SerializeField] private Transform dough;        // �����ų ���� ������Ʈ  
    [SerializeField] private float scaleFactor = 0.001f; // ������ ���� ����

    [Header("���� �ִ� ũ�� / �����ܰ� �Ӱ谪")]
    [SerializeField] private float maxScaleX = 50f;   // x�� �ִ� ũ��  
    [SerializeField] private float maxScaleY = 50f;   // y�� �ִ� ũ��
    [SerializeField] private float thresholdScaleX = 50f; // x�� �����ܰ� �Ӱ谪
    [SerializeField] private float thresholdScaleY = 50f; // y�� �����ܰ� �Ӱ谪

    [Header("���� ��ũ��Ʈ & ������Ʈ")]
    [SerializeField] private Dough2D dough2d;
    [SerializeField] private CookieShapeManager CookieShape;
    public GameObject rollingPin;

    private Vector2 lastTouchPos;

    void Start()
    {
        // ��) ó������ ������ (5,5,1) ������ �۰� ���۽��Ѿ�
        // �Ѹ� ����(�巡��) �߿� Ŀ���� �� ü���� �� ����
        dough.localScale = new Vector3(5f, 5f, 1f);
    }

    void Update()
    {
        // Rolling pin�� �浹 ���� �ƴϸ� ũ�⸦ �������� ����
        if (!dough2d.isCollidedPin) return;

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                lastTouchPos = t.position;
            }
            else if (t.phase == TouchPhase.Moved)
            {
                // �̵� �Ÿ���ŭ ������ Ȯ��
                float distance = t.deltaPosition.magnitude;

                Vector2 newScale = dough.localScale;
                newScale.x += distance * scaleFactor;
                // y�� x���� ���� �� ����/���� �ø����� ����ġ ����
                newScale.y += distance * scaleFactor / 0.5f;

                // �ִ� ũ�� ����
                newScale.x = Mathf.Clamp(newScale.x, 1.3f, maxScaleX);
                newScale.y = Mathf.Clamp(newScale.y, 1.0f, maxScaleY);

                dough.localScale = newScale;

                // X, Y �� �� 15�� ������ ���� �ܰ�� (���ϴ� �Ӱ谪 ���� ����)
                if (newScale.x >= thresholdScaleX && newScale.y >= thresholdScaleY)
                {
                    rollingPin.SetActive(false);
                    CookieShape.CookieCutterShow();
                }
            }
        }
    }
}
