using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl2D : MonoBehaviour
{
    // Bowl ���� ������
    private List<GameObject> itemsInBowl = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        // foodItem �±׶�� ����, �Ǵ� ���� �±׸� �� ó���� ���� ����
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

    /// Ư�� �±׸� ���� �������� Bowl �ȿ� �ִ��� ����
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