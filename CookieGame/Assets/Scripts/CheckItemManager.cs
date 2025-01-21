using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

[SerializeField]
public class CheckItemManager : MonoBehaviour
{
    public static CheckItemManager Instance { get; private set; }
    public bool[] itemUsed = new bool[20];

    private void Awake()
    {
        Instance = this;
    }

    public void ResetItem()
    {
        for (int i = 0; i < itemUsed.Length; i++)
        {
            itemUsed[i] = false;
        }
    }

    public void UseItem(int index)
    {
        itemUsed[index] = true;
        Debug.Log(index + ":" + itemUsed[index]);
    }
}