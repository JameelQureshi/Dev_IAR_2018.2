using System;
using System.Xml.Linq;

public class ARTrackingBlockImpl : IBlock
{
    BlocklyEvents eventObj;

    public object parse(BlocklyEvents eventObject, string blockType, string codeBlockName, XElement element)
    {
        object obj = null;
        this.eventObj = eventObject;

        switch (blockType)
        {
            case "ARTracking":
                if (!eventObj.firstExecution)
                {
                    obj = parseARTracking(element);
                }
                break;
            case "AppTracking":
                if (!eventObj.firstExecution)
                {
                    obj = parseAppTracking(element);
                }
                break;
            default:
                Console.WriteLine("Default case, may be simple event at the start! " +
                    "lets see what is it : " + blockType);
                break;
        }
        return obj;
    }

    private object parseARTracking(XElement element)
    {
        object obj = new object();
        element = BlocklyUtil.applyNameSpace(element);
        XElement block = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "statement").Element(BlocklyUtil.ns + "block");
        obj = eventObj.parseBlock(block);
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseAppTracking(XElement element)
    {
        object obj = new object();
        element = BlocklyUtil.applyNameSpace(element);
        XElement block = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "statement").Element(BlocklyUtil.ns + "block");
        obj = eventObj.parseBlock(block);
        eventObj.parseNextBlock(element);
        return obj;
    }
}
