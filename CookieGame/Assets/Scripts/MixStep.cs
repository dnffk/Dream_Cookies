using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MixStep
{
    [Tooltip("�׸� �ȿ� �߰��Ǿ�� �� �±�")]
    public List<string> requiredTags;

    [Tooltip("�ȳ� ����")]
    public string instruction;

    [Tooltip("�� �ܰ谡 ���� �ܰ�����?")]
    public bool isMixStep;

    [Tooltip("���⿡ �ʿ��� �ð�(��). Add�ܰ迣 0")]
    public float mixTime;

    [Tooltip("�� �ܰ踦 �Ϸ��ϸ� �Ѿ ���� �ܰ��� �ε���(steps �迭 ��)")]
    public int nextStepIndex;
}