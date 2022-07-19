using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoChange : MonoBehaviour
{
    [SerializeField] Sprite[] Logos = new Sprite[2];
    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        image.sprite = Logos[0];
    }
    // Update is called once per frame
    void Update()
    {
        if (FlagManager.Instance.AllClear)
        {
            image.sprite = Logos[1];
        }
    }
}
