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

    public float press = 1.0f;         // 1초 이상 눌러야 쿠키 생성
    private bool isPressing = false;
    private float pressTime = 0f;
    private int cookieCount = 0;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    isPressing = true;
                    pressTime = 0f;
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (isPressing)
                        pressTime += Time.deltaTime;
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isPressing && (Shape2D.isCircle || Shape2D.isHeart || Shape2D.isStar || Shape2D.isCookieMan))
                    {
                        if (pressTime >= press)
                        {
                            Vector2 spawnPos = Camera.main.ScreenToWorldPoint(t.position);
                            CreateCookie(spawnPos);
                        }
                    }
                    isPressing = false;
                    pressTime = 0f;
                    break;
            }
        }
    }

    private void CreateCookie(Vector2 spawnPos)
    {
        GameObject prefab = null;

        if (Shape2D.isCircle)
        {
            prefab = circleCookiePrefab;
            CheckItemManager.Instance.UseItem(ItemName.Circle);
        }
        else if (Shape2D.isHeart)
        {
            prefab = heartCookiePrefab;
            CheckItemManager.Instance.UseItem(ItemName.Heart);
        }
        else if (Shape2D.isStar)
        {
            prefab = starCookiePrefab;
            CheckItemManager.Instance.UseItem(ItemName.Star);
        }
        else if (Shape2D.isCookieMan)
        {
            prefab = cookieManPrefab;
            CheckItemManager.Instance.UseItem(ItemName.CookieMan);
        }

        if (prefab != null)
        {
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }

        // 선택된 모양 초기화
        Shape2D.isCircle = false;
        Shape2D.isHeart = false;
        Shape2D.isStar = false;
        Shape2D.isCookieMan = false;

        cookieCount++;
        if (cookieCount >= 4)
        {
            nextButton.SetActive(true);
            itemPick.gameObject.SetActive(false);
        }
    }

    // 쿠키틀들을 보여주는 함수
    public void CookieCutterShow()
    {
        Circle.SetActive(true);
        Star.SetActive(true);
        Heart.SetActive(true);
        CookieMan.SetActive(true);
    }
}