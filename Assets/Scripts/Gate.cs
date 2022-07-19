using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    [SerializeField] string nextStage;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (GetComponent<BossStage>() != null)
            {
                GetComponent<BossStage>().Boss();
            }
            if (nextStage == "Title")
            {
                SoundManager.Instance.PlaySeByName("clearall");
                if (SceneManager.GetActiveScene().name == "Stage01_3")
                {
                    SoundManager.Instance.StopBgm();
                    SoundManager.Instance.PlayBgmByName("bgm_techsynth_rhythm");
                }else if(SceneManager.GetActiveScene().name == "Stage02_3")
                {
                    SoundManager.Instance.StopBgm();
                    SoundManager.Instance.PlayBgmByName("bgm_techsynth_rhythm");
                } else if (SceneManager.GetActiveScene().name == "Stage03_3")
                {
                    SoundManager.Instance.StopBgm();
                    SoundManager.Instance.PlayBgmByName("bgm_techsynth_rhythm");
                }
            }
            else
            {
                SoundManager.Instance.PlaySeByName("clear");
            }
           
           SceneManagerScript.ChangeStage(nextStage);
            FlagManager.Instance.ClearStage(SceneManager.GetActiveScene().name);
        }
    }
    
}
