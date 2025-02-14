using UnityEngine;

public class CookieShapeManager : MonoBehaviour
{
    [SerializeField] public Shape2D Shape2D;
    [SerializeField] private GameObject circleCookiePrefab;
    [SerializeField] private GameObject heartCookiePrefab;
    [SerializeField] private GameObject starCookiePrefab;
    [SerializeField] private GameObject cookieManPrefab;
    [SerializeField] private ItemPickManager itemPick;

    [SerializeField] private GameObject Circle;
    [SerializeField] private GameObject Heart;
    [SerializeField] private GameObject Star;
    [SerializeField] private GameObject CookieMan;

    public GameObject nextButton;

    public float press = 1.0f;
    private bool isPressing = false;
    private float pressTime = 0f;
    private int cookieCount = 0;

    private Vector2 touchStartPos;

    void Update()
    {
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
                touchStartPos = t.position;
                isPressing = false;
                pressTime = 0f;
            }
        }

        // 아무 모양이나 선택되어 있을 때만 누른 시간 체크
        if (isPressing && (Shape2D.isCircle || Shape2D.isStar || Shape2D.isHeart || Shape2D.isCookieMan))
        {
            pressTime += Time.deltaTime;
            if (pressTime >= press)
            {
                Vector2 spawnPos = Camera.main.ScreenToWorldPoint(new Vector2(touchStartPos.x, touchStartPos.y));

                CreateCookie(spawnPos);
                pressTime = 0f;
                isPressing = false;
            }
        }
    }

    // 생성 위치를 파라미터로 받도록 수정
    private void CreateCookie(Vector2 spawnPos)
    {
        GameObject prefab = null;

        if (Shape2D.isCircle)
        {
            prefab = circleCookiePrefab;
        }
        else if (Shape2D.isHeart)
        {
            prefab = heartCookiePrefab;
        }
        else if (Shape2D.isStar)
        {
            prefab = starCookiePrefab;
        }
        else if (Shape2D.isCookieMan)
        {
            prefab = cookieManPrefab;
        }

        if (prefab != null)
        {
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }

        // 모양 선택 해제
        Shape2D.isCircle = false;
        Shape2D.isHeart = false;
        Shape2D.isStar = false;
        Shape2D.isCookieMan = false;

        cookieCount++;
        if (cookieCount >= 1)
        {
            nextButton.SetActive(true);
            itemPick.gameObject.SetActive(false);
        }
    }

    public void CookieCutterShow()
    {
        Circle.SetActive(true);
        Star.SetActive(true);
        Heart.SetActive(true);
        CookieMan.SetActive(true);
    }
}
