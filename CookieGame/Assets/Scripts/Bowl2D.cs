using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl2D : MonoBehaviour
{
    // Bowl 내부 아이템
    private List<GameObject> itemsInBowl = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        // foodItem 태그라고 가정, 또는 여러 태그를 다 처리할 수도 있음
        if (!itemsInBowl.Contains(other.gameObject))
        {
            itemsInBowl.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (itemsInBowl.Contains(other.gameObject))
        {
            itemsInBowl.Remove(other.gameObject);
        }
    }

    /// 특정 태그를 가진 아이템이 Bowl 안에 있는지 여부
    public bool HasItem(string tagName)
    {
        foreach (var item in itemsInBowl)
        {
            if (item.CompareTag(tagName))
            {
                return true;
            }
        }
        return false;
    }
}