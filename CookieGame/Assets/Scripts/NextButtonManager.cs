using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextButtonManager : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject NextButton;
    public float moveSpeed = 1.0f;

    private Vector3 targetPos; // Vector3�� ����� ������ Vector2�� ����ϸ� ī�޶��� z�� ��ġ�� -10���� 0���� �����Ǿ ������Ʈ�� ������ �ʰ� �Ǳ� ����

    void Start()
    {
        targetPos = MainCamera.transform.position;
    }
    void Update()
    {
        MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    public void OnButtonClicked() // ��ư Ŭ�� �� ��(Lerp �� �� ȣ��) ������ ���ϴ� ��ġ���� ������������ �̵��� �� ���⿡ Update���� ����ؼ� ���� �� ���� ȣ��
    {
        targetPos = MainCamera.transform.position + new Vector3(20f, 0f, 0f);
        NextButton.SetActive(false);
    }
}