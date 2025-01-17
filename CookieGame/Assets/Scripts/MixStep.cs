using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MixStep
{
    [Tooltip("그릇 안에 추가되어야 할 태그")]
    public List<string> requiredTags;

    [Tooltip("안내 문구")]
    public string instruction;

    [Tooltip("이 단계가 섞기 단계인지?")]
    public bool isMixStep;

    [Tooltip("섞기에 필요한 시간(초). Add단계엔 0")]
    public float mixTime;

    [Tooltip("이 단계를 완료하면 넘어갈 다음 단계의 인덱스(steps 배열 내)")]
    public int nextStepIndex;
}