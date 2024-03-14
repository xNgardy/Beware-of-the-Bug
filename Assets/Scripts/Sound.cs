using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
   [SerializeField] AudioSource sound;
   [SerializeField] AudioClip sfx1, sfx2;

   public void Click(){
    sound.clip = sfx1;
    sound.Play();
   }

   public void Explosion(){
    sound.clip = sfx2;
    sound.Play();
   }

}
