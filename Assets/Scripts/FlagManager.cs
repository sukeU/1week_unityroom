using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagManager : SingletonMonoBehaviour<FlagManager>
{
    [SerializeField]public List<string> clearStages = new List<string>();
    [SerializeField] int count = 0;
    [SerializeField] public bool AllClear=false;
    public void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
     
    public void ClearStage(string currentStage)
    {
        if (!clearStages.Contains(currentStage))
        {
            clearStages.Add(currentStage);
            ++count;
        }
        if (count == 16&&!AllClear)
        {
            AllClear = true;
            SoundManager.Instance.PlaySeByName("allstageclear");
        }
    }
}
