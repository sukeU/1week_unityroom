using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObj : MonoBehaviour
{
    Color selectColor=new Color(0.5f,0.5f,0.5f);
    Color defaultColor = new Color(1.0f, 1.0f, 1.0f);
    public void setParent(Transform t)
    {
        transform.parent = t;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name== "GrabPoint")
        {
            GetComponent<SpriteRenderer>().color = selectColor;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "GrabPoint")
        {
            GetComponent<SpriteRenderer>().color = defaultColor;
        }
    }

}
