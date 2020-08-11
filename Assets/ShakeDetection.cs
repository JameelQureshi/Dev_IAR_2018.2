using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;


public class ShakeDetection : MonoBehaviour {

    float accelerometerUpdateInterval = 1.0f / 60.0f;
    // The greater the value of LowPassKernelWidthInSeconds, the slower the
    // filtered value will converge towards current input sample (and vice versa).
    float lowPassKernelWidthInSeconds = 1.0f;
    // This next parameter is initialized to 2.0 per Apple's recommendation,
    // or at least according to Brady! ;)
    float shakeDetectionThreshold = 2.0f;

    float lowPassFilterFactor;
    Vector3 lowPassValue;

    private bool clicked;

    void Start()
    {
        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
    }
   
    void Update()
    {
        Vector3 acceleration = Input.acceleration;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        Vector3 deltaAcceleration = acceleration - lowPassValue;

        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
        {
            // Perform your "shaking actions" here. If necessary, add suitable
            // guards in the if check above to avoid redundant handling during
            // the same shake (e.g. a minimum refractory period).
            Debug.Log("Shake event detected at time " + Time.time);
            LoosenElements();

        }
    }

    void LoosenElements()
    {
        GameObject floatingObject;
        if (clicked)
        {
            foreach (string buttonName in GlobalVariables.ButtonList)
            {
                floatingObject = GameObject.Find(buttonName);
                SwingFloating swingScript = floatingObject.GetComponent<SwingFloating>();
                WindowDragger dragScript = floatingObject.GetComponent<WindowDragger>();
                if (swingScript != null)
                {
                    Destroy(swingScript);
                }
                if (dragScript != null)
                {
                    Destroy(dragScript);
                }
                floatingObject.transform.eulerAngles = Vector3.zero;
                floatingObject.GetComponent<Button>().interactable = true;
            }
            clicked = false;
        }
        else
        {
            foreach (string buttonName in GlobalVariables.ButtonList)
            {
                floatingObject = GameObject.Find(buttonName);
                floatingObject.AddComponent<SwingFloating>();
                floatingObject.AddComponent<WindowDragger>();
                floatingObject.GetComponent<Button>().interactable = false;

            }
            clicked = true;
        }

    }




}
