using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void RetryGame(){
    SceneManager.LoadSceneAsync(1);
   }

   public void MainMenu(){
      SceneManager.LoadSceneAsync(0);
   }
}
