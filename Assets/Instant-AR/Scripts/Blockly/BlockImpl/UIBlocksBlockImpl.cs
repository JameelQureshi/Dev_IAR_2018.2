using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Xml.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class UIBlocksBlockImpl : IBlock
{
    BlocklyEvents eventObj;
    PopUpBlockImpl popupImpl;

    public UIBlocksBlockImpl()
    {
    }

    public object parse(BlocklyEvents eventObject, string blockType, string codeBlockName, XElement element)
    {
        object obj = null;
        this.eventObj = eventObject;
        this.popupImpl = new PopUpBlockImpl();
        this.popupImpl.eventObj = this.eventObj;

        switch (blockType)
        {
            case "colour_picker":
                if (codeBlockName != null && codeBlockName.Length > 1)
                {
                    if (codeBlockName.Equals("COLOUR"))
                    {
                        obj = parseColourPicker(element);
                    }
                    else
                    {
                        obj = parseColourPicker(element);
                    }
                }
                break;
            case "updateProperty":
                obj = parseUpdateProperty(element);
                break;
            case "newUpdateProperty":
                obj = parseNewUpdateProperty(element);
                break;
            case "CallElement":
                obj = parseCallElement(element);
                break;
            case "SendElement":
                obj = parseSendElement(element);
                break;
            case "Elements":
                obj = parseElements(element);
                break;
            case "WaitElement":
                obj = parseWaitElement(element);
                break;
            case "ARQueryAll":
                obj = parseARQueryAll(element);
                break;
            case "ARQuery":
                obj = parseARQuery(element);
                break;
            case "UITableReport":
                obj = parseUITableReport(element);
                break;
            case "fieldPopOverVideo":
                obj = parseFieldPopOverVideo(element);
                break;
            case "simpleEvent":
                break; ////Gonna be a dummy implement for this as its tackled in testbuttonscript
            default:
                Console.WriteLine("Default case, may be simple event at the start! " +
                    "lets see what is it : " + blockType);
                break;
        }
        return obj;
    }

    private object parseColourPicker(XElement element)
    {
        element = BlocklyUtil.applyNameSpace(element);
        XElement ColorElement = element.Element(BlocklyUtil.ns + "block");
        if (ColorElement == null)
        {
            //cound be a case of shadow
            ColorElement = element.Element(BlocklyUtil.ns + "shadow");
        }
        string colorString = ColorElement.Element(BlocklyUtil.ns + "field").Value;
        //Color col = BlocklyUtil.getColor(colorString);
        //colorString = col.ToString();
        if (colorString.StartsWith("#"))
        {
            colorString = colorString.Replace("#", "");
        }
        colorString = colorString.ToUpper();
        return colorString;
    }

    private object parseARQuery(XElement element)
    {
        object obj = new object();
        element = BlocklyUtil.applyNameSpace(element);
        //string queryName = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault()?.Value;
        XElement queryNameValue = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault();
        string queryName = null;
        if (queryNameValue != null)
        {
            XElement qnblock = queryNameValue.Element(BlocklyUtil.ns + "block");
            XElement shadowBlock = queryNameValue.Element(BlocklyUtil.ns + "shadow");
            if (qnblock != null && qnblock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(qnblock);
            }
            else if (shadowBlock != null && shadowBlock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(shadowBlock);
            }
            if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
            {
                queryName = ((BlocklyReference)obj).value.ToString();
            }
            else
            {
                queryName = obj.ToString();
            }
        }

        string selection = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("selection")).FirstOrDefault()?.Value;
        //string queryNode = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("queryNode")).FirstOrDefault()?.Value;
        XElement queryNodeValue = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("queryNode")).FirstOrDefault();
        string queryNode = null;
        if (queryNodeValue != null)
        {
            XElement qnblock = queryNodeValue.Element(BlocklyUtil.ns + "block");
            XElement shadowBlock = queryNodeValue.Element(BlocklyUtil.ns + "shadow");
            if (qnblock != null && qnblock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(qnblock);
            }
            else if (shadowBlock != null && shadowBlock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(shadowBlock);
            }
            if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
            {
                queryNode = ((BlocklyReference)obj).value.ToString();
            }
            else
            {
                queryNode = obj.ToString();
            }
        }

        string queryPath = "$..*['" + queryNode + "']";
        BlocklyReference bRef = new BlocklyReference();
        if (selection.Equals("one-key", StringComparison.OrdinalIgnoreCase) ||
            selection.Equals("all-key", StringComparison.OrdinalIgnoreCase))
        {
            QueryInfo qi = new QueryInfo();
            qi.setQueryDetails(queryName, selection, queryNode);
            bRef = BlocklyUtil.getQueryResults1(qi);
        }
        else
        {
            bRef.value = queryPath;
            bRef = BlocklyUtil.getQueryResults(bRef, queryName, selection);
        }
        //bRef.value = queryPath;
        //bRef = BlocklyUtil.getQueryResults(bRef, queryName, selection);
        //QueryInfo qi = new QueryInfo();
        //qi.setQueryDetails(queryName, selection, queryNode);
        //bRef = BlocklyUtil.getQueryResults1(qi);
        obj = bRef;
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseARQueryAll(XElement element)
    {
        object obj = new object();
        element = BlocklyUtil.applyNameSpace(element);
        string selection = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("selection")).FirstOrDefault()?.Value;
        //string queryName = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault()?.Value;
        XElement queryNameValue = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault();
        string queryName = null;
        if (queryNameValue != null)
        {
            XElement block = queryNameValue.Element(BlocklyUtil.ns + "block");
            XElement shadowBlock = queryNameValue.Element(BlocklyUtil.ns + "shadow");
            if (block != null && block.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(block);
            }
            else if (shadowBlock != null && shadowBlock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(shadowBlock);
            }
            if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
            {
                queryName = ((BlocklyReference)obj).value.ToString();
            }
            else
            {
                queryName = obj.ToString();
            }
        }
        string queryPath = getQueryPathAll(element);
        BlocklyReference bRef = new BlocklyReference();
        if (selection.Equals("one-key", StringComparison.OrdinalIgnoreCase) ||
            selection.Equals("all-key", StringComparison.OrdinalIgnoreCase))
        {
            QueryInfo qi = getQueryPathAll1(element);
            bRef = BlocklyUtil.getQueryResults1(qi);
        }
        else
        {
            bRef.value = queryPath;
            if (selection == null)
            {
                selection = "";
            }
            bRef = BlocklyUtil.getQueryResults(bRef, queryName, selection);
        }
        //bRef.value = queryPath;
        //if (selection == null)
        //{
        //    selection = "";
        //}
        //bRef = BlocklyUtil.getQueryResults(bRef, queryName, selection);
        //QueryInfo qi = getQueryPathAll1(element);
        //bRef = BlocklyUtil.getQueryResults1(qi);
        obj = bRef;
        eventObj.parseNextBlock(element);
        return obj;
    }

    //private string getQueryPath(XElement element)
    //{
    //    element = BlocklyUtil.applyNameSpace(element);
    //    object obj = new object();
    //    string queryName = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault()?.Value;
    //    string selection = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("selection")).FirstOrDefault()?.Value;
    //    string queryNode = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("queryNode")).FirstOrDefault()?.Value;
    //    return "$..['" + queryNode + "']";
    //}

    private QueryInfo getQueryPathAll1(XElement element)
    {
        QueryInfo qi = new QueryInfo();
        element = BlocklyUtil.applyNameSpace(element);
        object obj = new object();
        //string queryName = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault()?.Value;
        XElement queryNameValue = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault();
        string queryName = null;
        if (queryNameValue != null)
        {
            XElement qnblock = queryNameValue.Element(BlocklyUtil.ns + "block");
            XElement shadowBlock = queryNameValue.Element(BlocklyUtil.ns + "shadow");
            if (qnblock != null && qnblock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(qnblock);
            }
            else if (shadowBlock != null && shadowBlock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(shadowBlock);
            }
            if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
            {
                queryName = ((BlocklyReference)obj).value.ToString();
            }
            else
            {
                queryName = obj.ToString();
            }
        }

        string selection = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("selection")).FirstOrDefault()?.Value;

        //string queryNode = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("queryNode")).FirstOrDefault()?.Value;
        XElement queryNodeValue = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("queryNode")).FirstOrDefault();
        string queryNode = null;
        if (queryNodeValue != null)
        {
            XElement qnblock = queryNodeValue.Element(BlocklyUtil.ns + "block");
            XElement shadowBlock = queryNodeValue.Element(BlocklyUtil.ns + "shadow");
            if (qnblock != null && qnblock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(qnblock);
            }
            else if (shadowBlock != null && shadowBlock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(shadowBlock);
            }
            if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
            {
                queryNode = ((BlocklyReference)obj).value.ToString();
            }
            else
            {
                queryNode = obj.ToString();
            }
        }

        string condition = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("condition")).FirstOrDefault()?.Value;

        //string a = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("A")).FirstOrDefault()?.Value;
        XElement aNodeValue = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("nodeName")).FirstOrDefault();
        string a = null;
        if (aNodeValue != null)
        {
            XElement qnblock = aNodeValue.Element(BlocklyUtil.ns + "block");
            XElement shadowBlock = aNodeValue.Element(BlocklyUtil.ns + "shadow");
            if (qnblock != null && qnblock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(qnblock);
            }
            else if (shadowBlock != null && shadowBlock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(shadowBlock);
            }
            if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
            {
                a = ((BlocklyReference)obj).value.ToString();
            }
            else
            {
                a = obj.ToString();
            }
        }

        string logicCompareList = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("logicCompareList")).FirstOrDefault()?.Value;
        //XElement block = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        XElement block = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("B")).FirstOrDefault();
        obj = eventObj.parseBlock(block);
        string b = "";

        if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
        {
            BlocklyReference br = new BlocklyReference();
            br = (BlocklyReference)obj;
            b = (string)br.value;
        }
        else
        {
            b = (string)obj.ToString();
        }
        qi.setQueryDetails(queryName, selection, queryNode);
        qi.setQueryCondition(condition, a, logicCompareList, b);
        return qi;
    }


    private string getQueryPathAll(XElement element)
    {
        element = BlocklyUtil.applyNameSpace(element);
        object obj = new object();
        //string queryName = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault()?.Value;
        XElement queryNameValue = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("queryName")).FirstOrDefault();
        string queryName = null;
        if (queryNameValue != null)
        {
            XElement qnblock = queryNameValue.Element(BlocklyUtil.ns + "block");
            XElement shadowBlock = queryNameValue.Element(BlocklyUtil.ns + "shadow");
            if (qnblock != null && qnblock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(qnblock);
            }
            else if (shadowBlock != null && shadowBlock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(shadowBlock);
            }
            if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
            {
                queryName = ((BlocklyReference)obj).value.ToString();
            }
            else
            {
                queryName = obj.ToString();
            }
        }

        string selection = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("selection")).FirstOrDefault()?.Value;
        //string queryNode = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("queryNode")).FirstOrDefault()?.Value;
        XElement queryNodeValue = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("queryNode")).FirstOrDefault();
        string queryNode = null;
        if (queryNodeValue != null)
        {
            XElement qnblock = queryNodeValue.Element(BlocklyUtil.ns + "block");
            XElement shadowBlock = queryNodeValue.Element(BlocklyUtil.ns + "shadow");
            if (qnblock != null && qnblock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(qnblock);
            }
            else if (shadowBlock != null && shadowBlock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(shadowBlock);
            }
            if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
            {
                queryNode = ((BlocklyReference)obj).value.ToString();
            }
            else
            {
                queryNode = obj.ToString();
            }
        }

        string condition = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("condition")).FirstOrDefault()?.Value;
        //string a = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("A")).FirstOrDefault()?.Value;
        XElement aNodeValue = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("nodeName")).FirstOrDefault();
        string a = null;
        if (aNodeValue != null)
        {
            XElement qnblock = aNodeValue.Element(BlocklyUtil.ns + "block");
            XElement shadowBlock = aNodeValue.Element(BlocklyUtil.ns + "shadow");
            if (qnblock != null && qnblock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(qnblock);
            }
            else if (shadowBlock != null && shadowBlock.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(shadowBlock);
            }
            if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
            {
                a = ((BlocklyReference)obj).value.ToString();
            }
            else
            {
                a = obj.ToString();
            }
        }

        string logicCompareList = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("logicCompareList")).FirstOrDefault()?.Value;
        //XElement block = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        XElement block = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("B")).FirstOrDefault();
        string b = "";
        if (block != null)
        {
            XElement bBlock = block.Element(BlocklyUtil.ns + "block");
            if (bBlock == null)
            {
                //may be a shadow case
                bBlock = block.Element(BlocklyUtil.ns + "shadow");
            }
            if (bBlock != null)
            {
                obj = eventObj.parseBlock(bBlock);
            }
        }

        if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
        {
            BlocklyReference br = new BlocklyReference();
            br = (BlocklyReference)obj;
            b = (string)br.value;
        }
        else
        {
            b = (string)obj.ToString();
        }

        //lets prepare querypath
        string queryPath = "$..*";
        if (condition != null && condition.Trim().Length > 0)
        {
            if (condition.Equals("sibling", StringComparison.OrdinalIgnoreCase))
            {
                queryPath = queryPath + "[?(@.['" + a + "']" + getLogicOperator(logicCompareList)
                    + "'" + b + "')]" + ".['" + queryNode + "']";
                //queryPath = queryPath + "[?(@." + a + getLogicOperator(logicCompareList) + "\'" + b + "\')]" + "." + queryNode;
                //Debug.Log("<color=red> @@@@@@@@@@  queryPath : </color>" + queryPath);
            }
            else if (condition.Equals("parent", StringComparison.OrdinalIgnoreCase))
            {
                //TODO Not yes supported by jsonpath, need to find better ways
                queryPath = queryPath + "['" + b + "'].['" + queryNode + "']";

            }
            else if (condition.Equals("child", StringComparison.OrdinalIgnoreCase))
            {
                //queryPath = queryPath + queryNode + "[?(@." + a + getLogicOperator(logicCompareList) + "'" + b + "')]";
                queryPath = queryPath + "['" + queryNode + "'][?(@.['" + a + "']"
                    + getLogicOperator(logicCompareList) + "'" + b + "')]";
            }
        }
        else
        {
            queryPath = queryPath + "['" + queryNode + "']";
            //queryPath = queryPath + queryNode;
        }
        return queryPath;
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

    private object parseNewUpdateProperty(XElement element)
    {
        object obj = new object();
        element = BlocklyUtil.applyNameSpace(element);
        string elementId = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("ElementId")).FirstOrDefault()?.Value;
        string propertyId = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("propertyId")).FirstOrDefault()?.Value;
        if (elementId != null && propertyId != null)
        {
            obj = getUIElementProperty(elementId, propertyId);
        }
        return obj;
    }

    private object parseUpdateProperty(XElement element)
    {
        element = BlocklyUtil.applyNameSpace(element);
        object obj = new object();
        string elementId = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("ElementId")).FirstOrDefault()?.Value;
        string propertyId = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("propertyId")).FirstOrDefault()?.Value;
        string subProperty = null;
        if (propertyId != null && propertyId.Equals("Border", StringComparison.OrdinalIgnoreCase))
        {
            subProperty = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("borderOptions")).FirstOrDefault()?.Value;
        }
        else if (propertyId != null && propertyId.Equals("Draggable", StringComparison.OrdinalIgnoreCase))
        {
            subProperty = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("dragOptions")).FirstOrDefault()?.Value;
        }
        else if (propertyId != null && propertyId.Equals("Ranges", StringComparison.OrdinalIgnoreCase))
        {
            this.popupImpl.updatePopupGaugeRange(element);
        }
        if (subProperty != null)
        {
            propertyId = propertyId + "-" + subProperty;
        }
        var xx = element.Attributes();
        string id = element.Element(BlocklyUtil.ns + "block").Attribute("id").Value.ToString();
        //XElement block = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        XElement block = BlocklyUtil.getValueBlock(element);
        obj = eventObj.parseBlock(block);
        updateUIElementProperty(elementId, propertyId, obj, id);
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseCallElement(XElement element)
    {
        object obj = new object();
        string elementId = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("ElementId")).FirstOrDefault()?.Value;
        string propertyId = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("callPopUpProperty")).FirstOrDefault()?.Value;
        this.popupImpl.executeCallBlock(elementId, propertyId, element);
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseSendElement(XElement element)
    {
        object obj = new object();
        string recipient = null;
        string subject = null;
        string message = null;
        //string elementId = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("ElementId")).FirstOrDefault()?.Value;
        //string carrierId = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("CarrierId")).FirstOrDefault()?.Value;
        XElement recipientBlock = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("Recipient")).FirstOrDefault();
        XElement subjectBlock = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("Subject")).FirstOrDefault();
        XElement messageBlock = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("Message")).FirstOrDefault();

        recipientBlock = BlocklyUtil.getValueBlockFromValue(recipientBlock);
        obj = eventObj.parseBlock(recipientBlock);
        recipient = BlocklyUtil.getStringFromObj(obj);

        subjectBlock = BlocklyUtil.getValueBlockFromValue(subjectBlock);
        obj = eventObj.parseBlock(subjectBlock);
        subject = BlocklyUtil.getStringFromObj(obj);

        messageBlock = BlocklyUtil.getValueBlockFromValue(messageBlock);
        obj = eventObj.parseBlock(messageBlock);
        message = BlocklyUtil.getStringFromObj(obj);

        //if (elementId.Equals("Text"))
        //{
        //    recipient = BlocklyUtil.getEamilIdForPhoneNumber(recipient, carrierId);
        //}

        var content = "{ \"recipient\" : \"" + recipient + "\"," + "\"subject\" : \"" + subject + "\"," +
            "\"message\" : \"" + message + "\"}";
        var encoding = new System.Text.UTF8Encoding();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        headers.Add("Accept", "application/json");
        String api = GlobalVariables.Send_Mail; //TODO LAX : put all api referneces to one single place
        var RestResponse = WebFunctions.PostHeader(api, encoding.GetBytes(content), headers);
        var targetImageObjects = JsonConvert.DeserializeObject<object>(RestResponse.text);

        //try
        //{
        //    MailMessage mail = new MailMessage();
        //    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

        //    mail.From = new MailAddress("instantar.status@gmail.com");
        //    mail.To.Add(recipient);
        //    mail.Subject = subject;
        //    mail.Body = message;

        //    SmtpServer.Port = 587;
        //    SmtpServer.Credentials = new System.Net.NetworkCredential("instantar.status@gmail.com", "instantar@123");
        //    SmtpServer.EnableSsl = true;

        //    SmtpServer.Send(mail);
        //}
        //catch (Exception ex)
        //{
        //    //TODO LAX : not sure what to do here
        //}
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseElements(XElement element)
    {
        return element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("ElementId")).FirstOrDefault()?.Value;
    }

    private object parseWaitElement(XElement element)
    {
        object obj = new object();
        XElement block = element.Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        if (block == null)
        {
            //check if this is the case of shadow block
            block = element.Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "shadow");
        }
        float sec = 0;
        obj = eventObj.parseBlock(block);
        if (obj != null)
        {
            if (obj.GetType().Equals(typeof(BlocklyReference)))
            {
                sec = float.Parse(((BlocklyReference)obj).value.ToString());
            }
            else
            {
                sec = float.Parse(obj.ToString());
            }
            sec = sec * 1000;
        }
        //TODO : Lax to consult Jitu to implement wait for given obj seconds
        //StartCoroutine(BlocklyUtil.WaitForSecondsBlockly(sec));
        Thread.Sleep(Convert.ToInt32(sec));
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseUITableReport(XElement element)
    {
        object obj = new object();
        element = BlocklyUtil.applyNameSpace(element);

        List<Tabel_DescriptionItem> tdiList = new List<Tabel_DescriptionItem>();
        List<SelectionsItem> siList = new List<SelectionsItem>();
        List<DataItem> dList = new List<DataItem>();
        foreach (ARReportInfo1 rep in PopupButtons.reportDetails)
        {
            List<ColumnValue> cvList = rep.rowDetails;
            List<Group> grpList = new List<Group>();
            foreach (ColumnValue cv in cvList)
            {
                Debug.Log("<color=purple> PopupButtons report, key : " + cv.columnName + " and the value : " + cv.columnValue + "</color>");
                if (!tdiList.Any(n => n.Name == cv.columnName))
                {
                    Tabel_DescriptionItem tdi = new Tabel_DescriptionItem();
                    tdi.Name = cv.columnName;
                    tdi.Type = cv.columnName.GetType().ToString();
                    tdiList.Add(tdi);
                }

                if (!siList.Any(n => n.Name == cv.columnName))
                {
                    SelectionsItem si = new SelectionsItem();
                    si.Name = cv.columnName;
                    si.Title = cv.columnName;
                    si.Width = 0;
                    si.Sortable = true;
                    siList.Add(si);
                }

                Group grp = new Group();
                grp.ColumnName = cv.columnName;
                grp.ColumnData = cv.columnValue;
                grpList.Add(grp);
            }
            DataItem di = new DataItem();
            di.GroupList = grpList;
            dList.Add(di);
        }
        Tabel_Description td = new Tabel_Description();
        td.ColumnSize = 2;
        td.DataStructure = tdiList;
        Selections sel = new Selections();
        sel.SelectedColumns = siList;
        Data data = new Data();
        //DataItem di = new DataItem();
        //di.GroupList = grpList;
        //List<DataItem> diList = new List<DataItem>() { di };
        data.Rows = PopupButtons.reportDetails.Count;
        data.DataList = dList;
        TableReport tr = new TableReport(td, sel, data);
        string json = JsonUtility.ToJson(tr);
        Debug.Log("<color=purple> PopupButtons report, json : " + json + "</color>");

        PopupUtilities.makePopupTable(null, null, json, true, null);
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseFieldPopOverVideo(XElement element)
    {
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        XElement popOverElement = element.Element(BlocklyUtil.ns + "block");
        if (popOverElement == null)
        {
            //cound be a case of shadow
            popOverElement = element.Element(BlocklyUtil.ns + "shadow");
        }
        if (popOverElement != null)
        {
            string url = popOverElement.Element(BlocklyUtil.ns + "field").Value;
            return url;
        }
        else
        {
            return obj;
        }
    }

    private void updateUIElementProperty(string elementId, string propertyId, object value, string id)
    {
        GameObject gameObj = GameObject.Find(elementId);
        //if (elementId.StartsWith("PopUpYesNo", StringComparison.OrdinalIgnoreCase))
        //{
        //    BlocklyEvents.popupId = id;
        //    updatePopUpYesNo(propertyId, value);
        //}
        //else if (elementId.Equals("PopUpText", StringComparison.OrdinalIgnoreCase))
        //{
        //    BlocklyEvents.popupId = id;
        //    updatePopupText(propertyId, value);
        //}
        //else if (elementId.Equals("PopUpGauge", StringComparison.OrdinalIgnoreCase))
        //{
        //    BlocklyEvents.popupId = id;
        //    updatePopupGauge(propertyId, value);
        //}
        //else if (elementId.Equals("PopUpTable", StringComparison.OrdinalIgnoreCase))
        //{
        //    BlocklyEvents.popupId = id;
        //    updatePopupTable(propertyId, value);
        //}
        //else if (elementId.Equals("PopUpVideo", StringComparison.OrdinalIgnoreCase))
        //{
        //    BlocklyEvents.popupId = id;
        //    updatePopupVideo(propertyId, value);
        //}
        if (elementId.Equals("PopUpYesNo") || elementId.Equals("PopUpText") ||
            elementId.Equals("PopUpGauge") || elementId.Equals("PopUpTable") || elementId.Equals("PopUpVideo"))
        {
            this.popupImpl.updatePopUpBlocksProperty(elementId, propertyId, value, id);
        }
        else if (gameObj != null)
        {
            if (elementId.StartsWith("panel", StringComparison.OrdinalIgnoreCase))
            {
                updatePanel(gameObj, propertyId, value);
            }
            else if (elementId.StartsWith("button", StringComparison.OrdinalIgnoreCase)
                && gameObj.GetComponent<Button>() != null)
            {
                //if (elementId.Equals("button-10", StringComparison.OrdinalIgnoreCase))
                //{
                //    //TODO LAX Strictly for testing only, remove it asap after testing
                //    updatePopUp(gameObj, propertyId, value);
                //}
                updateButton(gameObj.GetComponent<Button>(), propertyId, value);
            }
            else if (elementId.StartsWith("dropdown", StringComparison.OrdinalIgnoreCase)
                && gameObj.GetComponent<Dropdown>() != null)
            {
                updateDropdwon(gameObj.GetComponent<Dropdown>(), propertyId, value);
            }
            else if (elementId.StartsWith("inputfield", StringComparison.OrdinalIgnoreCase)
                && gameObj.GetComponent<InputField>() != null)
            {
                updateInputField(gameObj.GetComponent<InputField>(), propertyId, value);
            }
            else if (elementId.StartsWith("text", StringComparison.OrdinalIgnoreCase)
                && gameObj.GetComponent<Text>() != null)
            {
                updateText(gameObj.GetComponent<Text>(), propertyId, value);
            }
            else if (elementId.StartsWith("slider", StringComparison.OrdinalIgnoreCase)
                && gameObj.GetComponent<Slider>() != null)
            {
                updateSlider(gameObj.GetComponent<Slider>(), propertyId, value);
            }
            else if (elementId.StartsWith("toggle", StringComparison.OrdinalIgnoreCase)
                && gameObj.GetComponent<Toggle>() != null)
            {
                updateToggle(gameObj.GetComponent<Toggle>(), propertyId, value);
            }
            else if (elementId.StartsWith("canvas", StringComparison.OrdinalIgnoreCase))
            {
                updateCanvas(gameObj, propertyId, value);
            }
            //else if (elementId.StartsWith("PopUpYesNo", StringComparison.CurrentCulture))
            //{
            //    //TODO : LAX implement this
            //    updatePopUp(gameObj, propertyId, value);
            //}
        }
    }

    private void updateCanvas(GameObject canvas, string propertyId, object value)
    {

        Debug.Log("<color=red> ################## Inside updateCanvas: ,propertyId is: </color>" + propertyId);
        Debug.Log("<color=red> ################## Inside updateCanvas: ,value is: </color>" + value);
        switch (propertyId)
        {
            case "Color":
                Image img = canvas.GetComponent<Image>();
                img.color = BlocklyUtil.getColor((string)value);
                break;

            case "isVisible":
                bool active = bool.Parse(value.ToString());
                if (active)
                {
                    canvas.transform.localScale = Vector3.one;

                    foreach (string canvasName in GlobalVariables.CanvasNames.Keys)
                    {
                        Debug.Log("<color=red> @@@@ canvasName   : </color>" + canvasName);
                        if (canvasName.Equals(canvas.name))
                        {
                            continue;
                        }
                        else
                        {
                            GameObject canvasObject = GameObject.Find(canvasName);
                            canvasObject.transform.localScale = Vector3.zero;
                        }

                    }
                }
                else
                {
                    canvas.transform.localScale = Vector3.zero;
                    string homeScreenName = null;
                    bool anyVisible = false;
                    foreach (string canvasName in GlobalVariables.CanvasNames.Keys)
                    {
                        Debug.Log("<color=red> @@@@ canvasName   : </color>" + canvasName);
                        // string homeScreenName;
                        if (GlobalVariables.CanvasNames[canvasName])
                        {
                            homeScreenName = canvasName;
                            Debug.Log("<color=green> @@@@ homeScreenName   : </color>" + homeScreenName);
                        }


                        GameObject canvasObject = GameObject.Find(canvasName);
                        if (canvasObject.transform.localScale == Vector3.one)
                        {
                            Debug.Log("<color=green> @@@@ Already VISIBLE is   : </color>" + canvasName);
                            anyVisible = true;
                            break;
                        }
                    }
                    if (!anyVisible)
                    {
                        GameObject canvasObject = GameObject.Find(homeScreenName);
                        canvasObject.transform.localScale = Vector3.one;
                        Debug.Log("<color=green> @@@@ MAKING HOMESCREEN VISIBLE  : </color>" + homeScreenName);
                    }



                }
                break;
            default:
                updateCommonProperties(canvas, propertyId, value);
                break;
        }
    }

    private void updatePanel(GameObject panel, string propertyId, object value)
    {
        switch (propertyId)
        {
            case "Color":
                Image img = panel.GetComponent<Image>();
                img.color = BlocklyUtil.getColor((string)value);
                break;
            default:
                updateCommonProperties(panel, propertyId, value);
                break;
        }
    }

    private void updateButton(Button button, string propertyId, object value)
    {
        switch (propertyId)
        {
            case "Color":
                ColorBlock theColor = button.colors;
                theColor.normalColor = BlocklyUtil.getColor((string)value);
                theColor.highlightedColor = theColor.normalColor;
                button.GetComponent<Button>().colors = theColor;
                break;
            case "ButtonSpecificProperties":
                //Do button specific stuff
                break;
            default:
                updateCommonProperties(button.gameObject, propertyId, value);
                break;
        }
    }

    private void updateDropdwon(Dropdown dropdown, string propertyId, object value)
    {
        switch (propertyId)
        {
            case "Color":
                ColorBlock theColor = dropdown.colors;
                theColor.normalColor = BlocklyUtil.getColor((string)value);
                theColor.highlightedColor = theColor.normalColor;
                dropdown.GetComponent<Dropdown>().colors = theColor;
                break;
            case "OptionList":
                List<string> listOptions = getDropdownOptions(value);
                dropdown.ClearOptions();
                //dropdown.value = 0;
                dropdown.AddOptions(listOptions);
                dropdown.value = 0;
                dropdown.Select(); // optional
                dropdown.RefreshShownValue();
                break;
            default:
                updateCommonProperties(dropdown.gameObject, propertyId, value);
                break;
        }
    }

    private void updateInputField(InputField inputField, string propertyId, object value)
    {
        switch (propertyId)
        {
            case "Color":
                ColorBlock theColor = inputField.colors;
                theColor.normalColor = BlocklyUtil.getColor((string)value);
                theColor.highlightedColor = theColor.normalColor;
                inputField.GetComponent<InputField>().colors = theColor;
                break;
            case "InputFieldSpecificProperties":
                //Do inputField specific stuff
                break;
            default:
                updateCommonProperties(inputField.gameObject, propertyId, value);
                break;
        }
    }

    private void updateSlider(Slider slider, string propertyId, object value)
    {
        switch (propertyId)
        {
            case "Color":
                ColorBlock theColor = slider.colors;
                theColor.normalColor = BlocklyUtil.getColor((string)value);
                theColor.highlightedColor = theColor.normalColor;
                slider.GetComponent<Slider>().colors = theColor;
                break;
            case "SliderSpecificProperties":
                //Do slider specific stuff
                break;
            default:
                updateCommonProperties(slider.gameObject, propertyId, value);
                break;
        }
    }

    private void updateToggle(Toggle toggle, string propertyId, object value)
    {
        switch (propertyId)
        {
            case "Color":
                ColorBlock theColor = toggle.colors;
                theColor.normalColor = BlocklyUtil.getColor((string)value);
                theColor.highlightedColor = theColor.normalColor;
                toggle.GetComponent<Toggle>().colors = theColor;
                break;
            case "ToggleSpecificProperties":
                //Do toggle specific stuff
                break;
            default:
                updateCommonProperties(toggle.gameObject, propertyId, value);
                break;
        }
    }

    private void updateText(Text text, string propertyId, object value)
    {
        switch (propertyId)
        {
            case "TextFieldSpecificProperties":
                //Do text specific stuff
                break;
            default:
                updateCommonProperties(text.gameObject, propertyId, value);
                break;
        }
    }

    private void updateCommonProperties(GameObject gObj, string propertyId, object value)
    {
        if (value.GetType().Equals(typeof(BlocklyReference)))
        {
            value = ((BlocklyReference)value).value;
        }
        switch (propertyId)
        {
            case "Scale":
                gObj.transform.localScale = new Vector3(float.Parse(value.ToString()), float.Parse(value.ToString()), gObj.transform.localScale.z);
                break;
            case "Scale_X":
                gObj.transform.localScale = new Vector3(float.Parse(value.ToString()), gObj.transform.localScale.y, gObj.transform.localScale.z);
                break;
            case "Scale_Y":
                gObj.transform.localScale = new Vector3(gObj.transform.localScale.x, float.Parse(value.ToString()), gObj.transform.localScale.z);
                break;
            case "Position_X":
                gObj.transform.localPosition = new Vector3(float.Parse(value.ToString()) * gObj.transform.parent.GetComponent<RectTransform>().rect.width,
                    gObj.transform.localPosition.y, gObj.transform.localPosition.z);
                break;
            case "Position_Y":
                gObj.transform.localPosition = new Vector3(gObj.transform.localPosition.x,
                    float.Parse(value.ToString()) * gObj.transform.parent.GetComponent<RectTransform>().rect.height, gObj.transform.localPosition.z);
                break;
            case "Width":
                float width = float.Parse(value.ToString()) * gObj.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
                gObj.GetComponent<RectTransform>().sizeDelta = new Vector2(width, gObj.GetComponent<RectTransform>().sizeDelta.y);
                break;
            case "Height":
                float height = float.Parse(value.ToString()) * gObj.transform.parent.GetComponent<RectTransform>().sizeDelta.y;
                gObj.GetComponent<RectTransform>().sizeDelta = new Vector2(gObj.GetComponent<RectTransform>().sizeDelta.x, height);
                break;
            case "Label":
                gObj.GetComponentInChildren<Text>().text = (string)value.ToString();
                break;
            case "FontSize":
                gObj.GetComponentInChildren<Text>().fontSize = int.Parse(value.ToString());
                break;
            case "FontStyle":
                gObj.GetComponentInChildren<Text>().fontStyle = PrimaryScreenController.getFontStyle(value.ToString());
                break;
            case "isVisible":
                bool active = bool.Parse(value.ToString());
                if (active)
                {
                    gObj.transform.localScale = Vector3.one;
                }
                else
                {
                    gObj.transform.localScale = Vector3.zero;
                }
                break;
            case "ImageSource":
                String imagePath = PrimaryScreenController.saveSecondaryImage((string)value);
                if (imagePath != null && imagePath.Length > 0)
                {
                    //TODO LAX : Slice it or else it will go all voer the point
                    //also make it best fit and then horizontalwrap should Wrap
                    //Sprite targetImageSprite = IMG2Sprite.LoadNewSprite(imagePath, 100f, SpriteMeshType.Tight);
                    Image img = gObj.GetComponent<Image>();
                    img.sprite = PrimaryScreenController.loadNewSprite(imagePath);
                    //img.sprite = targetImageSprite;
                    img.type = Image.Type.Simple;
                    //TO-Do Following two setting are hardcoded, need to be fixed
                    img.preserveAspect = false;
                    img.color = BlocklyUtil.getColor("rgba(255,255,255,255)");

                    if (gObj.GetComponent<Button>() != null)
                    {
                        Button button = gObj.GetComponent<Button>();
                        ColorBlock theColor = button.GetComponent<Button>().colors;
                        theColor.normalColor = BlocklyUtil.getColor("rgba(255,255,255,255)");
                        theColor.pressedColor = theColor.normalColor;
                        theColor.highlightedColor = Color.Lerp(theColor.normalColor, Color.black, .3f);
                        theColor.disabledColor = Color.Lerp(theColor.normalColor, Color.white, .2f);
                        button.GetComponent<Button>().colors = theColor;
                    }
                }
                break;
            default:
                Console.WriteLine("Default case");
                break;
        }
    }

    private object getUIElementProperty(string elementId, string propertyId)
    {
        GameObject gameObj = GameObject.Find(elementId);
        object obj = null;
        if (elementId.Equals("PopUpYesNo", StringComparison.OrdinalIgnoreCase))
        {
            gameObj = GameObject.Find("UIPopupYesNo");
        }
        else if (elementId.Equals("PopUpText", StringComparison.OrdinalIgnoreCase))
        {
            gameObj = GameObject.Find("UIPopupText");
        }
        //GameObject gameObj = GameObject.Find(elementId);
        if (elementId.StartsWith("PopUpYesNo", StringComparison.OrdinalIgnoreCase))
        {
            obj = popupImpl.getPopUpYesNoProperties(gameObj, propertyId);
        }
        else if (elementId.Equals("PopUpText", StringComparison.OrdinalIgnoreCase))
        {
            obj = popupImpl.getPopUpTextProperties(gameObj, propertyId);
        }
        else if (gameObj != null)
        {
            if (elementId.StartsWith("panel", StringComparison.OrdinalIgnoreCase))
            {
                obj = getPanelProperties(gameObj, propertyId);
            }
            else if (elementId.StartsWith("button", StringComparison.OrdinalIgnoreCase)
                && gameObj.GetComponent<Button>() != null)
            {
                //if (elementId.Equals("button-10", StringComparison.OrdinalIgnoreCase))
                //{
                //    //TODO LAX Strictly for testing only, remove it asap after testing
                //    updatePopUp(gameObj, propertyId, value);
                //}
                obj = getButtonProperties(gameObj.GetComponent<Button>(), propertyId);
            }
            else if (elementId.StartsWith("dropdown", StringComparison.OrdinalIgnoreCase)
                && gameObj.GetComponent<Dropdown>() != null)
            {
                obj = getDropdownProperties(gameObj.GetComponent<Dropdown>(), propertyId);
            }
            else if (elementId.StartsWith("inputfield", StringComparison.OrdinalIgnoreCase)
                && gameObj.GetComponent<InputField>() != null)
            {
                obj = getInputFieldProperties(gameObj.GetComponent<InputField>(), propertyId);
            }
            else if (elementId.StartsWith("text", StringComparison.OrdinalIgnoreCase)
                && gameObj.GetComponent<Text>() != null)
            {
                obj = getTextProperties(gameObj.GetComponent<Text>(), propertyId);
            }
            else if (elementId.StartsWith("canvas", StringComparison.OrdinalIgnoreCase))
            {
                obj = getCanvasProperties(gameObj, propertyId);
            }

            //else if (elementId.StartsWith("PopUpYesNo", StringComparison.CurrentCulture))
            //{
            //    //TODO : LAX implement this
            //    updatePopUp(gameObj, propertyId, value);
            //}
        }
        return obj;
    }

    private object getCanvasProperties(GameObject gameObj, string propertyId)
    {
        Debug.Log("<color=red> ################## Inside getCanvasProperties: ,propertyId is: </color>" + propertyId);
        object obj = new object();
        switch (propertyId)
        {
            case "Color":
                obj = gameObj.GetComponent<Image>().color.ToString();
                break;
            default:
                obj = getCommonGameObjectProperty(gameObj, propertyId);
                break;
        }
        return obj;
    }

    private object getPanelProperties(GameObject gameObj, string propertyId)
    {
        object obj = new object();
        switch (propertyId)
        {
            case "Color":
                obj = gameObj.GetComponent<Image>().color.ToString();
                break;
            default:
                obj = getCommonGameObjectProperty(gameObj, propertyId);
                break;
        }
        return obj;
    }

    private object getButtonProperties(Button button, string propertyId)
    {
        object obj = new object();
        switch (propertyId)
        {
            case "Color":
                ColorBlock theColor = button.GetComponent<Button>().colors;
                obj = ColorUtility.ToHtmlStringRGB(theColor.normalColor);
                break;
            default:
                obj = getCommonGameObjectProperty(button.gameObject, propertyId);
                break;
        }
        return obj;
    }

    private object getDropdownProperties(Dropdown dropdown, string propertyId)
    {
        object obj = new object();
        switch (propertyId)
        {
            case "Color":
                obj = dropdown.GetComponent<Image>().color.ToString();
                break;
            case "SelectedItem":
                //obj = dropdown.GetComponent<Image>().color.ToString();
                obj = dropdown.options[dropdown.value].text;
                break;
            default:
                obj = getCommonGameObjectProperty(dropdown.gameObject, propertyId);
                break;
        }
        return obj;
    }

    private object getInputFieldProperties(InputField inputField, string propertyId)
    {
        object obj = new object();
        switch (propertyId)
        {
            case "Color":
                ColorBlock theColor = inputField.GetComponent<Button>().colors;
                obj = theColor.normalColor.ToString();
                break;
            default:
                obj = getCommonGameObjectProperty(inputField.gameObject, propertyId);
                break;
        }
        return obj;
    }

    private object getTextProperties(Text text, string propertyId)
    {
        object obj = new object();
        switch (propertyId)
        {
            case "Color":
                obj = text.color.ToString();
                break;
            default:
                obj = getCommonGameObjectProperty(text.gameObject, propertyId);
                break;
        }
        return obj;
    }

    private object getCommonGameObjectProperty(GameObject gObj, string propertyId)
    {
        object obj = new object();
        switch (propertyId)
        {
            case "Scale_X":
                obj = gObj.transform.localScale.x;
                break;
            case "Scale_Y":
                obj = gObj.transform.localScale.y;
                break;
            case "Scale":
                obj = gObj.transform.localScale;
                break;
            case "Position_X":
                obj = gObj.transform.localPosition.x;
                break;
            case "Position_Y":
                obj = gObj.transform.localPosition.y;
                break;
            case "Width":
                obj = gObj.GetComponent<RectTransform>().sizeDelta.x;
                break;
            case "Height":
                obj = gObj.GetComponent<RectTransform>().sizeDelta.y;
                break;
            case "Label":
                obj = gObj.GetComponentInChildren<Text>().text;
                break;
            case "FontSize":
                obj = gObj.GetComponentInChildren<Text>().fontSize;
                break;
            case "FontStyle":
                obj = gObj.GetComponentInChildren<Text>().fontStyle.ToString();
                break;
            case "isVisible":
                obj = gObj.activeSelf;
                break;
            case "Image":
                obj = gObj.GetComponent<Image>();
                break;
            default:
                Console.WriteLine("Default case");
                break;
        }
        return obj;
    }

    private List<string> getDropdownOptions(object value)
    {
        List<string> listOptions = new List<string>();
        if (value != null && value.GetType().Equals(typeof(BlocklyReference)))
        {
            BlocklyReference bRef = (BlocklyReference)value;
            if (bRef.value != null)
            {
                if (bRef.type.Equals("list") || bRef.type.Equals("ARQueryAll") || bRef.type.Equals("ARQuery"))
                {
                    if (bRef.value.GetType().Equals(typeof(List<string>)))
                    {
                        listOptions = (List<string>)bRef.value;
                    }
                    else if (bRef.value.GetType().Equals(typeof(List<object>)))
                    {
                        listOptions = ((List<object>)bRef.value).Select(s => (string)s).ToList();
                    }
                    else
                    {
                        //TODO LAX : fix it later with more appropriate implementation
                        listOptions = ((List<object>)bRef.value).Select(s => (string)s).ToList();
                    }
                }
                else if (bRef.type.Equals("text"))
                {
                    listOptions.Add(bRef.value.ToString());
                }
                else
                {
                    listOptions.Add(bRef.value.ToString());
                }
            }
        }
        return listOptions;
    }

}
