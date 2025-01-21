using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextButtonManager : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject NextButton;
    public float moveSpeed = 1.0f;

    private Vector3 targetPos; // Vector3를 사용한 이유는 Vector2를 사용하면 카메라의 z축 위치가 -10에서 0으로 고정되어서 오브젝트가 보이지 않게 되기 때문

    void Start()
    {
        targetPos = MainCamera.transform.position;
    }
    void Update()
    {
        MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    public void OnButtonClicked() // 버튼 클릭 한 번(Lerp 한 번 호출) 만으로 원하는 위치까지 선형보간으로 이동할 수 없기에 Update문을 사용해서 여러 번 누적 호출
    {
        targetPos = MainCamera.transform.position + new Vector3(20f, 0f, 0f);
        NextButton.SetActive(false);
    }
}