using UnityEngine;

public class CookieShapeManager : MonoBehaviour
{
    [SerializeField] public Shape2D Shape2D;
    [SerializeField] private GameObject circleCookiePrefab;
    [SerializeField] private GameObject heartCookiePrefab;
    [SerializeField] private GameObject starCookiePrefab;
    [SerializeField] private GameObject cookieManPrefab;
    public GameObject nextButton;

    public float press = 1.0f; // 1�� �̻� ������ ��Ű ����
    private bool isPressing = false;    // ���� ������ �ִ� ������
    private float pressTime = 0f;       // ���� �ð� ����
    private int cookieCount = 0;

    void Update()
    {
        // ��ġ ���� ����
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                isPressing = true;
                pressTime = 0f;
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                isPressing = false;
                pressTime = 0f;
            }
        }

        if (isPressing && Shape2D.isCircle || Shape2D.isStar || Shape2D.isHeart || Shape2D.isCookieMan)
        {
            pressTime += Time.deltaTime;
            if (pressTime >= press)
            {
                CreateCookie();

                pressTime = 0f;
                isPressing = false;
            }
        }
    }

    private void CreateCookie()
    {
        if (Shape2D.isCircle)
        {
            Instantiate(circleCookiePrefab, transform.position, Quaternion.identity);
        }
        else if (Shape2D.isHeart)
        {
            Instantiate(heartCookiePrefab, transform.position, Quaternion.identity);
        }
        else if (Shape2D.isStar)
        {
            Instantiate(starCookiePrefab, transform.position, Quaternion.identity);
        }
        else if (Shape2D.isCookieMan)
        {
            Instantiate(cookieManPrefab, transform.position, Quaternion.identity);
        }

        Shape2D.isCircle = false;
        Shape2D.isHeart = false;
        Shape2D.isStar = false;
        Shape2D.isCookieMan = false;

        cookieCount++;

        if(cookieCount >= 4)
        {
            nextButton.SetActive(true);
        }
    }
}