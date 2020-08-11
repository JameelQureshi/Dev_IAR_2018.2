using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class DropdownEventScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onValueChange(GameObject gameobject)
    {
        if ((gameobject.GetComponentInChildren<Text>().text.ToLower().Equals("back")) ^
                 (gameobject.GetComponentInChildren<Text>().text.ToLower().Equals("<<")))
        {
            Back2CloudScene();
        }

        BlocklyEvents bEvents = new BlocklyEvents();
        bEvents.eventType = "ItemSelect";
        bEvents.clickedEvent(gameObject);
    }

    public void Back2CloudScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("3-CloudReco");
    }
}
