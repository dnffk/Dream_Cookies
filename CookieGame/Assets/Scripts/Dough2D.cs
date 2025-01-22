using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dough2D : MonoBehaviour
{
    public RollingManager rollingManager;
    public bool isCollidedPin = false; // ���װ� Rolling pin�� ����ִ���?

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Rolling pin"))
        {
            isCollidedPin = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Rolling pin"))
        {
            isCollidedPin = false;
        }
    }
}