using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl2D : MonoBehaviour
{
    // Bowl ���� ������ �����ϴ� list
    public List<GameObject> itemsInBowl = new List<GameObject>();
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!itemsInBowl.Contains(other.gameObject)) // ������ item�� list�� �߰����� ���� item�� ���
        {
            itemsInBowl.Add(other.gameObject);
        }
    }

    // Ư�� �±׸� ���� �������� Bowl �ȿ� �ִ���?
    public bool HasItem(string tagName)
    {
        foreach (var item in itemsInBowl) // itemInBowl list���� �ִ� �����۵� ��
        {
            if (item != null && item.CompareTag(tagName)) // item�� null�� �ƴ� ��, �������� tagName�� tag�� ���� ���
            {
                return true; // true ��ȯ
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

            // ���� �±� �� �ϳ��� ��Ī�Ǹ� �߰�
            foreach (var t in tags)
            {
                if (item.CompareTag(t))
                {
                    result.Add(item);
                    break; // �̹� �߰������Ƿ� ���� �±״� ����
                }
            }
        }
        return result;
    }
}