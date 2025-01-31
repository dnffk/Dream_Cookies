using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape2D : MonoBehaviour
{
    public bool isCircle = false;
    public bool isHeart = false;
    public bool isStar = false;
    public bool isCookieMan = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Circle"))
        {
            isCircle = true;
        }
        else if(collision.gameObject.CompareTag("Heart"))
        {
            isHeart = true;
        }
        else if (collision.gameObject.CompareTag("Star"))
        {
            isStar = true;
        }
        else if (collision.gameObject.CompareTag("CookieMan"))
        {
            isCookieMan = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cut"))
        {
            isCookieMan = true;
        }
    }
}