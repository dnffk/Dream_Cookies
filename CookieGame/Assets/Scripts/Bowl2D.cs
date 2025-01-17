using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl2D : MonoBehaviour
{
    // Bowl 내부 아이템 저장하는 list
    public List<GameObject> itemsInBowl = new List<GameObject>();
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!itemsInBowl.Contains(other.gameObject)) // 감지된 item이 list에 추가되지 않은 item일 경우
        {
            itemsInBowl.Add(other.gameObject);
        }
    }

    // 특정 태그를 가진 아이템이 Bowl 안에 있는지?
    public bool HasItem(string tagName)
    {
        foreach (var item in itemsInBowl) // itemInBowl list내에 있는 아이템들 중
        {
            if (item != null && item.CompareTag(tagName)) // item이 null이 아닐 때, 아이템이 tagName의 tag를 가진 경우
            {
                return true; // true 반환
            }
        }
        return false;
    }

    public List<GameObject> GetAllItemsByTags(params string[] tags)
    {
        List<GameObject> result = new List<GameObject>();
        foreach (var item in itemsInBowl)
        {
            if (item == null) continue;

            // 여러 태그 중 하나라도 매칭되면 추가
            foreach (var t in tags)
            {
                if (item.CompareTag(t))
                {
                    result.Add(item);
                    break; // 이미 추가했으므로 다음 태그는 무시
                }
            }
        }
        return result;
    }
}