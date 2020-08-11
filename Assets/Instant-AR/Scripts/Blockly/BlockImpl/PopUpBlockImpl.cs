using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class PopUpBlockImpl
{
    public BlocklyEvents eventObj {get; set;}

    public PopUpBlockImpl()
    {
    }

    public void executeCallBlock(string elementId, string propertyId, XElement element)
    {
        switch (elementId)
        {
            case "PopUpYesNo":
                executePopUpYesNo(propertyId);
                break;
            case "PopUpText":
                executePopUpText(propertyId);
                break;
            case "PopUpTable":
                executePopUpTable(propertyId, element);
                break;
            case "PopUpVideo":
                executePopUpVideo(propertyId);
                break;
            case "PopUpGauge":
                executePopUpGauge(propertyId);
                break;
            default:
                break;
        }
    }

    private void executePopUpYesNo(string propertyId)
    {
        PopupYesNoPOCO pY = BlocklyEvents.pYesNo;
        switch (propertyId)
        {
            case "Show":
                //PopupUtilities.makePopupYesNo(pY.callingObject, pY.parentContainer, pY.popupMessage, pY.destroySiblings, null);
                PopupUtilities.makePopupYesNo(pY.callingObject, null, pY.popupMessage, pY.destroySiblings, null);
                break;
            case "Destroy":
                break;
            case "Close":
                break;
            default:
                break;
        }
    }

    private void executePopUpText(string propertyId)
    {
        PopupYesNoPOCO pY = BlocklyEvents.pYesNo;
        switch (propertyId)
        {
            case "Show":
                PopupUtilities.makePopupYesNo(pY.callingObject, pY.parentContainer, pY.popupMessage, pY.destroySiblings, null);
                break;
            case "Destroy":
                break;
            case "Close":
                break;
            default:
                break;
        }
    }

    private void executePopUpTable(string propertyId, XElement element)
    {
        switch (propertyId)
        {
            case "Show":
                parseUITableReport(element);
                break;
            case "Destroy":
                break;
            case "Close":
                break;
            default:
                break;
        }
    }

    private void executePopUpVideo(string propertyId)
    {
        PopupVideoPOCO p = BlocklyEvents.pVideo;
        switch (propertyId)
        {
            case "Show":
                //PopupUtilities.makePopupVideo(null, null, true, true, "https://dl.dropbox.com/s/t17ie1xn33wqw0u/InstantAR_F02.mp4", true, Color.green, 5);
                PopupUtilities.makePopupVideo(p.callingObject, p.parentContainer, p.destroySiblings,
                    p.preserveAspect, p.videoURL, p.playOnStart, p.borderColor, p.borderWidth);
                break;
            case "Play":
                PopupUtilities.makePopupVideo(p.callingObject, p.parentContainer, p.destroySiblings,
                    p.preserveAspect, p.videoURL, p.playOnStart, p.borderColor, p.borderWidth);
                break;
            case "Pause":
                break;
            case "Stop":
                break;
            case "Destroy":
                break;
            case "Close":
                break;
            default:
                break;
        }
    }

    private void executePopUpGauge(string propertyId)
    {
        GaugePOCO g = BlocklyEvents.pGauge;
        switch (propertyId)
        {
            case "Show":
                PopupUtilities.makePopupGauge(g);
                break;
            case "Destroy":
                
                break;
            case "Close":
                
                break;
            default:
                break;
        }
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

    public void updatePopUpBlocksProperty(string elementId, string propertyId, object value, string id)
    {
        BlocklyEvents.popupId = id;
        string strValue = BlocklyUtil.getStringFromObj(value);
        if (elementId.StartsWith("PopUpYesNo", StringComparison.OrdinalIgnoreCase))
        {
            updatePopUpYesNo(propertyId, strValue);
        }
        else if (elementId.Equals("PopUpText", StringComparison.OrdinalIgnoreCase))
        {
            updatePopupText(propertyId, strValue);
        }
        else if (elementId.Equals("PopUpGauge", StringComparison.OrdinalIgnoreCase))
        {
            updatePopupGauge(propertyId, strValue);
        }
        else if (elementId.Equals("PopUpTable", StringComparison.OrdinalIgnoreCase))
        {
            updatePopupTable(propertyId, strValue);
        }
        else if (elementId.Equals("PopUpVideo", StringComparison.OrdinalIgnoreCase))
        {
            updatePopupVideo(propertyId, strValue);
        }
    }

    public void updatePopupGaugeRange1(string color, string start, string end)
    {
        List<GaugePOCO.GaugeRange> ranges = BlocklyEvents.pGauge.gaugeRanges;
        if (ranges == null)
        {
            ranges = new List<GaugePOCO.GaugeRange>();
        }
        Color gColor = BlocklyUtil.getColor(color);
        float gStart = float.Parse(start.ToString());
        float gEnd = float.Parse(end.ToString());
        GaugePOCO.GaugeRange gRange = new GaugePOCO.GaugeRange();
        gRange.color = gColor;
        gRange.start = gStart;
        gRange.end = gEnd;
        ranges.Add(gRange);
    }

    public void updatePopupGaugeRange(XElement element)
    {
        object obj = null;
        Color color = Color.blue;
        float start = 0;
        float end = 0;
        List<ChartRange> ranges = BlocklyEvents.pGauge.ChartRanges;
        ranges = ranges ?? new List<ChartRange>();
        string rangeNumber = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("rangeNo")).FirstOrDefault()?.Value;

        XElement block = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("VALUE")).FirstOrDefault();
        if (block != null)
        {
            block = BlocklyUtil.getValueBlockFromValue(block);
            if (block != null)
            {
                obj = eventObj.parseBlock(block);
                if (obj != null)
                {
                    color = BlocklyUtil.getColor(BlocklyUtil.getStringFromObj(obj));
                }
            }
        }

        block = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("start")).FirstOrDefault();
        if (block != null)
        {
            block = BlocklyUtil.getValueBlockFromValue(block);
            if (block != null)
            {
                obj = eventObj.parseBlock(block);
                if (obj != null)
                {
                    start = float.Parse(BlocklyUtil.getStringFromObj(obj));
                }
            }
        }

        block = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("end")).FirstOrDefault();
        if (block != null)
        {
            block = BlocklyUtil.getValueBlockFromValue(block);
            if (block != null)
            {
                obj = eventObj.parseBlock(block);
                if (obj != null)
                {
                    end = float.Parse(BlocklyUtil.getStringFromObj(obj));
                }
            }
        }

        ChartRange gRange = new ChartRange();
        gRange.Colour = color;
        gRange.StartValue = start;
        gRange.EndValue = end;
        ranges.Add(gRange);
        BlocklyEvents.pGauge.ChartRanges = ranges;
    }

    private void updatePopUpYesNo(string propertyId, object value)
    {
        updatePopUpYesNoCommonProperties(propertyId, value);
    }

    private void updatePopupText(string propertyId, object value)
    {
        updatePopUpYesNoCommonProperties(propertyId, value);
    }

    private void updatePopupGauge(string propertyId, object value)
    {
        if (BlocklyEvents.pGauge.callingObject == null)
        {
            BlocklyEvents.pGauge.callingObject = eventObj.eventGameObject;
        }
        switch (propertyId)
        {
            case "Border":
                BlocklyEvents.pGauge.isBorder = bool.Parse(value.ToString());
                break;
            case "Border-Color":
                BlocklyEvents.pGauge.borderColor = BlocklyUtil.getColor((string)value);
                break;
            case "Border-Width":
                BlocklyEvents.pGauge.borderWidth = float.Parse(value.ToString());
                break;
            case "Draggable":
                BlocklyEvents.pGauge.isDraggable = bool.Parse(value.ToString());
                break;
            case "Draggable-DragArea":
                BlocklyEvents.pGauge.dragArea = (string)value.ToString();
                break;
            case "TouchSesitive":
                BlocklyEvents.pGauge.touchSesitive = bool.Parse(value.ToString());
                break;
            case "ImageSource":
                BlocklyEvents.pGauge.imageSource = (string)value.ToString();
                break;
            case "SourceAsset":
                BlocklyEvents.pGauge.sourceAsset = (string)value.ToString();
                break;
            case "Color":
                BlocklyEvents.pGauge.color = BlocklyUtil.getColor((string)value);
                break;
            case "ParentContainer":
                GameObject gameObj = GameObject.Find(value.ToString());
                BlocklyEvents.pGauge.parentContainer = gameObj;
                break;
            case "AlignParentSize":
                BlocklyEvents.pGauge.alignParentSize = bool.Parse(value.ToString());
                break;
            case "DestroySiblings":
                BlocklyEvents.pGauge.flushOthers = bool.Parse(value.ToString());
                break;
            case "Radius":
                BlocklyEvents.pGauge.GaugeRadius = float.Parse(value.ToString());
                break;
            case "Title":
                BlocklyEvents.pGauge.m_Title = (string)value.ToString();
                break;
            case "TtitleColor":
                BlocklyEvents.pGauge.m_TitleColor = BlocklyUtil.getColor((string)value);
                break;
            case "Unit":
                BlocklyEvents.pGauge.m_Unit = (string)value.ToString();
                break;
            case "Value":
                BlocklyEvents.pGauge.GaugeValue = float.Parse(value.ToString());
                break;
            default:
                break;
        }
    }

    private void updatePopupVideo(string propertyId, object value)
    {
        if (BlocklyEvents.pVideo.callingObject == null)
        {
            BlocklyEvents.pVideo.callingObject = eventObj.eventGameObject;
        }
        switch (propertyId)
        {
            case "Border":
                BlocklyEvents.pVideo.isBorder = bool.Parse(value.ToString());
                break;
            case "Border-Color":
                BlocklyEvents.pVideo.borderColor = BlocklyUtil.getColor((string)value);
                break;
            case "Border-Width":
                BlocklyEvents.pVideo.borderWidth = float.Parse(value.ToString());
                break;
            case "Draggable":
                BlocklyEvents.pVideo.isDraggable = bool.Parse(value.ToString());
                break;
            case "Draggable-DragArea":
                BlocklyEvents.pVideo.dragArea = (string)value.ToString();
                break;
            case "TouchSesitive":
                BlocklyEvents.pVideo.touchSesitive = bool.Parse(value.ToString());
                break;
            case "ImageSource":
                BlocklyEvents.pVideo.imageSource = (string)value.ToString();
                break;
            case "SourceAsset":
                //BlocklyEvents.pVideo.sourceAsset = (string)value.ToString();
                BlocklyEvents.pVideo.videoURL = (string)value.ToString();
                PopupVideoPOCO p = BlocklyEvents.pVideo;
                PopupUtilities.makePopupVideo(p.callingObject, p.parentContainer, p.destroySiblings,
                    p.preserveAspect, BlocklyEvents.pVideo.videoURL, p.playOnStart, p.borderColor, p.borderWidth);
                break;
            case "VideoURL":
                BlocklyEvents.pVideo.videoURL = (string)value.ToString();
                break;
            case "PlayOnStart":
                BlocklyEvents.pVideo.playOnStart = bool.Parse(value.ToString());
                break;
            case "PreserveAspect":
                BlocklyEvents.pVideo.preserveAspect = bool.Parse(value.ToString());
                break;
            case "ParentContainer":
                GameObject gameObj = GameObject.Find(value.ToString());
                BlocklyEvents.pVideo.parentContainer = gameObj;
                break;
            case "AlignParentSize":
                BlocklyEvents.pVideo.alignParentSize = bool.Parse(value.ToString());
                break;
            case "DestroySiblings":
                BlocklyEvents.pVideo.destroySiblings = bool.Parse(value.ToString());
                break;
            default:
                break;
        }
    }

    private void updatePopupTable(string propertyId, object value)
    {
        switch (propertyId)
        {
            case "Border":
                BlocklyEvents.pTable.isBorder = bool.Parse(value.ToString());
                break;
            case "Border-Color":
                BlocklyEvents.pTable.borderColor = BlocklyUtil.getColor((string)value);
                break;
            case "Border-Width":
                BlocklyEvents.pTable.borderWidth = float.Parse(value.ToString());
                break;
            case "Draggable":
                BlocklyEvents.pTable.isDraggable = bool.Parse(value.ToString());
                break;
            case "Draggable-DragArea":
                BlocklyEvents.pTable.dragArea = (string)value.ToString();
                break;
            case "TouchSesitive":
                BlocklyEvents.pTable.touchSesitive = bool.Parse(value.ToString());
                break;
            case "ImageSource":
                BlocklyEvents.pTable.imageSource = (string)value.ToString();
                break;
            case "SourceAsset":
                BlocklyEvents.pTable.sourceAsset = (string)value.ToString();
                break;
            case "Color":
                BlocklyEvents.pTable.color = BlocklyUtil.getColor((string)value);
                break;
            case "Label":
                BlocklyEvents.pTable.label = (string)value.ToString();
                break;
            case "FontSize":
                BlocklyEvents.pTable.fontSize = float.Parse(value.ToString());
                break;
            case "TextColor":
                BlocklyEvents.pTable.textColor = BlocklyUtil.getColor((string)value);
                break;
            case "PopupMessage":
                BlocklyEvents.pTable.popupMessage = (string)value.ToString();
                break;
            case "PopupResponse":
                BlocklyEvents.pTable.popupResponse = (string)value.ToString();
                break;
            case "ParentContainer":
                GameObject gameObj = GameObject.Find(value.ToString());
                BlocklyEvents.pTable.parentContainer = gameObj;
                break;
            case "AlignParentSize":
                BlocklyEvents.pTable.alignParentSize = bool.Parse(value.ToString());
                break;
            case "DestroySiblings":
                BlocklyEvents.pTable.destroySiblings = bool.Parse(value.ToString());
                break;
            default:
                break;
        }
    }

    private void updatePopUpYesNoCommonProperties(string propertyId, object value)
    {
        if (BlocklyEvents.pYesNo.callingObject == null)
        {
            BlocklyEvents.pYesNo.callingObject = eventObj.eventGameObject;
        }
        switch (propertyId)
        {
            case "Border":
                BlocklyEvents.pYesNo.isBorder = bool.Parse(value.ToString());
                break;
            case "Border-Color":
                BlocklyEvents.pYesNo.borderColor = BlocklyUtil.getColor((string)value);
                break;
            case "Border-Width":
                BlocklyEvents.pYesNo.borderWidth = float.Parse(value.ToString());
                break;
            case "Draggable":
                BlocklyEvents.pYesNo.isDraggable = bool.Parse(value.ToString());
                break;
            case "Draggable-DragArea":
                BlocklyEvents.pYesNo.dragArea = (string)value.ToString();
                break;
            case "TouchSesitive":
                BlocklyEvents.pYesNo.touchSesitive = bool.Parse(value.ToString());
                break;
            case "ImageSource":
                BlocklyEvents.pYesNo.imageSource = (string)value.ToString();
                break;
            case "SourceAsset":
                BlocklyEvents.pYesNo.sourceAsset = (string)value.ToString();
                break;
            case "Color":
                BlocklyEvents.pYesNo.color = BlocklyUtil.getColor((string)value);
                break;
            case "Label":
                BlocklyEvents.pYesNo.label = (string)value.ToString();
                break;
            case "FontSize":
                BlocklyEvents.pYesNo.fontSize = float.Parse(value.ToString());
                break;
            case "TextColor":
                BlocklyEvents.pYesNo.textColor = BlocklyUtil.getColor((string)value);
                break;
            case "ParentContainer":
                GameObject gameObj = GameObject.Find(value.ToString());
                BlocklyEvents.pYesNo.parentContainer = gameObj;
                break;
            case "PopupMessage":
                BlocklyEvents.pYesNo.popupMessage = (string)value.ToString();
                break;
            case "PopupResponse":
                BlocklyEvents.pYesNo.popupResponse = (string)value.ToString();
                break;
            case "AlignParentSize":
                BlocklyEvents.pYesNo.alignParentSize = bool.Parse(value.ToString());
                break;
            case "DestroySiblings":
                BlocklyEvents.pYesNo.destroySiblings = bool.Parse(value.ToString());
                break;
            default:
                break;
        }
    }

    public object getPopUpTextProperties(GameObject gameObj, string propertyId)
    {
        object obj = new object();
        switch (propertyId)
        {
            case "PopupResponse":
                obj = BlocklyEvents.pYesNo.popupResponse;
                break;
            case "PopupMessage":
                obj = BlocklyEvents.pYesNo.popupMessage;
                break;
            default:
                break;
        }
        return obj;
    }

    public object getPopUpYesNoProperties(GameObject gameObj, string propertyId)
    {
        object obj = new object();
        switch (propertyId)
        {
            case "PopupResponse":
                obj = BlocklyEvents.pYesNo.popupResponse;
                break;
            case "PopupMessage":
                obj = BlocklyEvents.pYesNo.popupMessage;
                break;
            default:
                break;
        }
        return obj;
    }
}
