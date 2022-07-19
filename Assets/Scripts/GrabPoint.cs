using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPoint : MonoBehaviour
{
    Player player;
    private void Start()
    {
        player = transform.parent.GetComponent<Player>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<GrabbableObj>()!=null)
        {
            player.IsGrabPoint = true;
            player.GrabPointObj = collision.gameObject;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<GrabbableObj>() != null&&!player.IsGrabPoint)
        {
            player.IsGrabPoint = true;
            player.GrabPointObj = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<GrabbableObj>() != null)
        {
            player.IsGrabPoint = false;
            player.GrabPointObj = null;
        }
    }
}
