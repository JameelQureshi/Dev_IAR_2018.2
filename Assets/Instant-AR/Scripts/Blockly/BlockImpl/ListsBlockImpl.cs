using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public class ListsBlockImpl : IBlock
{
    BlocklyEvents eventObj;
    public ListsBlockImpl()
    {
    }

    public object parse(BlocklyEvents eventObject, string blockType, string codeBlockName, XElement element)
    {
        object obj = null;
        this.eventObj = eventObject;
        switch (blockType)
        {
            case "lists_setIndex":
                obj = parseListsSetIndex(element);
                break;
            case "lists_create_with":
                obj = parseListsCreateWith(element);
                break;
            case "lists_indexOf":
                obj = parseListsIndexOf(element);
                break;
            case "lists_isEmpty":
                obj = parseListsIsEmpty(element);
                break;
            case "lists_getSublist":
                obj = parseListsGetSublist(element);
                break;
            case "lists_length":
                obj = parseListsLength(element);
                break;
            case "lists_split":
                obj = parseListsSplit(element);
                break;
            case "lists_sort":
                obj = parseListsSort(element);
                break;
            default:
                Console.WriteLine("Default case");
                break;
        }
        return obj;
    }

    private object parseListsSetIndex(XElement element)
    {
        object obj = new object();
        element = BlocklyUtil.applyNameSpace(element);
        string mode = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("MODE")).FirstOrDefault()?.Value;
        string where = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("WHERE")).FirstOrDefault()?.Value;
        XElement listVar = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("LIST")).FirstOrDefault();
        XElement toValue = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("TO")).FirstOrDefault();

        if (listVar != null)
        {
            XElement block = BlocklyUtil.applyNameSpace(listVar).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
            obj = eventObj.parseBlock(block);
        }
        object listVarObj = obj;
        if (toValue != null)
        {
            XElement block = BlocklyUtil.applyNameSpace(toValue).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
            obj = eventObj.parseBlock(block);
        }
        object toValueObj = obj;

        if (toValueObj != null)
        {
            if (toValueObj.GetType().Equals(typeof(BlocklyReference)))
            {
                BlocklyReference toValueObjBR = (BlocklyReference)toValueObj;
                toValueObj = toValueObjBR.value;
            }
            List<object> values = new List<object>();
            if (listVarObj != null)
            {
                if (listVarObj.GetType().Equals(typeof(BlocklyReference)))
                {
                    BlocklyReference bRef = (BlocklyReference)listVarObj;
                    if (BlocklyEvents.blocklyReferences.ContainsKey(bRef.name))
                    {
                        if (BlocklyEvents.blocklyReferences[bRef.name].value != null)
                        {
                            values = (List<object>)BlocklyEvents.blocklyReferences[bRef.name].value;
                        }
                        BlocklyEvents.blocklyReferences.Remove(bRef.name);
                    }
                    if (bRef.value != null)
                    {
                        values = (List<object>)bRef.value;
                    }
                    values = populateListSetIndex(mode, where, toValueObj, values);
                    bRef.value = values;
                    bRef.type = "list";
                    BlocklyEvents.blocklyReferences.Add(bRef.name, bRef);
                    obj = bRef;
                }
            }
            else
            {
                //TODO should not be the case!!
            }
        }
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseListsCreateWith(XElement element)
    {
        object obj = new object();
        List<object> numberList = new List<object>();
        element = BlocklyUtil.applyNameSpace(element);
        var valueBlocks = element.Element(BlocklyUtil.ns + "block").Elements(BlocklyUtil.ns + "value");
        //var listVar = element.Descendants(BlocklyUtil.ns + "value")
        //    .Where(child => child.Attribute("name").Value.StartsWith("ADD", StringComparison.Ordinal));
        if (valueBlocks!= null)
        {
            foreach (XElement ele in valueBlocks)
            {
                XElement block = BlocklyUtil.applyNameSpace(ele).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
                if (block != null)
                {
                    obj = eventObj.parseBlock(block);
                    if (obj != null)
                    {
                        numberList.Add(obj);
                    }
                }
            }
        }
        BlocklyReference bRef = new BlocklyReference();
        bRef.type = "list";
        bRef.value = numberList;
        eventObj.parseNextBlock(element);
        return bRef;
    }

    private object parseListsIndexOf(XElement element)
    {
        object obj = null;
        int index = 0;
        List<object> objectList = null;
        string find = null;
        element = BlocklyUtil.applyNameSpace(element);
        string mode = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("END")).FirstOrDefault()?.Value;
        XElement valueListBlock = element.Descendants(BlocklyUtil.ns + "value")
            .Where(child => child.Attribute("name").Value.Equals("VALUE")).FirstOrDefault();
        XElement valueFindBlock = element.Descendants(BlocklyUtil.ns + "value")
            .Where(child => child.Attribute("name").Value.Equals("FIND")).FirstOrDefault();
        if (valueListBlock != null)
        {
            valueListBlock = BlocklyUtil.applyNameSpace(valueListBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
            if (valueListBlock != null)
            {
                obj = eventObj.parseBlock(valueListBlock);
            }
            if (obj != null)
            {
                objectList = getListFromObj(obj);
                if (valueFindBlock != null)
                {
                    valueFindBlock = BlocklyUtil.applyNameSpace(valueFindBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
                    if (valueFindBlock != null)
                    {
                        obj = eventObj.parseBlock(valueFindBlock);
                    }
                    if (obj != null)
                    {
                        find = BlocklyUtil.getStringFromObj(obj);
                    }
                    if (mode.Equals("FIRST"))
                    {
                        index = objectList.FindIndex(s => s.Equals(find));
                    } else if (mode.Equals("LAST"))
                    {
                        index = objectList.FindLastIndex(s => s.Equals(find));
                    } else
                    {
                        //TODO not sure, will return first anyway
                        index = objectList.FindIndex(s => s.Equals(find));
                    }
                }
            }
        }

        eventObj.parseNextBlock(element);
        return index;
    }

    private object parseListsIsEmpty(XElement element)
    {
        object obj = null;
        bool empty = false;
        element = BlocklyUtil.applyNameSpace(element);
        XElement valueListBlock = element.Descendants(BlocklyUtil.ns + "value")
            .Where(child => child.Attribute("name").Value.Equals("VALUE")).FirstOrDefault();
        if (valueListBlock != null)
        {
            valueListBlock = BlocklyUtil.applyNameSpace(valueListBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
            if (valueListBlock != null)
            {
                obj = eventObj.parseBlock(valueListBlock);
            }
            if (obj != null)
            {
                if (getListFromObj(obj).Count() == 0)
                {
                    empty = true;
                }
            }
        }

        eventObj.parseNextBlock(element);
        return empty;
    }

    private object parseListsGetSublist(XElement element)
    {
        object obj = null;
        List<object> objectList = null;
        element = BlocklyUtil.applyNameSpace(element);
        XElement valueListBlock = element.Descendants(BlocklyUtil.ns + "value")
            .Where(child => child.Attribute("name").Value.Equals("LIST")).FirstOrDefault();
        string where1 = element.Descendants(BlocklyUtil.ns + "field")
            .Where(child => child.Attribute("name").Value.Equals("WHERE1")).FirstOrDefault()?.Value;
        string where2 = element.Descendants(BlocklyUtil.ns + "field")
            .Where(child => child.Attribute("name").Value.Equals("WHERE2")).FirstOrDefault()?.Value;
        if (valueListBlock != null)
        {
            valueListBlock = BlocklyUtil.applyNameSpace(valueListBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
            if (valueListBlock != null)
            {
                obj = eventObj.parseBlock(valueListBlock);
            }
            if (obj != null)
            {
                objectList = getListFromObj(obj);
                if (objectList != null && objectList.Count > 0)
                {
                    int startIndex = getIndex(element, where1, "AT1", objectList.Count);
                    int endIndex = getIndex(element, where2, "AT2", objectList.Count);
                    if (startIndex > endIndex)
                    {
                        objectList = new List<object>();
                    }
                    else
                    {   //TODO Lax and Jitu to discuss if user types startindex based on zero base or actual number
                        //accordingly get range from startIndex or startIndex-1
                        if (startIndex != 0)
                        {
                            endIndex = endIndex - startIndex + 1; //+1 in order to include the starting index as well
                            startIndex = startIndex - 1;
                        } 
                        //if (endIndex != objectList.Count)
                        //{
                        //    endIndex = endIndex - 1;
                        //}
                        objectList = objectList.GetRange(startIndex, endIndex);
                    }
                }
                
            }
        }

        eventObj.parseNextBlock(element);
        return objectList;
    }

    private object parseListsLength(XElement element)
    {
        object obj = null;
        int length = 0;
        XElement valueBlock = BlocklyUtil.applyNameSpace(element).Element(BlocklyUtil.ns + "block")
            .Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        if (valueBlock != null)
        {
            obj = eventObj.parseBlock(valueBlock);
            if (obj != null)
            {
                length = getListFromObj(obj).Count;
            }
        }
        eventObj.parseNextBlock(element);
        return length;
    }

    private object parseListsSplit(XElement element)
    {
        object obj = null;
        string mode = element.Descendants(BlocklyUtil.ns + "field")
            .Where(child => child.Attribute("name").Value.Equals("MODE")).FirstOrDefault()?.Value;
        XElement valueInputBlock = element.Descendants(BlocklyUtil.ns + "value")
            .Where(child => child.Attribute("name").Value.Equals("INPUT")).FirstOrDefault();
        XElement valueDelimBlock = element.Descendants(BlocklyUtil.ns + "value")
            .Where(child => child.Attribute("name").Value.Equals("DELIM")).FirstOrDefault();

        if (valueInputBlock != null)
        {
            valueInputBlock = BlocklyUtil.applyNameSpace(valueInputBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
            if (valueInputBlock != null)
            {
                object input = eventObj.parseBlock(valueInputBlock);
                XElement text = BlocklyUtil.applyNameSpace(valueDelimBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
                if (text == null)
                {
                    text = BlocklyUtil.applyNameSpace(valueDelimBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "shadow");
                }
                if (text != null)
                {
                    object delim = eventObj.parseBlock(text);
                    if (delim != null)
                    {
                        if (mode.Equals("SPLIT"))
                        {
                            List<string> returnList = new List<string>();
                            string baseString = BlocklyUtil.getStringFromObj(input);
                            string[] delimArray = { BlocklyUtil.getStringFromObj(delim) };
                            returnList = baseString.Split(delimArray, StringSplitOptions.None).ToList();
                            obj = returnList;
                        } else if (mode.Equals("JOIN"))
                        {
                            List<object> inputList = getListFromObj(input);
                            string returnStr = "";
                            string delimStr = BlocklyUtil.getStringFromObj(delim);
                            foreach (object ob in inputList)
                            {
                                returnStr = returnStr + ob.ToString() + delimStr;
                            }
                            obj = returnStr;
                        }
                    }
                }
            }
        }

        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseListsSort(XElement element)
    {
        object obj = null;
        BlocklyReference bRef = new BlocklyReference();
        string direction = element.Descendants(BlocklyUtil.ns + "field")
            .Where(child => child.Attribute("name").Value.Equals("DIRECTION")).FirstOrDefault()?.Value;
        string type = element.Descendants(BlocklyUtil.ns + "field")
            .Where(child => child.Attribute("name").Value.Equals("TYPE")).FirstOrDefault()?.Value;
        XElement valueInputBlock = element.Descendants(BlocklyUtil.ns + "value")
            .Where(child => child.Attribute("name").Value.Equals("INPUT")).FirstOrDefault();
        XElement valueListBlock = element.Descendants(BlocklyUtil.ns + "value")
            .Where(child => child.Attribute("name").Value.Equals("LIST")).FirstOrDefault();
        if (valueListBlock != null)
        {
            valueListBlock = BlocklyUtil.applyNameSpace(valueListBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
            obj = eventObj.parseBlock(valueListBlock);
            List<object> list = getListFromObj(obj);
            int dire = 0;
            int.TryParse(direction, out dire);
            bRef.type = "list";
            if (dire == 1)
            {
                //obj = list.OrderBy(x => x).ToList();
                bRef.value = list.OrderBy(x => x).ToList();
            } else if (dire == -1)
            {
                bRef.value = list.OrderByDescending(x => x).ToList();
                //obj = list.OrderByDescending(x => x).ToList();
            }
        }
        eventObj.parseNextBlock(element);
        return bRef;
    }

    private List<object> populateListSetIndex(
        string mode, string where, object toValueObj, List<object> values)
    {
        if (toValueObj.GetType().Equals(typeof(BlocklyReference)))
        {
            toValueObj = ((BlocklyReference)toValueObj).value;
        }
        int length = values.Count;
        if (mode.Equals("INSERT"))
        {
            switch (where)
            {
                case "FIRST":
                    values.Insert(0, toValueObj);
                    break;
                case "LAST":
                    values.Add(toValueObj);
                    break;
                case "RANDOM":
                    //TODO implement it
                    break;
                case "FROM_END":
                    //TODO implement it
                    break;
                case "FROM_START":
                    //TODO implement it
                    break;
                default:
                    break;
            }
        }
        else if (mode.Equals("SET"))
        {
            switch (where)
            {
                case "FIRST":
                    values[0] = toValueObj;
                    break;
                case "LAST":
                    if (values.Count == 0)
                    {
                        values.Add(toValueObj);
                        //values[values.Count] = toValueObj;
                    } else
                    {
                        values.Insert(0, toValueObj);
                        //values[values.Count - 1] = toValueObj;
                    }
                    break;
                case "RANDOM":
                    //TODO implement it
                    break;
                case "FROM_END":
                    //TODO implement it
                    break;
                case "FROM_START":
                    //TODO implement it
                    break;
                default:
                    break;
            }
        }
        return values;
    }

    private List<object> getListFromObj(object obj) 
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

    private int getIndex(XElement element, string where, string valueName, int count)
    {
        int indexToReturn = 0;
        switch (where)
        {
            case "FIRST":
                indexToReturn = 0;
                break;
            case "LAST":
                indexToReturn = count;
                break;
            case "FROM_START":
                XElement valueStartBlock = element.Descendants(BlocklyUtil.ns + "value")
                    .Where(child => child.Attribute("name").Value.Equals(valueName)).FirstOrDefault();
                if (valueStartBlock != null)
                {
                    valueStartBlock = BlocklyUtil.applyNameSpace(valueStartBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
                    if (valueStartBlock != null)
                    {
                        object obj = eventObj.parseBlock(valueStartBlock);
                        if (obj != null)
                        {
                            int.TryParse(BlocklyUtil.getStringFromObj(obj), out indexToReturn);
                            if (indexToReturn > count)
                            {
                                indexToReturn = count;
                            }
                        }
                    }
                }
                break;
            case "FROM_END":
                XElement valueEndBlock = element.Descendants(BlocklyUtil.ns + "value")
                    .Where(child => child.Attribute("name").Value.Equals(valueName)).FirstOrDefault();
                if (valueEndBlock != null)
                {
                    valueEndBlock = BlocklyUtil.applyNameSpace(valueEndBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
                    if (valueEndBlock != null)
                    {
                        object obj = eventObj.parseBlock(valueEndBlock);
                        if (obj != null)
                        {
                            int.TryParse(BlocklyUtil.getStringFromObj(obj), out indexToReturn);
                            if (indexToReturn > count)
                            {
                                indexToReturn = 0;
                            } else
                            {
                                indexToReturn = count - indexToReturn + 1;//To include the starting index as well
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
        return indexToReturn;
    }
}
