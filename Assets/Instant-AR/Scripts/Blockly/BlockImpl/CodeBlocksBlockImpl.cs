using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using UnityEngine;

public class CodeBlocksBlockImpl : IBlock
{
    BlocklyEvents eventObj;

    public CodeBlocksBlockImpl()
    {
    }

    public object parse(BlocklyEvents eventObject, string blockType, string codeBlockName, XElement element)
    {
        object obj = null;
        this.eventObj = eventObject;
        switch (blockType)
        {
            case "procedures_defnoreturn":
                if (!eventObj.firstExecution)
                {
                    if (codeBlockName != null && codeBlockName.Length > 1)
                    {
                        string expression = "//prefix:block[@type='procedures_defnoreturn' and prefix:field/text()='" + codeBlockName + "']";
                        Debug.Log("<color=green> $$$$$$$$$$$$$$$$$ expression  : </color>" + expression);
                        element = BlocklyUtil.applyNameSpace(element);
                        element = element.XPathSelectElement(expression, BlocklyUtil.getnsmgr());
                    }
                    else
                    {
                        element = BlocklyUtil.applyNameSpace(element);
                        element = element.XPathSelectElement("//block[@type=\"procedures_defnoreturn\"]", BlocklyUtil.getnsmgr());
                    }
                    obj = parseProceduresDefnoreturn(element);
                }
                break;
            case "procedures_callnoreturn":
                //if (codeBlockName != null && codeBlockName.Length > 1)
                //{
                //    string expression = "//prefix:block[@type='procedures_defnoreturn' and prefix:field/text()='" + codeBlockName + "']";
                //    Debug.Log("<color=red> $$$$$$$$$$$$$$$$$ expression  : </color>" + expression);
                //    element = BlocklyUtil.applyNameSpace(element);
                //    element = element.XPathSelectElement(expression, BlocklyUtil.getnsmgr());
                //}
                //else
                //{
                //    element = BlocklyUtil.applyNameSpace(element);
                //    element = element.XPathSelectElement("//block[@type=\"procedures_defnoreturn\"]", BlocklyUtil.getnsmgr());
                //}
                //obj = parseProceduresDefnoreturn(element);
                obj = parseProceduresCallnoreturn(element, codeBlockName);
                break;
            case "procedures_defreturn":
                if (!eventObj.firstExecution)
                {
                    if (codeBlockName != null && codeBlockName.Length > 1)
                    {
                        string expression = "//prefix:block[@type='procedures_defreturn' and prefix:field/text()='" + codeBlockName + "']";
                        Debug.Log("<color=green> $$$$$$$$$$$$$$$$$ expression  : </color>" + expression);
                        element = BlocklyUtil.applyNameSpace(element);
                        element = element.XPathSelectElement(expression, BlocklyUtil.getnsmgr());
                    }
                    else
                    {
                        element = BlocklyUtil.applyNameSpace(element);
                        element = element.XPathSelectElement("//block[@type=\"procedures_defreturn\"]", BlocklyUtil.getnsmgr());
                    }
                    obj = parseProceduresDefreturn(element);
                }
                break;
            case "procedures_callreturn":
                obj = parseProceduresCallreturn(element, codeBlockName);
                break;
            default:
                Console.WriteLine("Default case");
                break;
        }

        return obj;
    }

    private object parseProceduresDefnoreturn(XElement element)
    {
        object obj = new object();
        element = BlocklyUtil.applyNameSpace(element);
        XElement block = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "statement").Element(BlocklyUtil.ns + "block");
        obj = eventObj.parseBlock(block);
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseProceduresCallnoreturn(XElement element, string codeBlockName)
    {
        object obj = new object();
        //element = BlocklyUtil.applyNameSpace(element);
        //XElement mutation = element.Descendants(BlocklyUtil.ns + "mutation")
        //    .Where(child => child.Attribute("name").Value.Equals(codeBlockName)).FirstOrDefault();
        ////XElement mutation = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "mutation");
        //string methodToBeCalled = null;
        //if (mutation != null)
        //{
        //    methodToBeCalled = mutation.Attribute("name").Value;
        //}
        XElement mutation = element.Descendants(BlocklyUtil.ns + "mutation").FirstOrDefault();
        var mutationArgs = mutation.Descendants(BlocklyUtil.ns + "arg");
        if (mutationArgs != null)
        {
            int i = 0;
            foreach (XElement value in mutationArgs)
            {
                BlocklyReference bRef = new BlocklyReference();
                bRef.type = "var";
                bRef.name = value.Attribute("name").Value;
                XElement varValue = element.Descendants(BlocklyUtil.ns + "value")
                    .Where(child => child.Attribute("name").Value.Equals("ARG" + i)).FirstOrDefault();
                if (varValue != null)
                {
                    varValue = varValue.Element(BlocklyUtil.ns + "block");//.Element(BlocklyUtil.ns + "block");
                    obj = eventObj.parseBlock(varValue);
                    if (obj.GetType().Equals(typeof(BlocklyReference)))
                    {
                        bRef.type = ((BlocklyReference)obj).type;
                        bRef.value = ((BlocklyReference)obj).value;
                    } else
                    {
                        bRef.value = obj;
                    }
                }                
                BlocklyEvents.blocklyReferences.Remove(bRef.name);
                BlocklyEvents.blocklyReferences.Add(bRef.name, bRef);
                i = i + 1;
            }
        }
        obj = eventObj.executeBlockType(null, "procedures_defnoreturn", codeBlockName, eventObj.baseElement);
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseProceduresDefreturn(XElement element)
    {
        object obj = new object();
        element = BlocklyUtil.applyNameSpace(element);
        XElement block = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "statement").Element(BlocklyUtil.ns + "block");
        XElement returnElement = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        obj = eventObj.parseBlock(block);
        obj = eventObj.parseBlock(returnElement);
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseProceduresCallreturn(XElement element, string codeBlockName)
    {
        object obj = new object();
        //element = BlocklyUtil.applyNameSpace(element);
        //XElement mutation = element.Descendants(BlocklyUtil.ns + "mutation")
        //    .Where(child => child.Attribute("name").Value.Equals(codeBlockName)).FirstOrDefault();
        ////XElement mutation = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "mutation");
        //string methodToBeCalled = null;
        //if (mutation != null)
        //{
        //    methodToBeCalled = mutation.Attribute("name").Value;
        //}
        XElement mutation = element.Descendants(BlocklyUtil.ns + "mutation").FirstOrDefault();
        var mutationArgs = mutation.Descendants(BlocklyUtil.ns + "arg");
        if (mutationArgs != null)
        {
            int i = 0;
            foreach (XElement value in mutationArgs)
            {
                BlocklyReference bRef = new BlocklyReference();
                bRef.type = "var";
                bRef.name = value.Attribute("name").Value;
                XElement varValue = element.Descendants(BlocklyUtil.ns + "value")
                    .Where(child => child.Attribute("name").Value.Equals("ARG" + i)).FirstOrDefault();
                if (varValue != null)
                {
                    varValue = varValue.Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
                    obj = eventObj.parseBlock(varValue);
                    if (obj.GetType().Equals(typeof(BlocklyReference)))
                    {
                        bRef.type = ((BlocklyReference)obj).type;
                        bRef.value = ((BlocklyReference)obj).value;
                    }
                    else
                    {
                        bRef.value = obj;
                    }
                }
                BlocklyEvents.blocklyReferences.Remove(bRef.name);
                BlocklyEvents.blocklyReferences.Add(bRef.name, bRef);
                i = i + 1;
            }
        }

        obj = eventObj.executeBlockType(null, "procedures_defreturn", codeBlockName, eventObj.baseElement);
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseColourPicker(XElement element)
    {
        element = BlocklyUtil.applyNameSpace(element);
        return element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "field").Value;
    }
}
