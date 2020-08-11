using System;
using System.Collections.Generic;
using System.Xml.Linq;

public class VariablesBlockImpl : IBlock
{
    BlocklyEvents eventObj;

    public VariablesBlockImpl()
    {
    }

    public object parse(BlocklyEvents eventObject, string blockType, string codeBlockName, XElement element)
    {
        object obj = null;
        this.eventObj = eventObject;
        switch (blockType)
        {
            case "variables_set":
                if (codeBlockName != null && codeBlockName.Length > 1)
                {
                    if (codeBlockName.Equals("VAR"))
                    {
                        obj = parseVariablesSetVar(element);
                    }
                    else
                    {
                        obj = parseVariablesSetVar(element);
                    }
                }
                else
                {
                    obj = parseVariablesSetVar(element);
                }
                break;
            case "variables_get":
                if (codeBlockName != null && codeBlockName.Length > 1)
                {
                    if (codeBlockName.Equals("VAR"))
                    {
                        obj = parseVariablesGetVar(element);
                    }
                    else
                    {
                        obj = parseVariablesGetVar(element);
                    }
                }
                break;
            default:
                Console.WriteLine("Default case");
                break;
        }
        return obj;
    }

    private object parseVariablesSetVar(XElement element)
    {
        object obj = new object();
        BlocklyReference bRef = new BlocklyReference();
        element = BlocklyUtil.applyNameSpace(element);
        XElement field = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "field");
        bRef.name = field.Value;
        XElement value = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value");
        if (value != null)
        {
            XElement block = value.Element(BlocklyUtil.ns + "block");
            if (block != null && block.Attribute("type") != null)
            {
                bRef.type = block.Attribute("type").Value;
            }
            obj = eventObj.parseBlock(block);
            if (obj != null && obj.GetType().Equals(typeof(BlocklyReference)))
            {
                bRef.value = ((BlocklyReference)obj).value;
                bRef.type = ((BlocklyReference)obj).type;
            }
            else
            {
                bRef.value = obj;
            }
            if (eventObj.firstExecution)
            {
                if (BlocklyEvents.blocklyReferencesGlobal.ContainsKey(bRef.name))
                {
                    BlocklyEvents.blocklyReferencesGlobal.Remove(bRef.name);
                }
                BlocklyEvents.blocklyReferencesGlobal.Add(bRef.name, bRef);
            } else if(BlocklyEvents.blocklyReferencesGlobal.ContainsKey(bRef.name))
            {
                //if (TestButtonScript.blocklyReferencesGlobal.ContainsKey(bRef.name))
                //{
                //    TestButtonScript.blocklyReferencesGlobal.Remove(bRef.name);
                //}
                BlocklyEvents.blocklyReferencesGlobal.Remove(bRef.name);
                BlocklyEvents.blocklyReferencesGlobal.Add(bRef.name, bRef);
            } else
            {
                if (BlocklyEvents.blocklyReferences.ContainsKey(bRef.name))
                {
                    BlocklyEvents.blocklyReferences.Remove(bRef.name);
                }
                BlocklyEvents.blocklyReferences.Add(bRef.name, bRef);
            }
            eventObj.parseNextBlock(element);
        }
        return obj;
    }

    private object parseVariablesGetVar(XElement element)
    {
        element = BlocklyUtil.applyNameSpace(element);
        object obj = new object();
        string variableName = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "field").Value;
        if (BlocklyEvents.blocklyReferences.ContainsKey(variableName))
        {
            BlocklyReference bRef = BlocklyEvents.blocklyReferences[variableName];
            if (bRef != null)
            {
                if (bRef.type != null && bRef.value != null
                    && bRef.type.Equals("text") && bRef.value.ToString().StartsWith("ARQuery:", StringComparison.Ordinal))
                {
                    bRef = BlocklyUtil.getQueryResults(bRef, "", "");
                }
                obj = bRef;
            }
        } else if (BlocklyEvents.blocklyReferencesGlobal.ContainsKey(variableName))
        {
            BlocklyReference bRef = BlocklyEvents.blocklyReferencesGlobal[variableName];
            if (bRef != null)
            {
                if (bRef.type != null && bRef.value != null
                    && bRef.type.Equals("text") && bRef.value.ToString().StartsWith("ARQuery:", StringComparison.Ordinal))
                {
                    bRef = BlocklyUtil.getQueryResults(bRef, "", "");
                }
                obj = bRef;
            }
        }
        else
        {
            BlocklyReference bRef = new BlocklyReference();
            bRef.name = variableName;
            //bRef.value = new object();
            BlocklyEvents.blocklyReferences.Add(variableName, bRef);
            obj = bRef;
        }
        eventObj.parseNextBlock(element);
        return obj;
    }
}
