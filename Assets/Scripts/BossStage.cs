using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStage : MonoBehaviour
{
    // Start is called before the first frame update
    public void Boss()
    {
        SoundManager.Instance.StopBgm();
        SoundManager.Instance.PlayBgmByName("boss");
    }


}
