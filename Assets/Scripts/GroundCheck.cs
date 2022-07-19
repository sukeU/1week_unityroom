using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] List<Collider2D> Colliders = new List<Collider2D>();
    Player player;
    private void Start()
    {
        player = transform.parent.GetComponent<Player>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Colliders.Add(collision);
        if (Colliders.Count > 0) {
         
            if (collision.gameObject.layer == 9&&player.transform.parent==null)
            {
                if (collision.transform.parent!=null)
                {
                    player.Board(collision.transform.parent);
                }
                else {
                    player.Board(collision.transform);
                }
               
            }
            else if (collision.tag == "Spike")
            {
                SceneManagerScript.Reload();
            }
            else if(collision.gameObject.layer != 9)//RunningCores‚Ì9
            {
                player.Board(null);
            }
            player.IsGround = true; 
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9 && player.transform.parent == null)
        {
            if (collision.transform.parent != null)
            {
                player.Board(collision.transform.parent);
            }
            else
            {
                player.Board(collision.transform);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Colliders.Remove(collision);
        if (Colliders.Count == 0)
        {
            if (collision.gameObject.layer == 9 && player.transform.parent!=null)
            {
                player.Board(null);
            }
            player.IsGround = false;
        }

    }
}
