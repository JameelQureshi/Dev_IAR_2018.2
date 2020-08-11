using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class BlocklyUtil
{
    public static XNamespace ns = "https://developers.google.com/blockly/xml";
    public static XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
    static Dictionary<string, string> queryResultsMapping = new Dictionary<string, string>();

    public static XElement applyNameSpace(XElement element)
    {
        string s = element.ToString().Trim();
        if (!s.StartsWith("<xml", StringComparison.Ordinal))
        {
            s = "<xml xmlns=\"https://developers.google.com/blockly/xml\"> \n" + s + "\n</xml>";
            element = XElement.Parse(s);
        }
        return element;
    }

    public static XmlNamespaceManager getnsmgr()
    {
        nsmgr.AddNamespace("prefix", "https://developers.google.com/blockly/xml");
        return nsmgr;
    }

    public static Color getColor(String color)
    {
        //if (color.IndexOf("rgba") == 0)
        if (color.StartsWith("rgba", StringComparison.OrdinalIgnoreCase))
        {
            //Debug.Log("<color=red> @@@@@@@@@@  ITS RGBA COLOR :  </color>" + color);
            return getRGBAColor(color);
        }

        Color newCol;
        if (string.IsNullOrEmpty(color))
        {
            color = "#000000";
        }
        else
        {
            // Debug.Log("<color=red> @@@@@@@@@@  getColor Color is : </color>" + color);
            if (color.IndexOf("#") != -1)
            {
                color = color.Substring(color.LastIndexOf("#"));
            }
            else
            {
                color = "#" + color;
            }
            // Debug.Log("<color=green> @@@@@@@@@@  getColor changed Color is : </color>" + color);

        }
        if (ColorUtility.TryParseHtmlString(color, out newCol))
        {
            return newCol;
        }
        return newCol;

    }

    private static Color getRGBAColor(String color)
    {
        //color = "rgba(255, 255, 255, 1)";
        color = color.Trim().Substring("rgba(".Length).Replace(")", string.Empty);

        string[] words = color.Split(',');
        float r = 1;
        float g = 1;
        float b = 1;
        float a = 1;
        try
        {
            if (words.Length == 4)
            {
                r = float.Parse(words[0]) / 255;
                g = float.Parse(words[1]) / 255;
                b = float.Parse(words[2]) / 255;
                a = float.Parse(words[3]);
            }
        }
        catch (Exception e)
        {
            Debug.Log("<color=red> @@@@@@@@@@  Exception in Building RGBA color without space  is : </color>" + e.StackTrace);
            r = 1; b = 1; g = 1; a = 1;
        }

        Color newCol = new Color(r, g, b, a);

        return newCol;

    }

    public static BlocklyReference getQueryResults(BlocklyReference bRef, string queryName, string selection)
    {
        string queryLocalPath = Path.Combine(Application.persistentDataPath,
                       GlobalVariables.VUFORIA_UNIQUE_ID, bRef.value.ToString() + ".xml");
        string queryNameLocalPath = Path.Combine(Application.persistentDataPath,
                       GlobalVariables.VUFORIA_UNIQUE_ID, queryName + ".json");
        string imagePath = Path.Combine(Application.persistentDataPath, GlobalVariables.VUFORIA_UNIQUE_ID);
        //Debug.Log("<color=red> @@@@@@@@@@  imagePath is : </color>" + imagePath);
        if (!Directory.Exists(imagePath))
        {
            Directory.CreateDirectory(imagePath);
        }
        if (!File.Exists(queryNameLocalPath))
        {
            string url = GlobalVariables.GET_Query_Results + "ALL/" + queryName;
            //string url = "http://localhost:5000/getqueryresult/" + "ALL/" + queryName;
            if (queryName != null && queryName.Trim().Length > 0)
            {
                url = url + "/" + queryName;
            }
            var queryResponse = WebFunctions.Get(url);
            if (queryResponse.error == null)
            {
                if (queryResponse.text != null && queryResponse.text.Length > 1)
                {
                    string queryPath = Path.Combine(Application.persistentDataPath,
                        GlobalVariables.VUFORIA_UNIQUE_ID, queryName + ".json");
                    try
                    {
                        File.WriteAllText(queryPath, queryResponse.text);
                        queryResultsMapping.Add(queryName, queryResponse.text);
                        //WriteAllTextWithBackup(queryName, queryResponse.text);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("<color=red> @@@@@@@@@@  WriteAllText exception : </color>" + e.StackTrace);
                    }

                }
            }
        }
        //string responses = File.ReadAllText(queryNameLocalPath);
        string responses = "";
        if (queryResultsMapping.ContainsKey(queryName))
        {
            responses = queryResultsMapping[queryName];
        }
        else
        {
            responses = File.ReadAllText(queryNameLocalPath);
        }
        JObject jsonObject = JObject.Parse(responses);
        IEnumerable<JToken> queryResults = jsonObject.SelectTokens(bRef.value.ToString());
        if (selection.StartsWith("one-", StringComparison.Ordinal))
        {
            var value = jsonObject.SelectToken(bRef.value.ToString());
            bRef.value = null;
            if (value != null)
            {
                string val = value.ToString();
                bRef = getResult(val, bRef);
            }
            //Since its just one result, lets set type as string
            bRef.type = "string";
            //List<String> listofStuff = jsonObject.SelectToken(bRef.value.ToString()).Select(s => (string)s).ToList();

            //if (listofStuff != null && listofStuff.Count > 0)
            //{
            //    bRef = getResult(listofStuff.ElementAt(0), bRef);
            //}

            return bRef;
        }
        else
        {
            List<String> listofStuff = jsonObject.SelectTokens(bRef.value.ToString()).Select(s => (string)s).ToList();
            bRef.value = null;
            bRef = getResultList(listofStuff, bRef);
            return bRef;
        }
    }

    public static BlocklyReference getQueryResults1(QueryInfo qi)
    {
        //qi = new QueryInfo();
        //qi.setQueryName("KeysightQuery2"); qi.setSelection("all-value");qi.setQueryNode("RedP");qi.setCondition("");qi.setA("");qi.setLogicCompareList("");qi.setB("");
        string imagePath = Path.Combine(Application.persistentDataPath, GlobalVariables.VUFORIA_UNIQUE_ID);
        if (!Directory.Exists(imagePath))
        {
            Directory.CreateDirectory(imagePath);
        }

        string uniqueQueryId = qi.createdId();
        string queryNameLocalPath = Path.Combine(Application.persistentDataPath,
                       GlobalVariables.VUFORIA_UNIQUE_ID, uniqueQueryId + ".json");
        if (!File.Exists(queryNameLocalPath))
        {
            string url = GlobalVariables.REST_SERVER + "query/jsonpath";
            var body = "{ \"queryName\" : \"" + qi.getQueryName() + "\"," +
            "\"selection\" : \"" + qi.getSelection() + "\"," +
            "\"queryNode\" : \"" + qi.getQueryNode() + "\"," +
            "\"condition\" : \"" + qi.getCondition() + "\"," +
            "\"a\" : \"" + qi.getA() + "\"," +
            "\"logicCompareList\" : \"" + qi.getLogicCompareList() + "\"," +
            "\"b\" : \"" + qi.getB() + "\"}";
            var encoding = new System.Text.UTF8Encoding();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("Accept", "application/json");
            var queryResponse = WebFunctions.PostHeader(url, encoding.GetBytes(body), headers);
            if (queryResponse.error == null || !queryResponse.error.Contains("Internal Server Error"))
            {
                if (queryResponse.text != null && queryResponse.text.Length > 1)
                {
                    try
                    {
                        File.WriteAllText(queryNameLocalPath, queryResponse.text);
                        queryResultsMapping.Add(queryNameLocalPath, queryResponse.text);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("<color=red> @@@@@@@@@@  WriteAllText exception : </color>" + e.StackTrace);
                    }
                }
            }
            else
            {
                return new BlocklyReference();
            }
        }

        string responses = "";
        BlocklyReference bRef = new BlocklyReference();
        bRef.name = "ARQuery";
        if (queryResultsMapping.ContainsKey(queryNameLocalPath))
        {
            responses = queryResultsMapping[queryNameLocalPath];
        }
        else
        {
            responses = File.ReadAllText(queryNameLocalPath);
            queryResultsMapping.Add(queryNameLocalPath, responses);
        }
        //if (responses.Trim().StartsWith("["))
        //{
        //    responses = responses.Trim().Substring(1);
        //}
        //if (responses.Trim().EndsWith("]"))
        //{
        //    responses = responses.Trim().Substring(1);
        //}

        responses = responses.Trim().Trim('"').Trim(']').Trim('[');
        if (qi.getSelection().StartsWith("one", StringComparison.OrdinalIgnoreCase))
        {

            bRef.type = "string";
            bRef.value = responses;
        }
        else
        {
            List<string> resultList = new List<string>();
            if (responses.Contains("},"))
            {
                string[] delimArray = { "}," };
                string[] splitString = responses.Split(delimArray, StringSplitOptions.None);
                foreach (string s in splitString)
                {
                    string ss = s.Trim();
                    if (ss.StartsWith("\""))
                    {
                        ss = ss.Substring(1);
                    }
                    resultList.Add(ss);
                }
            }
            else if (responses.Contains("\","))
            {
                responses = Regex.Replace(responses, @"\t\n\r", "");
                string[] delimArray = { "\"," };
                string[] splitString = responses.Split(delimArray, StringSplitOptions.None);
                foreach (string s in splitString)
                {
                    string ss = s.Trim();
                    if (ss.StartsWith("\""))
                    {
                        ss = ss.Substring(1);
                    }
                    resultList.Add(ss);
                }
            }
            else
            {
                responses = responses.Trim();
                if (responses.StartsWith("\""))
                {
                    responses = responses.Substring(1);
                }
                resultList.Add(responses);
            }
            bRef.type = "list";
            bRef.value = resultList;
        }
        return bRef;
    }

    public static bool chkprime(int num)
    {
        if (num <= 1) return false;
        if (num == 2) return true;
        if (num % 2 == 0) return false;

        for (int i = 2; i < num; i++)
            if (num % i == 0)
                return false;
        return true;
    }

    public static BlocklyReference getResult(string listofStuff, BlocklyReference bRef)
    {
        string refinedResult = listofStuff.Trim();
        if (refinedResult.StartsWith("\"", StringComparison.Ordinal))
        {
            refinedResult = refinedResult.Remove(0, 1);
        }
        if (refinedResult.EndsWith("\"", StringComparison.Ordinal))
        {
            refinedResult = refinedResult.Remove(refinedResult.Length - 1);
        }
        bRef.value = refinedResult;
        bRef.type = "list";

        return bRef;
    }

    public static BlocklyReference getResultList(List<String> listofStuff, BlocklyReference bRef)
    {
        List<string> resultList = new List<string>();
        foreach (string resultText in listofStuff)
        {
            string refinedResult = resultText.Trim();
            if (refinedResult.StartsWith("\"", StringComparison.Ordinal))
            {
                refinedResult = refinedResult.Remove(0, 1);
            }
            if (refinedResult.EndsWith("\"", StringComparison.Ordinal))
            {
                refinedResult = refinedResult.Remove(refinedResult.Length - 1);
            }
            resultList.Add(refinedResult);
        }
        bRef.value = resultList;
        bRef.type = "list";

        return bRef;
    }

    public static string getStringFromObj(object obj)
    {
        string returnVal = null;
        if (obj == null)
        {
            return null;
        }
        if (obj.GetType().Equals(typeof(string)))
        {
            returnVal = (string)obj;
        }
        else if (obj.GetType().Equals(typeof(BlocklyReference)))
        {
            returnVal = ((BlocklyReference)obj).value.ToString();
        }
        else
        {
            returnVal = obj.ToString();
        }
        return returnVal;
    }

    public static List<object> getListFromObj(object obj)
    {
        List<object> returnList = new List<object>();
        if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
        {
            BlocklyReference bRef = (BlocklyReference)obj;
            if (bRef.value != null)
            {
                if (bRef.value.GetType().Equals(typeof(List<string>)))
                {
                    returnList = ((List<string>)bRef.value).Cast<object>().ToList();
                }
                else if (bRef.value.GetType().Equals(typeof(List<object>)))
                {
                    returnList = (List<object>)bRef.value;
                }
                else if (bRef.value.GetType().Equals(typeof(string)))
                {
                    returnList.Add(bRef.value.ToString());
                }
                else
                {
                    returnList.Add(bRef.value.ToString());
                }
            }
        }
        else
        {
            if (obj.GetType().Equals(typeof(List<string>)))
            {
                returnList = ((List<string>)obj).Cast<object>().ToList();
            }
            else if (obj.GetType().Equals(typeof(List<object>)))
            {
                returnList = (List<object>)obj;
            }
            else if (obj.GetType().Equals(typeof(string)))
            {
                returnList.Add(obj.ToString());
            }
            else
            {
                returnList.Add(obj.ToString());
            }
        }

        return returnList;
    }

    public static XElement getValueBlock(XElement parent)
    {
        XElement block = parent.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        if (block == null)
        {
            //check if this is the case of shadow block
            block = parent.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "shadow");
        }
        return block;
    }

    public static XElement getValueBlockFromValue(XElement parent)
    {
        string ss = parent.ToString();
        //XElement block = parent.Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        XElement block = parent.Element(BlocklyUtil.ns + "block");
        if (block == null)
        {
            //check if this is the case of shadow block
            //block = parent.Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "shadow");
            block = parent.Element(BlocklyUtil.ns + "shadow");
        }
        return block;
    }

    public static string getEamilIdForPhoneNumber(string phone, string carrierId)
    {
        switch (carrierId)
        {
            case "ATT":
                return phone + "@txt.att.net";
            case "Verizon":
                return phone + "@vtext.com";
            case "Sprint":
                return phone + "@messaging.sprintpcs.com";
            case "TMobile":
                return phone + "@tmomail.net";
            default:
                return phone;
        }
    }

    //public static IEnumerator WaitForSecondsBlockly(float sec)
    //{
    //    //yield on a new YieldInstruction that waits for 5 seconds.
    //    yield return new WaitForSeconds(sec);
    //}
}
