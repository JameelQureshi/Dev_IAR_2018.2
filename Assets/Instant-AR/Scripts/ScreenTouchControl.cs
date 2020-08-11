/*  * ==============================================================================   * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.  *   * @Author : Jitender Hooda   *   ==============================================================================  */


using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenTouchControl : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{    
       
    //Example text Object.
    public GameObject PopupObject;
    public GameObject Shadow;
    public GameObject Border;
    public GameObject VideoSlider;
    //Vectors Position.
    private Vector3 FirstPos;    //First position.
    private Vector3 LastPos;     //Last position.

    //added for zoom
    public float zoomOutMin = 0.0f;
    public float zoomOutMax = 10.0f;

    Vector3 touchStart;
    private float Distance;  //Minimum distance for a Swipe.
    private Color shadowColor;
    private float shadowDistance=20f;

    private float videoSliderWait = 5f;

    private bool videoPointerEnter;

    IEnumerator DisapperVideoSlider()
    {
        VideoSlider.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(5f);
        if (!videoPointerEnter)
        {
            VideoSlider.transform.localScale = Vector3.zero;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (Shadow != null)
        {
            Shadow.GetComponent<Image>().color = Color.red;
        }
        if (VideoSlider != null)
        {
            VideoSlider.transform.localScale = Vector3.one;
            videoPointerEnter = true;
            StartCoroutine(DisapperVideoSlider());
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {

        if (Shadow != null)
        {
            Shadow.GetComponent<Image>().color = Color.red;
        }
        if(VideoSlider != null)
        {
            VideoSlider.transform.localScale = Vector3.one;
            videoPointerEnter = true;
            StartCoroutine(DisapperVideoSlider());
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (Shadow != null) {
            Shadow.GetComponent<Image>().color = shadowColor;
        }
        if (VideoSlider != null)
        {
            //StartCoroutine(DisapperVideoSlider());
            videoPointerEnter = false;
        }
    }


    void Start()
    {

        Distance = Screen.height * 15 / 100; //DragDistance.

        if (Shadow != null)
        {
            shadowColor = Shadow.GetComponent<Image>().color;
            Shadow.GetComponent<RectTransform>().offsetMin = new Vector2(shadowDistance * -1, shadowDistance * -1);
            Shadow.GetComponent<RectTransform>().offsetMax = new Vector2(shadowDistance, shadowDistance);
        }

        if (Border != null && Border.GetComponent<BorderScript>() != null)
        {
            shadowDistance = shadowDistance + Border.GetComponent<BorderScript>().borderWidth;

        }

    }

    void Update()
    {
        SingleTouchActions();
        DoubleTouchActions();
        KeyboardTesting();
    }

    void SwipeLeft()
    {
        if (VideoSlider == null)
        {
            //Destroy(PopupObject);
            StartCoroutine(DecayDestroy(PopupObject));
        }
    }

    void SwipeRight()
    {
        if (VideoSlider == null)
        {
            //Destroy(PopupObject);
            StartCoroutine(DecayDestroy(PopupObject));
        }
    }

    void SwipeUp()
    {
        //Destroy(PopupObject);
        StartCoroutine(DecayDestroy(PopupObject));
    }

    void SwipeDown()
    {
        //Destroy(PopupObject);
        StartCoroutine(DecayDestroy(PopupObject));

    }
    // Enter the necessary actions, functions in these Voids END.

    IEnumerator DecayDestroy(GameObject decayingObject)
    {
        if (VideoSlider != null)
        {
            GlobalVariables.VIDEO_BUTTON_CLICKED = false;
            StreamVideo streamVideo = decayingObject.GetComponent<StreamVideo>();
            if (streamVideo != null)
            {
                UnityEngine.Video.VideoPlayer videoPlayer = streamVideo.getVideoPlayer();
                videoPlayer.Pause();
            }
        }


        float increment = 0.05f;
        float rotateAngle = 0f;

        while (decayingObject.transform.localScale.x > 0)
        {
            decayingObject.transform.localScale = decayingObject.transform.localScale - new Vector3(increment, increment, increment);
            //PrimaryScreenButton.GetComponent<RectTransform>().Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
            //decayingObject.transform.Rotate(0f, 0f, rotateAngle);
            yield return new WaitForSeconds(0.01f);
            //rotateAngle = rotateAngle + 5f;
        }
        Destroy(decayingObject);

    }

    void SingleTouchActions(){

        if (Input.touchCount == 1) // User TouchCount.
        {

            Touch touch = Input.GetTouch(0); // Get Touch.
            if (touch.phase == TouchPhase.Began) //Check for First Touch.
            {

                FirstPos = touch.position;
                LastPos = touch.position;

            }
            else if (touch.phase == TouchPhase.Moved) // Update the last position.
            {

                LastPos = touch.position;

            }
            else if (touch.phase == TouchPhase.Ended) //Check finger.
            {
                LastPos = touch.position;  //last touch position.

                //Check distance.
                if (Mathf.Abs(LastPos.x - FirstPos.x) > Distance || Mathf.Abs(LastPos.y - FirstPos.y) > Distance)
                {

                    if (Mathf.Abs(LastPos.x - FirstPos.x) > Mathf.Abs(LastPos.y - FirstPos.y))
                    {

                        if ((LastPos.x > FirstPos.x))
                        {

                            // Swipe Right!
                            SwipeRight();

                        }
                        else
                        {

                            // Swipe Left!
                            SwipeLeft();

                        }
                    }
                    else
                    {

                        if (LastPos.y > FirstPos.y)
                        {

                            // Swipe Up!
                            SwipeUp();

                        }
                        else
                        {

                            // Swipe Down!
                            SwipeDown();

                        }
                    }
                }
            }
        }
    }

    void DoubleTouchActions() {
        //zoom in-out code===================
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            float difference = currentMagnitude - prevMagnitude;

            zoom(difference * 0.001f);
        }

    }

    void KeyboardTesting(){

        //To Test Zoom from Mouse scroll

        // Swipes and tap working on mobile device. For debug use "Arrows" for swipes and "Space" for Tap. 

        zoom(Input.GetAxis("Mouse ScrollWheel")*0.1f);


        if (Input.GetKeyUp("up"))
        {

            //Example KeyBoard. Up Swipe.
            SwipeUp();

        }

        if (Input.GetKeyUp("down"))
        {

            //Example KeyBoard. Down Swipe.
            SwipeDown();

        }

        if (Input.GetKeyUp("left"))
        {

            //Example KeyBoard. Left Swipe.
            SwipeLeft();

        }

        if (Input.GetKeyUp("right"))
        {

            //Example KeyBoard. Right Swipe.
            SwipeRight();

        }

        if (Input.GetKeyUp("space"))
        {

            //Example KeyBoard. Tap!
            //SwipeTap();

        }
        // Debug Control from Arrows KeyBoard END.

    }


    void zoom(float increment)
    {

        //PopupObject.transform.localScale = PopupObject.transform.localScale + new Vector3(increment, increment, increment);


        if (PopupObject.transform.localScale.x >= zoomOutMin && PopupObject.transform.localScale.x<=zoomOutMax)
        {
            PopupObject.transform.localScale = PopupObject.transform.localScale + new Vector3(increment, increment, increment);
        }
        else if (PopupObject.transform.localScale.x < zoomOutMin) {
            PopupObject.transform.localScale = new Vector3(zoomOutMin, zoomOutMin, zoomOutMin);
        }
        else if (PopupObject.transform.localScale.x > zoomOutMax)
        {
            PopupObject.transform.localScale = new Vector3(zoomOutMax, zoomOutMax, zoomOutMax);
        }

        //Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }
}
