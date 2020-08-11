using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ApiBlockImpl : IBlock
{
    BlocklyEvents eventObj;
    bool allQueryWithKeyInCondition = false;
    string oneLevelAboveKeyName;
    List<QueryAddition> above;
    List<QueryAddition> below;
    static Dictionary<string, string> queryResultsMapping = new Dictionary<string, string>();

    public object parse(BlocklyEvents eventObject, string blockType, string codeBlockName, XElement element)
    {
        object obj = null;
        this.eventObj = eventObject;

        switch (blockType)
        {
            case "NodeValue":
                obj = parseNodeValue(element);
                break;
            case "QueryAndOr":
                obj = parseQueryAndOr(element);
                break;
            case "ARQueryNew":
                obj = parseARQueryNew(element);
                break;
            case "ARQueryAllNew":
                obj = parseARQueryAllNew(element);
                break;
            default:
                Console.WriteLine("Default case, may be simple event at the start! " +
                    "lets see what is it : " + blockType);
                break;
        }
        return obj;
    }

    private object parseNodeValue(XElement element)
    {
        string nodeType = element.Descendants(BlocklyUtil.ns + "field")
            .Where(child => child.Attribute("name").Value.Equals("nodeValue")).FirstOrDefault().Value;
        string queryLevel = element.Descendants(BlocklyUtil.ns + "field")
            .Where(child => child.Attribute("name").Value.Equals("queryLevel")).FirstOrDefault().Value;
        string logicCompare = element.Descendants(BlocklyUtil.ns + "field")
            .Where(child => child.Attribute("name").Value.Equals("logicCompare")).FirstOrDefault().Value;
        XElement nodeNameBlock = element.Descendants(BlocklyUtil.ns + "value")
            .Where(child => child.Attribute("name").Value.Equals("ConditionNodeName")).FirstOrDefault();
        string nodeName = null;
        if (nodeNameBlock != null)
        {
            nodeName = BlocklyUtil.getStringFromObj(getObjectFromValueBlock(nodeNameBlock));
        }
        XElement nodeValueBlock = element.Descendants(BlocklyUtil.ns + "value")
            .Where(child => child.Attribute("name").Value.Equals("NodeConditionValue")).FirstOrDefault();
        string nodeValue = BlocklyUtil.getStringFromObj(getObjectFromValueBlock(nodeValueBlock));

        string condition = "";
        string logicOperator = getLogicOperator(logicCompare);
        if (nodeType.Equals("Key"))
        {
            condition = "(@.['" + nodeValue + "'])";
        }
        else if (nodeType.Equals("Value"))
        {
            condition = "(@.['" + nodeName + "']" + logicOperator + "'" + nodeValue + "')";
        }
        QueryCondition qc = new QueryCondition();
        //qc.condition = condition;
        qc.type = nodeType;
        qc.level = queryLevel;
        qc.logicOperator = logicOperator;
        qc.nodeName = nodeName;
        qc.nodeValue = nodeValue;
        return qc;
    }

    private object parseQueryAndOr(XElement element)
    {
        object obj = null;
        QueryLogicCompare qlc = new QueryLogicCompare();

        string logic = element.Descendants(BlocklyUtil.ns + "field")
            .Where(child => child.Attribute("name").Value.Equals("selection")).FirstOrDefault().Value;
        qlc.logicOperator = getConditionOperator(logic);

        XElement valueBlock = element.Descendants(BlocklyUtil.ns + "value")
           .Where(child => child.Attribute("name").Value.Equals("QueryAndOrB")).FirstOrDefault();
        XElement vblock = valueBlock.Element(BlocklyUtil.ns + "block");
        if (vblock != null && vblock.Attribute("type") != null)
        {
            obj = eventObj.parseBlock(vblock);
        }
        if (obj != null)
        {
            if (obj.GetType().Equals(typeof(BlocklyReference)))
            {
                obj = ((BlocklyReference)obj).value;
            }
            if (obj.GetType().Equals(typeof(QueryCondition)))
            {
                qlc.qcA = (QueryCondition)obj;
            }
            else if (obj.GetType().Equals(typeof(QueryLogicCompare)))
            {
                qlc.qlcA = (QueryLogicCompare)obj;
            }
            else
            {
                //TODO : LAX : Should this even be the case??
            }
        }

        valueBlock = element.Descendants(BlocklyUtil.ns + "value")
           .Where(child => child.Attribute("name").Value.Equals("QueryAndOrA")).FirstOrDefault();
        vblock = valueBlock.Element(BlocklyUtil.ns + "block");
        if (vblock != null && vblock.Attribute("type") != null)
        {
            obj = eventObj.parseBlock(vblock);
        }
        if (obj != null)
        {
            if (obj.GetType().Equals(typeof(BlocklyReference)))
            {
                obj = ((BlocklyReference)obj).value;
            }
            if (obj.GetType().Equals(typeof(QueryCondition)))
            {
                qlc.qcB = (QueryCondition)obj;
            }
            else if (obj.GetType().Equals(typeof(QueryLogicCompare)))
            {
                qlc.qlcB = (QueryLogicCompare)obj;
            }
            else
            {
                //TODO : LAX : Should this even be the case??
            }
        }

        return qlc;
    }

    private object parseARQueryNew(XElement element)
    {
        object obj = new object();
        element = BlocklyUtil.applyNameSpace(element);
        XElement queryNameValue = element.Descendants(BlocklyUtil.ns + "value")
            .Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault();
        string queryName = null;
        if (queryNameValue != null)
        {
            obj = getObjectFromValueBlock(queryNameValue);
            if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
            {
                queryName = ((BlocklyReference)obj).value.ToString();
            }
            else
            {
                queryName = obj.ToString();
            }
        }

        string selection = element.Descendants(BlocklyUtil.ns + "field")
            .Where(child => child.Attribute("name").Value.Equals("selection")).FirstOrDefault()?.Value;

        XElement queryNodeValue = element.Descendants(BlocklyUtil.ns + "value")
            .Where(child => child.Attribute("name").Value.Equals("queryNode")).FirstOrDefault();
        string queryNode = null;
        if (queryNodeValue != null)
        {
            obj = getObjectFromValueBlock(queryNodeValue);
            if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
            {
                queryNode = ((BlocklyReference)obj).value.ToString();
            }
            else
            {
                queryNode = obj.ToString();
            }
        }
        queryNode = "['" + queryNode.Trim('\'') + "']";
        BlocklyReference bRef = new BlocklyReference();
        if (selection.Contains("key") || selection.Contains("Key"))
        {
            //Get the info from backend
            string queryPath = "";
            if (selection.Equals("parent-key"))
            {
                selection = "one-key";
                queryPath = "$..[?(@." + queryNode + ")]";
            }
            else if (selection.Equals("children-keys"))
            {
                selection = "all-key";
                queryPath = "$.." + queryNode + ".*";
            }
            else if (selection.Equals("sibling-keys"))
            {
                selection = "all-key";
                queryPath = "$..[?(@." + queryNode + ")].*";
            }
            bRef = getQueryResultsFromBackend(queryName, queryPath, selection);
        }
        else
        {
            string queryPath = "$..*" + queryNode;
            bRef.value = queryPath;
            bRef = getQueryResultsNew(bRef, queryName, selection);
        }
        obj = bRef;
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseARQueryAllNew(XElement element)
    {
        above = new List<QueryAddition>();
        below = new List<QueryAddition>();
        allQueryWithKeyInCondition = false;
        oneLevelAboveKeyName = "";

        object obj = new object();
        element = BlocklyUtil.applyNameSpace(element);
        XElement queryNameValue = element.Descendants(BlocklyUtil.ns + "value")
            .Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault();
        string queryName = null;
        if (queryNameValue != null)
        {
            obj = getObjectFromValueBlock(queryNameValue);
            if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
            {
                queryName = ((BlocklyReference)obj).value.ToString();
            }
            else
            {
                queryName = obj.ToString();
            }
        }

        string selection = element.Descendants(BlocklyUtil.ns + "field")
            .Where(child => child.Attribute("name").Value.Equals("selection")).FirstOrDefault()?.Value;

        XElement queryNodeValue = element.Descendants(BlocklyUtil.ns + "value")
            .Where(child => child.Attribute("name").Value.Equals("queryNode")).FirstOrDefault();
        string queryNode = null;
        if (queryNodeValue != null)
        {
            obj = getObjectFromValueBlock(queryNodeValue);
            if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
            {
                queryNode = ((BlocklyReference)obj).value.ToString();
            }
            else
            {
                queryNode = obj.ToString();
            }
        }

        XElement whereNodeValue = element.Descendants(BlocklyUtil.ns + "value")
           .Where(child => child.Attribute("name").Value.Equals("Where")).FirstOrDefault();
        XElement vblock = whereNodeValue.Element(BlocklyUtil.ns + "block");
        if (vblock != null && vblock.Attribute("type") != null)
        {
            obj = eventObj.parseBlock(vblock);
        }

        string queryPath = "";
        queryNode = "['" + queryNode.Trim('\'') + "']";
        if (obj.GetType().Equals(typeof(QueryCondition)))
        {
            queryPath = getQueryPathFromQueryCondition(queryNode, (QueryCondition)obj);
        }
        else if (obj.GetType().Equals(typeof(QueryLogicCompare)))
        {
            getQueryPathFromQueryLogicCompare((QueryLogicCompare)obj);

            queryPath = above.Count > 0 ? "[?(" : "";
            foreach (QueryAddition qa in above)
            {
                queryPath = queryPath + qa.condition + " " + qa.operand + " ";
            }
            queryPath = queryPath.Trim().TrimEnd('&').Trim('|').Trim();
            queryPath = queryPath.Length > 0 ? queryPath + ")].." : "";
            queryPath = (oneLevelAboveKeyName != null && oneLevelAboveKeyName.Length > 0) ?
                    queryPath + oneLevelAboveKeyName + "." + queryNode : queryPath + queryNode;

            queryPath = below.Count > 0 ? queryPath + "[?(" : queryPath;
            foreach (QueryAddition qa in below)
            {
                queryPath = queryPath + qa.condition + qa.operand;
            }
            queryPath.TrimEnd('&').Trim('|');
            queryPath = queryPath.EndsWith(queryNode) ? queryPath : queryPath + ")]";
        }
        else
        {
            //TODO : LAX : Should this even be the case??
        }

        BlocklyReference bRef = new BlocklyReference();
        if (selection.Contains("key") || selection.Contains("Key") || allQueryWithKeyInCondition)
        {
            //Get the info from backend
            if (selection.Equals("parent-key"))
            {
                selection = "one-key";
                if (queryPath.EndsWith(queryNode))
                {
                    queryPath = "$.." + queryPath.Replace(queryNode, "*");
                }
            }
            else if (selection.Equals("children-keys"))
            {
                selection = "all-key";
                queryPath = "$.." + queryPath + ".*";
            }
            else if (selection.Equals("sibling-keys"))
            {
                if (queryPath.EndsWith(queryNode))
                {
                    queryPath = "$.." + queryPath.Replace(queryNode, "*");
                }
                selection = "all-key";
            }
            else if (selection.Equals("one-value"))
            {
                queryPath = "$.." + queryPath;
                selection = "one-value";
            }
            else
            {
                queryPath = "$.." + queryPath;
                selection = "all-value";
            }
            bRef = getQueryResultsFromBackend(queryName, queryPath, selection);
        }
        else
        {
            bRef.value = "$..*" + queryPath;
            if (selection == null)
            {
                selection = "";
            }
            try
            {
                bRef = getQueryResultsNew(bRef, queryName, selection);
                if (bRef.value == null)
                {
                    //May be something wrong with C# query logic, lets try backend to be very sure
                    bRef.value = "$..*" + queryPath;
                    bRef = getQueryResultsFromBackend(queryName, bRef.value.ToString(), selection);
                }
            }
            catch (Exception e)
            {
                Debug.Log("<color=red> Query from C# may have issues, doing backend call : </color>" + e.StackTrace);
                bRef = getQueryResultsFromBackend(queryName, bRef.value.ToString(), selection);
            }
        }

        obj = bRef;
        eventObj.parseNextBlock(element);
        return obj;
    }

    private void getQueryPathFromQueryLogicCompare(QueryLogicCompare qlc)
    {
        if (qlc.qcA != null)
        {
            QueryCondition qc = qlc.qcA;
            string condition = getQueryFromQueryCondition(qc);
            QueryAddition qa = new QueryAddition();
            qa.condition = condition;
            qa.operand = qlc.logicOperator;
            allQueryWithKeyInCondition = qc.type.Equals("Key") ? true : false;
            if (qc.level.Equals("AnyLevelAbove") ||
                qc.level.Equals("OneLevelAbove") || qc.level.Equals("SameLevel"))
            {
                above.Add(qa);
                if (qc.level.Equals("OneLevelAbove") && qc.type.Equals("Key"))
                {
                    oneLevelAboveKeyName = qc.nodeValue;
                }
            }
            else if (qc.level.Equals("OneLevelBelow"))
            {
                below.Add(qa);
            }
            else if (qc.level.Equals("AnyLevelBelow"))
            {
                //TODO LAX : Not sure how to handle this
            }
        }
        else if (qlc.qlcA != null)
        {
            getQueryPathFromQueryLogicCompare(qlc.qlcA);
        }
        else
        {
            //TODO LAX : looks like side A condition is empty!
        }

        if (qlc.qcB != null)
        {
            QueryCondition qc = qlc.qcB;
            string condition = getQueryFromQueryCondition(qc);
            QueryAddition qa = new QueryAddition();
            qa.condition = condition;
            qa.operand = qlc.logicOperator;
            allQueryWithKeyInCondition = qc.type.Equals("Key") ? true : false;
            if (qc.level.Equals("AnyLevelAbove") ||
                qc.level.Equals("OneLevelAbove") || qc.level.Equals("SameLevel"))
            {
                above.Add(qa);
                if (qc.level.Equals("OneLevelAbove") && qc.type.Equals("Key"))
                {
                    oneLevelAboveKeyName = qc.nodeValue;
                }
            }
            else if (qc.level.Equals("OneLevelBelow"))
            {
                below.Add(qa);
            }
            else if (qc.level.Equals("AnyLevelBelow"))
            {
                //TODO LAX : Not sure how to handle this
            }
        }
        else if (qlc.qlcB != null)
        {
            getQueryPathFromQueryLogicCompare(qlc.qlcB);
        }
        else
        {
            //TODO LAX : looks like side A condition is empty!
        }
    }

    private string getQueryPathFromQueryCondition(string node, QueryCondition qc)
    {
        string path = "";
        string level = qc.level;
        string condition = getQueryFromQueryCondition(qc);

        if (level.Equals("AnyLevelAbove"))
        {
            if (qc.type.Equals("Key"))
            {
                //path = "[?(" + condition + ")].." + qc.nodeValue + ".." + node;
                path = "[?(" + condition + ")].." + qc.nodeValue + ".." + node;
                allQueryWithKeyInCondition = true;
            }
            else
            {
                path = "[?(" + condition + ")].." + node;
            }
        }
        else if (level.Equals("OneLevelAbove") || level.Equals("SameLevel"))
        {
            if (qc.type.Equals("Key"))
            {
                //path = "[?(" + condition + ")].." + qc.nodeValue + "." + node;
                path = "[?(" + condition + ")].." + qc.nodeValue + ".." + node;
                allQueryWithKeyInCondition = true;
            }
            else
            {
                //path = "[?(" + condition + ")].." + node;
                path = "[?(" + condition + ")]." + node;
            }
        }
        else if (level.Equals("OneLevelBelow"))
        {
            path = node + "[?(" + condition + ")]";
        }
        else if (level.Equals("AnyLevelBelow"))
        {
            //TODO LAX : Not sure how to handle this
        }
        return path;
    }

    private string getQueryFromQueryCondition(QueryCondition qc)
    {
        string condition = "";
        //qc.nodeName = BlocklyUtil.getStringFromObj(qc.nodeName);
        //qc.nodeValue = BlocklyUtil.getStringFromObj(qc.nodeValue);
        if (qc.type.Equals("Key"))
        {
            condition = "(@.['" + qc.nodeValue + "'])";
        }
        else if (qc.type.Equals("Value"))
        {
            condition = "@.['" + qc.nodeName + "']" + qc.logicOperator + "'" + qc.nodeValue + "'";
        }
        return condition;
    }

    private object getObjectFromValueBlock(XElement element)
    {
        object obj = null;
        XElement vblock = element.Element(BlocklyUtil.ns + "block");
        XElement shadowBlock = element.Element(BlocklyUtil.ns + "shadow");
        if (vblock != null && vblock.Attribute("type") != null)
        {
            obj = eventObj.parseBlock(vblock);
        }
        else if (shadowBlock != null && shadowBlock.Attribute("type") != null)
        {
            obj = eventObj.parseBlock(shadowBlock);
        }
        return obj;
    }

    private string getLogicOperator(string op)
    {
        switch (op)
        {
            case "EQ":
                return "==";
            case "NEQ":
                return "!=";
            case "LT":
                return "<";
            case "LTE":
                return "<=";
            case "GT":
                return ">";
            case "GTE":
                return ">=";
            default:
                return "==";
        }
    }

    private string getConditionOperator(string op)
    {
        switch (op)
        {
            case "AND":
                return "&&";
            case "OR":
                return "||";
            default:
                return "";
        }
    }

    public static BlocklyReference getQueryResultsNew(BlocklyReference bRef, string queryName, string selection)
    {
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
                    }
                    catch (Exception e)
                    {
                        Debug.Log("<color=red> @@@@@@@@@@  WriteAllText exception : </color>" + e.StackTrace);
                    }
                }
            }
        }
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
        //IEnumerable<JToken> queryResults = jsonObject.SelectTokens(bRef.value.ToString());
        if (selection.StartsWith("one-", StringComparison.Ordinal))
        {
            var value = jsonObject.SelectToken(bRef.value.ToString());
            bRef.value = null;
            if (value != null)
            {
                string val = value.ToString();
                bRef = BlocklyUtil.getResult(val, bRef);
            }
            //Since its just one result, lets set type as string
            bRef.type = "string";
            return bRef;
        }
        else
        {
            List<string> listofStuff = jsonObject.SelectTokens(bRef.value.ToString()).Select(s => (string)s).ToList();
            bRef.value = null;
            bRef = BlocklyUtil.getResultList(listofStuff, bRef);
            return bRef;
        }
    }

    public static BlocklyReference getQueryResultsFromBackend(string queryName, string queryPath, string selection)
    {
        string imagePath = Path.Combine(Application.persistentDataPath, GlobalVariables.VUFORIA_UNIQUE_ID);
        if (!Directory.Exists(imagePath))
        {
            Directory.CreateDirectory(imagePath);
        }

        string uniqueQueryId = queryName + "-" + queryPath + "-" + selection;
        string queryNameLocalPath = Path.Combine(Application.persistentDataPath,
                       GlobalVariables.VUFORIA_UNIQUE_ID, uniqueQueryId + ".json");
        if (!File.Exists(queryNameLocalPath))
        {
            Debug.Log("<color=green> Calling backend for results. queryName = " + queryName
                + " , queryPath = " + queryPath + " , selection = " + selection + " : </color>");
            queryPath = Uri.EscapeDataString(queryPath);
            //string url = "http://dropbox-env.8xe8tpevpj.us-west-1.elasticbeanstalk.com/keyqueryresult/" + queryName + "/" + queryPath + "/" + selection;
            //string url = "http://localhost:5000/keyqueryresult/" + queryName + "/" + queryPath + "/" + selection;
            string url = GlobalVariables.GET_Key_Query_Results + queryName + "/" + queryPath + "/" + selection;
            var queryResponse = WebFunctions.Get(url);
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
        } else
        {
            Debug.Log("<color=green> NOT calling backend for results. queryName = " + queryName
               + " , queryPath = " + queryPath + " , selection = " + selection + " : </color>");
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

        responses = responses.Trim().Trim('"').Trim(']').Trim('[').Trim('"');
        if (selection.StartsWith("parent", StringComparison.OrdinalIgnoreCase) ||
            selection.Contains("one"))
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
                    string ss = s.Trim().Trim('"');
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
                    string ss = s.Trim().Trim('"');
                    resultList.Add(ss);
                }
            }
            else if (responses.Contains(","))
            {
                responses = Regex.Replace(responses, @"\t\n\r", "");
                string[] delimArray = { "," };
                string[] splitString = responses.Split(delimArray, StringSplitOptions.None);
                foreach (string s in splitString)
                {
                    string ss = s.Trim().Trim('"');
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
}

public class QueryAddition
{
    public string condition;
    public string operand;
}

public class QueryCondition
{
    public string condition;
    public string type;
    public string level;
    public string logicOperator;
    public string nodeName;
    public string nodeValue;
}

public class QueryLogicCompare
{
    public string logicOperator;
    public QueryCondition qcA;
    public QueryCondition qcB;
    public QueryLogicCompare qlcA;
    public QueryLogicCompare qlcB;
}