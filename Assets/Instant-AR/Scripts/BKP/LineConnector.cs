using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnector : MonoBehaviour
{

    public GameObject startObject;
    public GameObject targetObject;
    public float lineDistance = 400;
    public string popupMessage;

    LineRenderer line;
    public Color lineColor;
    Canvas mainCanvas;
    bool isClicked = false;
    // Use this for initialization
    public void clicked()
    {

        var clones = GameObject.FindGameObjectsWithTag("UIPopupTextTag");
        foreach (var clone in clones)
            {
                Destroy(clone);
            }
        targetObject = null;
        line = null;

        isClicked = false;
        if (startObject == null)
        {
            startObject = this.gameObject;
        }

        if (targetObject == null)
        {
            GameObject popupPrefab = (GameObject)Resources.Load("UIPopupText");
            GameObject popupObject = (GameObject)Instantiate(popupPrefab);
            mainCanvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
            //popupObject.transform.SetParent(mainCanvas.transform);
            popupObject.transform.SetParent(startObject.transform);
            popupObject.transform.localPosition = Vector3.zero;
            IAPopupScript popupScript = (IAPopupScript)popupObject.GetComponent<IAPopupScript>();
            popupScript.message = popupMessage;
            targetObject = popupObject;
        }

        line = targetObject.GetComponent<LineRenderer>();
        if (line == null)
        {
            // make line
            Debug.Log("<color=red> @@@@@@@@@ There was no line rendered attached to targetobject, so making new one  : </color>");
            line = targetObject.AddComponent<LineRenderer>();
            line.startColor = Color.black;
            line.endColor = Color.black;
            line.startWidth = 5f;
            line.endWidth = 5f;
            line.positionCount = 2;
            line.useWorldSpace = true;
            //Material mat = new Material(Shader.Find("Default-Line"));
            line.material=Resources.Load<Material>("Default-Line");
        }
        else{
            Debug.Log("<color=green> @@@@@@@@@ There was already one line renderer attached to targetobject  : </color>");
        }

        if (lineColor != null)
        {
            line.startColor = lineColor;
            line.endColor = lineColor;
        }


        //Vector3 initalDistance= (targetObject.transform.position - startObject.transform.position).normalized;
        //if(initalDistance == Vector3.zero){
        //    initalDistance = initalDistance + Vector3.one;
        //}

        float randomX = Random.Range(1.0f, 2.0f);
        if((Random.Range(0, 2) >= 1)){
            randomX = randomX * -1;
        }
        float randomY = Random.Range(1.0f, 2.0f);
        if ((Random.Range(0, 2) >= 1)){
            randomY = randomY * -1;
        }



        Vector3 randomPosition = new Vector3(randomX, randomY, 0);
        targetObject.transform.position = randomPosition * lineDistance + startObject.transform.position;
        ClampToArea();

        line.SetPosition(0, startObject.transform.position);
        line.SetPosition(1, targetObject.transform.position);
        isClicked = true;

    }

    private void ClampToArea()
    {
        RectTransform DragObjectInternal = targetObject.GetComponent<RectTransform>();

        RectTransform DragAreaInternal = mainCanvas.GetComponent<RectTransform>();

        Vector3 pos = DragObjectInternal.position+mainCanvas.transform.position;

        Vector3 minPosition = DragAreaInternal.rect.min - DragObjectInternal.rect.min;
        Vector3 maxPosition = DragAreaInternal.rect.max - DragObjectInternal.rect.max;

        pos.x = Mathf.Clamp(DragObjectInternal.position.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(DragObjectInternal.position.y, minPosition.y, maxPosition.y);

        DragObjectInternal.position = pos+mainCanvas.transform.position;
    }

    private void ClampToAreaOrg()
    {
        RectTransform DragObjectInternal = targetObject.GetComponent<RectTransform>();

        RectTransform DragAreaInternal = mainCanvas.GetComponent<RectTransform>();

        Vector3 pos = DragObjectInternal.localPosition;

        Vector3 minPosition = DragAreaInternal.rect.min - DragObjectInternal.rect.min;
        Vector3 maxPosition = DragAreaInternal.rect.max - DragObjectInternal.rect.max;

        pos.x = Mathf.Clamp(DragObjectInternal.localPosition.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(DragObjectInternal.localPosition.y, minPosition.y, maxPosition.y);

        DragObjectInternal.localPosition = pos;
    }

    //Update is called once per frame
    void Update()
    {
        if (isClicked)
        {
            if (targetObject != null)
            {
                line.SetPosition(0, startObject.transform.position);
                line.SetPosition(1, targetObject.transform.position);
            }
        }
    }
}
