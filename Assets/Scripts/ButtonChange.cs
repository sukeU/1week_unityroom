using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChange : MonoBehaviour
{
    [SerializeField]List<Image> buttons = new List<Image>();
    // Start is called before the first frame update
    Color set = new Color(0.5f, 0.5f, 0.5f);
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            buttons.Add(transform.GetChild(i).GetComponent<Image>());
        }

    }

    private void Update()
    {
        foreach(var stage in FlagManager.Instance.clearStages)
        {
            foreach(var stageButton in buttons)
            {
                if (stageButton.name == stage)
                {
                    stageButton.color = set;
                }
            }
        }
    }
}


