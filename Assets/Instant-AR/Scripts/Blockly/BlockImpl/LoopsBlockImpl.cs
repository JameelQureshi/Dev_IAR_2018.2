using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public class LoopsBlockImpl : IBlock
{
    BlocklyEvents eventObj;
    public LoopsBlockImpl()
    {
    }

    public object parse(BlocklyEvents eventObject, string blockType, string codeBlockName, XElement element)
    {
        object obj = null;
        this.eventObj = eventObject;
        switch (blockType)
        {
            case "controls_forEach":
                obj = parseControlsForEach(element);
                break;
            case "controls_whileUntil":
                obj = parseControlsWhileUntil(element);
                break;
            case "controls_repeat_ext":
                obj = parseControlsRepeatExt(element);
                break;
            case "controls_for":
                obj = parseControlsFor(element);
                break;
            default:
                Console.WriteLine("Default case");
                break;
        }
        return obj;
    }

    private object parseControlsForEach(XElement element)
    {
        object obj = null;
        List<string> listOptions = new List<string>();
        element = BlocklyUtil.applyNameSpace(element);
        string varName = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "field").Value;
        XElement doStatements = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "statement").Element(BlocklyUtil.ns + "block");
        XElement value = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value");
        if (value != null)
        {
            XElement block = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
            if (block != null && block.Attribute("type") != null)
            {
                obj = eventObj.parseBlock(block);
            }
        }
        if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
        {
            BlocklyReference bRef = (BlocklyReference)obj;
            if (bRef.type.Equals("list") || bRef.type.Equals("ARQueryAll") || bRef.type.Equals("ARQuery"))
            {
                if (bRef.value != null)
                {
                    if (bRef.value.GetType().Equals(typeof(List<string>)))
                    {
                        listOptions = (List<string>)bRef.value;
                    }
                    else if (bRef.value.GetType().Equals(typeof(List<object>)))
                    {
                        listOptions = ((List<object>)bRef.value).Cast<string>().ToList();
                    }
                    else if (bRef.value.GetType().Equals(typeof(string)))
                    {
                        listOptions.Add(bRef.value.ToString());
                    }
                    else
                    {
                        listOptions.Add(bRef.value.ToString());
                    }
                }
            }
            else
            {
                listOptions.Add(bRef.value.ToString());
            }
        }
        else
        {
            listOptions.Add(obj.ToString());
        }
        int iterations = listOptions.Count;

        for (int i = 0; i < iterations; i++)
        {
            BlocklyReference bRef = new BlocklyReference();
            
            bRef.value = listOptions[i];
            bRef.name = varName;
            if (BlocklyEvents.blocklyReferences.ContainsKey(varName))
            {
                BlocklyEvents.blocklyReferences.Remove(varName);
            }
            BlocklyEvents.blocklyReferences.Add(varName, bRef);
            //lets iterate over the blocks now
            if (doStatements != null)
            {
                obj = eventObj.parseBlock(doStatements);
            }
        }
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseControlsWhileUntil(XElement element)
    {
        object obj = null;
        List<string> listOptions = new List<string>();
        element = BlocklyUtil.applyNameSpace(element);
        string mode = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("MODE")).FirstOrDefault()?.Value;
        XElement logicBlock = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        XElement doStatements = element.Descendants(BlocklyUtil.ns + "statement")
            .Where(child => child.Attribute("name").Value.StartsWith("DO", StringComparison.Ordinal)).FirstOrDefault();
        doStatements = BlocklyUtil.applyNameSpace(doStatements).Element(BlocklyUtil.ns + "statement").Element(BlocklyUtil.ns + "block");
        if (!string.IsNullOrEmpty(mode))
        {
            if (mode.Equals("WHILE"))
            {
                while(checkCondition(element, logicBlock))
                {
                    obj = eventObj.parseBlock(doStatements);
                }
            } else if (mode.Equals("UNTIL"))
            {
                while (!checkCondition(element, logicBlock))
                {
                    obj = eventObj.parseBlock(doStatements);
                }
            }
        }
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseControlsRepeatExt(XElement element)
    {
        object obj = null;
        int iterations = 0;
        element = BlocklyUtil.applyNameSpace(element);
        XElement doStatements = element.Descendants(BlocklyUtil.ns + "statement")
            .Where(child => child.Attribute("name").Value.StartsWith("DO", StringComparison.Ordinal)).FirstOrDefault();
        doStatements = BlocklyUtil.applyNameSpace(doStatements).Element(BlocklyUtil.ns + "statement").Element(BlocklyUtil.ns + "block");
        XElement valueBlock = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("TIMES")).FirstOrDefault();
        XElement block = BlocklyUtil.applyNameSpace(valueBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        if (block == null)
        {
            //might be a shadow case where user typed value inside shadow block
            block = BlocklyUtil.applyNameSpace(valueBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "shadow");
        }

        if (block != null)
        {
            obj = eventObj.parseBlock(block);
        }
        if(obj != null)
        {
            int.TryParse(BlocklyUtil.getStringFromObj(obj), out iterations);
        }
        for(int i = 0; i < iterations; i++)
        {
            obj = eventObj.parseBlock(doStatements);
        }
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseControlsFor(XElement element)
    {
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        XElement doStatements = element.Descendants(BlocklyUtil.ns + "statement")
            .Where(child => child.Attribute("name").Value.StartsWith("DO", StringComparison.Ordinal)).FirstOrDefault();
        doStatements = BlocklyUtil.applyNameSpace(doStatements).Element(BlocklyUtil.ns + "statement").Element(BlocklyUtil.ns + "block");
        XElement valueFrom = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("FROM")).FirstOrDefault();
        XElement valueTo = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("TO")).FirstOrDefault();
        XElement valueBy = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("BY")).FirstOrDefault();
        int from = getValueFromNumBlock(valueFrom);
        int to = getValueFromNumBlock(valueTo);
        int by = getValueFromNumBlock(valueBy);
        for(int i = from; i <= to;)
        {
            obj = eventObj.parseBlock(doStatements);
            i = i + by;
        }

        eventObj.parseNextBlock(element);
        return obj;
    }

    private bool checkCondition(XElement element, XElement logicBlock)
    {
        object obj = eventObj.parseBlock(logicBlock);
        bool b = false;
        if (obj.GetType().Equals(typeof(BlocklyReference)))
        {
            bool.TryParse(((BlocklyReference)obj).value.ToString(), out b);
        }
        else
        {
            bool.TryParse(obj.ToString(), out b);
        }
        return b;
    }

    private int getValueFromNumBlock(XElement element)
    {
        int val = 0;
        object obj = null;
        XElement value = BlocklyUtil.applyNameSpace(element).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        if (value == null)
        {
            //might be a shadow case where user typed value inside shadow block
            value = BlocklyUtil.applyNameSpace(element).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "shadow");
        }
        if (value != null)
        {
            obj = eventObj.parseBlock(value);
        }
        if (obj != null)
        {
            int.TryParse(BlocklyUtil.getStringFromObj(obj), out val);
        }
        return val;
    }
}
