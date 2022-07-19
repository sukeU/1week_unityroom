using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutPointCheck : MonoBehaviour
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
        if (Colliders.Count > 0) player.IsStack = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Colliders.Remove(collision);
        if (Colliders.Count == 0) player.IsStack = false;

    }
}
