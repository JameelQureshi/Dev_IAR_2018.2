using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeysightHealthScreen : MonoBehaviour {
    public Button heathButton;

    void Start()
    {
       
    }
  
    public void mainscreen(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("3-CloudReco");
    }

    public void openHealthScreen()
    {
        heathButton.transform.localScale = new Vector3(0, 0, 0);
        UnityEngine.SceneManagement.SceneManager.LoadScene("StaticTargetImage");
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("3-CloudReco");
    }
}
