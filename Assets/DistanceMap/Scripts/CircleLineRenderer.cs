using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic; using LitJson;

public class MyElement{

    public MyDistance distance;
}
public class MyDistance
{
    public string text;
    public string GameObjectName;

}


public class CircleLineRenderer : MonoBehaviour 
{

    struct TargetStruct
    {
        public string Name;
        public float Distance;
        public GameObject Line;
        public GameObject Target;
        public GameObject TextObj;
        public GameObject TextDown;
        public GameObject TextLeft;
        public GameObject TextRight;
        public Text Text;
    }

    public struct JSonObject
    {
        public string Name;
        public float Distance;
    }

    [SerializeField] DistanceMap circle;
    [SerializeField] GameObject SpawnerObject;
    [SerializeField] GameObject Target;
    [SerializeField] GameObject line;
    [SerializeField] GameObject ForwardObj;
    [SerializeField] GameObject UpObj;
    [SerializeField] float MinDistanceFromCenter = 250.0f;
    [SerializeField] float RandomRadiusMultiplier = 100.0f;
    [SerializeField] float MinAngleBetweenTargets = 5.0f;
    [SerializeField] float DistanceTextDifference = 45.0f;

    Vector2 targetDir;
    Vector2 temp;

    JSON_Helper helper;

    MyElement[] elements=new MyElement[7] ;
    TargetStruct[] targets; 
    bool updated = false;

    Dictionary<string, string> ObjectDistanceMapping = new Dictionary<string, string>();

    void Start()
    {
        StartCoroutine(GetText());
    }

    IEnumerator DownloadImage(string MediaUrl, GameObject obj)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            obj.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get(circle.JSON_String_URL);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(circle.MainCircleRadius, circle.MainCircleRadius, 1.0f);
            gameObject.GetComponent<RawImage>().color = circle.MainCircleColor;
            StartCoroutine(DownloadImage(circle.TargetImagesURL[5], gameObject));
            // Show results as text
            string json = www.downloadHandler.text;

            //===========
            JsonData jsonData = JsonMapper.ToObject(json);
            JsonUtility.ToJson(json);              Debug.Log("<color=green> ################## jsonText </color>" + json);              for (int i = 0; i < jsonData["destination_addresses"].Count; i++)             {                 //MyElement element = new MyElement();
                //MyDistance distance = new MyDistance();
                //element.distance = distance;

                //elements[i] = element;
                //distance.text = jsonData["rows"][0]["elements"][i]["distance"]["text"].ToString();
                //distance.GameObjectName = jsonData["destination_addresses"][i].ToString();
                ObjectDistanceMapping.Add(jsonData["destination_addresses"][i].ToString(), jsonData["rows"][0]["elements"][i]["distance"]["text"].ToString());             }             foreach (string key in ObjectDistanceMapping.Keys)             {                Debug.Log(key + ": " + ObjectDistanceMapping[key]);             }
            //Debug.Log(elements.Length);
            //for (int j= 0; j < elements.Length; j++){

            //    Debug.Log(elements[j].distance.GameObjectName+": "+elements[j].distance.text);
            //} 


            //==============





            string[] sections = json.Split(new char [] { '[', ']' }, System.StringSplitOptions.RemoveEmptyEntries);

            helper = new JSON_Helper(sections);

            targets = new TargetStruct[helper.objects.Count];

            float MinAngleCheck = 360.0f / targets.Length;

            System.Random random = new System.Random();
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].Name = helper.objects[i].Name;
                targets[i].Distance = helper.objects[i].Distance;
                targets[i].Line = Instantiate(line, SpawnerObject.transform);
                targets[i].Line.GetComponent<RectTransform>().localScale = new Vector3(1.0f, circle.LineThickness, 1.0f);
                targets[i].Line.GetComponent<RawImage>().color = circle.LineColor;
                targets[i].Target = Instantiate(Target, SpawnerObject.transform);
                targets[i].Target.GetComponent<RawImage>().color = circle.TargetCircleColor;
                targets[i].Target.GetComponent<RectTransform>().localScale = new Vector3(circle.TargetCircleRadius, circle.TargetCircleRadius, 1.0f);
                StartCoroutine(DownloadImage(circle.TargetImagesURL[i], targets[i].Target));
                targets[i].TextObj = targets[i].Target.transform.Find("Dist").gameObject;
                targets[i].TextDown = targets[i].Target.transform.Find("Dist1").gameObject;
                targets[i].TextLeft = targets[i].Target.transform.Find("Dist2").gameObject;
                targets[i].TextRight = targets[i].Target.transform.Find("Dist3").gameObject;
                targets[i].Text = targets[i].TextObj.GetComponent<Text>();
                Vector2 center = GetPosition(gameObject);
                float degree = i * (360.0f / targets.Length);
                Vector3 randPos = RandomCircle(center, (25f * RandomRadiusMultiplier) + MinDistanceFromCenter, degree / 360.0f);
                UpdatePosition(targets[i].Target, new Vector2(randPos.x, randPos.y));

            }
            
        }

        for (int i = 0; i < targets.Length; i++)
        {
            FollowTarget(targets[i]);
        }
    }

    bool AngleGood(int index)
    {
        if(index > 0)
        {
            for (int i = 0; i < index; i++)
            {
                if(Vector2.Angle(GetPosition(targets[i].Target) - GetPosition(gameObject), GetPosition(targets[index].Target) - GetPosition(gameObject)) < MinAngleBetweenTargets)
                {
                    return false;
                }
            }
        }
        return true;
    }

    void FollowTarget(TargetStruct s)
    {
        Vector2 Diff = GetPosition(s.Target) - GetPosition(gameObject);
        Vector2 forward = GetPosition(ForwardObj) - GetPosition(gameObject);
        Vector2 up = GetPosition(UpObj) - GetPosition(gameObject);

        float angle = Vector2.Angle(forward, Diff);

        Vector2 DiffInv = GetPosition(gameObject) - GetPosition(s.Target);

        s.Text.text = s.Distance.ToString();
        s.TextDown.GetComponent<Text>().text = s.Distance.ToString();
        s.TextLeft.GetComponent<Text>().text = s.Distance.ToString();
        s.TextRight.GetComponent<Text>().text = s.Distance.ToString();

        UpdatePosition(s.Line, Diff / 2.0f);

        if (GetPosition(s.Target).y < GetPosition(gameObject).y)
        {
            angle = -angle;
        }

        s.Line.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        s.Line.transform.localScale = new Vector3(Diff.magnitude / 100.0f, s.Line.transform.localScale.y, s.Line.transform.localScale.z);
        
        if(angle > 45.0f && angle < 135.0f)
        {
            TurnTextsOff(0, s);
        }
        else if(angle < -45.0f && angle > -135.0f)
        {
            TurnTextsOff(1, s);
        }
        else if(angle < 45 && angle > -45)
        {
            TurnTextsOff(3, s);
        }
        else
        {
            TurnTextsOff(2, s);
        }

    }
    
    void TurnTextsOff(int index, TargetStruct s)
    {
        if(index == 0)
        {
            s.TextObj.SetActive(true);
            s.TextDown.SetActive(false);
            s.TextLeft.SetActive(false);
            s.TextRight.SetActive(false);
        }
        else if(index == 1)
        {
            s.TextObj.SetActive(false);
            s.TextDown.SetActive(true);
            s.TextLeft.SetActive(false);
            s.TextRight.SetActive(false);
        }
        else if(index == 2)
        {
            s.TextObj.SetActive(false);
            s.TextDown.SetActive(false);
            s.TextLeft.SetActive(true);
            s.TextRight.SetActive(false);
        }
        else
        {
            s.TextObj.SetActive(false);
            s.TextDown.SetActive(false);
            s.TextLeft.SetActive(false);
            s.TextRight.SetActive(true);
        }
    }

    Vector2 GetPosition(GameObject obj)
    {
        return obj.GetComponent<RectTransform>().anchoredPosition;
    }

    Vector3 GetPosition3D(GameObject obj)
    {
        return obj.GetComponent<RectTransform>().anchoredPosition3D;
    }

    void UpdatePosition(GameObject obj, Vector2 pos)
    {
        obj.GetComponent<RectTransform>().anchoredPosition = pos; 
       
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    Vector3 RandomCircle(Vector3 center, float radius, float DegreeRatio)
    {
        float ang = DegreeRatio * 360.0f;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }

}
