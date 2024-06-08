using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Leafsound : MonoBehaviour
{

    public AudioSource scr;
    public AudioClip audioClip;
    public void setsound()
    {
        scr.clip = audioClip;
        scr.Play();
    }
}
