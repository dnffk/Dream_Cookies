using UnityEngine;

[System.Serializable]
public struct BakeStep
{
    [Tooltip("이 단계에서 표시할 안내 문구")]
    public string instruction;

    [Header("플래그들")]
    [Tooltip("문을 열어야 하는 단계인가?")]
    public bool isOpenDoorStep;

    [Tooltip("오븐 시간 다이얼을 설정해야 하는 단계인가?")]
    public bool isSetTimeDialStep;

    [Tooltip("오븐 온도 다이얼을 설정해야 하는 단계인가?")]
    public bool isSetTempDialStep;

    [Tooltip("굽기(5초간 대기) 단계인가?")]
    public bool isWaitBakeStep;

    [Tooltip("굽고 난 후 문 다시 열기 단계인가?")]
    public bool isOpenDoorAfterBakeStep;

    [Header("연결")]
    [Tooltip("이 단계를 완료했을 때 넘어갈 단계 인덱스 (steps 내)")]
    public int nextStepIndex;
}