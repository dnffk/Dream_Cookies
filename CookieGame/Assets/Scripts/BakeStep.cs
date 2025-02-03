using UnityEngine;

[System.Serializable]
public struct BakeStep
{
    [Tooltip("�� �ܰ迡�� ǥ���� �ȳ� ����")]
    public string instruction;

    [Header("�÷��׵�")]
    [Tooltip("���� ����� �ϴ� �ܰ��ΰ�?")]
    public bool isOpenDoorStep;

    [Tooltip("��Ű�� �ִ� �ܰ��ΰ�? (��Ű ������Ʈ/�ִϸ��̼�)")]
    public bool isInsertCookieStep;

    [Tooltip("���� �ð� ���̾��� �����ؾ� �ϴ� �ܰ��ΰ�?")]
    public bool isSetTimeDialStep;

    [Tooltip("���� �µ� ���̾��� �����ؾ� �ϴ� �ܰ��ΰ�?")]
    public bool isSetTempDialStep;

    [Tooltip("����(5�ʰ� ���) �ܰ��ΰ�?")]
    public bool isWaitBakeStep;

    [Tooltip("���� �� �� �� �ٽ� ���� �ܰ��ΰ�?")]
    public bool isOpenDoorAfterBakeStep;

    [Tooltip("��Ű�� ������ �ܰ��ΰ�?")]
    public bool isRemoveCookieStep;

    [Header("����")]
    [Tooltip("�� �ܰ踦 �Ϸ����� �� �Ѿ �ܰ� �ε��� (steps ��)")]
    public int nextStepIndex;
}