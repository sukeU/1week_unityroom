using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public static void Reload()
    {
        FadeManager.Instance.LoadScene(SceneManager.GetActiveScene().name, 0.25f);
      
    }
    public static void ChangeStage(string nextStage)
    {
        FadeManager.Instance.LoadScene(nextStage, 0.5f);
    }
    public  void SePlay(string name)
    {
        SoundManager.Instance.PlaySeByName(name);
    }
}
