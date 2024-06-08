using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleSound : MonoBehaviour
{
    public Leafsound sound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
   public void playSound()
    {
        sound.setsound();
        sound.scr.Play();
    }
}
