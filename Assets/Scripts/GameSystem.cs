using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    private static GameSystem instance;

    public int totalscore = 0 ;

    public int totalplay = 0;

    public void AddScore(){

        totalscore++;
        Debug.Log("AddScore " + totalscore );
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
            instance.Init();
        }
        else
            Destroy(gameObject);
    }

    public static GameSystem GetInstance()
    {
        return instance;
    }

    public void Init()
    {
        Debug.Log("AddScore " + totalscore );
    }

    public void reset (){
        totalscore = 0;
        totalplay = 0;
    }

}

