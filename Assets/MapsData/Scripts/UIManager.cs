using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Canvas listView;
    public GameObject mapOnButton;
    public GameObject mapOffButton;
    public GameObject resetButton;
    public Slider radiusSlider;
    public Text radiusText;
    private bool isFirstTime = true;
    // Use this for initialization
    void Start () {
        mapOffButton.SetActive(false);
        radiusSlider.value = LocationDataManager.Radius;
        radiusText.text = LocationDataManager.Radius + " Miles";
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GoBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void TurnMapOn()
    {
        listView.enabled = false;
        resetButton.SetActive(true);
        mapOnButton.SetActive(false);
        mapOffButton.SetActive(true);
    } 
    public void TurnMapOff()
    {
        listView.enabled = true;
        resetButton.SetActive(false);
        mapOnButton.SetActive(true);
        mapOffButton.SetActive(false);
    }
    public void OnRadiusValueChanged()
    {
        if (isFirstTime)
        {
            isFirstTime = false;
            return;
        }
        StopAllCoroutines();
        LocationDataManager.Radius = (int)radiusSlider.value;
        radiusText.text = LocationDataManager.Radius + " Miles";
        StartCoroutine(Repopulate());
    }

    IEnumerator Repopulate()
    {
        yield return new WaitForSeconds(1);
        ListDataCreator.instance.RePopulate();
    }
}
