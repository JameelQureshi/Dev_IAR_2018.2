using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Random = System.Random;

public class TextBlockImpl : IBlock
{
    BlocklyEvents eventObj;
    public TextBlockImpl()
    {
    }

    public object parse(BlocklyEvents eventObject, string blockType, string codeBlockName, XElement element)
    {
        object obj = null;
        this.eventObj = eventObject;
        switch (blockType)
        {
            case "text":
                obj = parseText(element);
                break;
            case "text_join":
                obj = parseTextJoin(element);
                break;
            case "text_prompt_ext":
                obj = parseTextPromptExt(element);
                break;
            case "text_length":
                obj = parseTextLength(element);
                break;
            case "text_changeCase":
                obj = parseTextChangeCase(element);
                break;
            case "text_indexOf":
                obj = parseTextIndexOf(element);
                break;
            case "text_charAt":
                obj = parseTextCharAt(element);
                break;
            case "text_trim":
                obj = parseTextTrim(element);
                break;
            case "text_append":
                obj = parseTextAppend(element);
                break;
            case "text_print":
                obj = parseTextPrint(element);
                break;
            default:
                Console.WriteLine("Default case");
                break;
        }
        return obj;
    }

    private object parseText(XElement element)
    {
        element = BlocklyUtil.applyNameSpace(element);
        string textVal = element.Descendants(BlocklyUtil.ns + "field")
            .Where(child => child.Attribute("name").Value.Equals("TEXT")).FirstOrDefault()?.Value;
        //string textVal = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "field").Value.Replace("\"", "");
        eventObj.parseNextBlock(element);
        return textVal;
    }

    private object parseTextPromptExt(XElement element)
    {
        //TODO put the implementation with pop up etc
        return null;
    }

    private object parseTextJoin(XElement element)
    {
        string finalString = "";
        var valueBlock = element.Elements(BlocklyUtil.ns + "value");
        foreach(XElement value in valueBlock)
        {
            XElement block = value.Element(BlocklyUtil.ns + "block");
            if (block != null)
            {
                object obj = eventObj.parseBlock(block);
                string val = BlocklyUtil.getStringFromObj(obj);
                if (val != null)
                {
                    finalString = finalString + val;
                }
            }
        }
        return finalString;
    }

    private object parseTextLength(XElement element)
    {
        int length = 0;
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        XElement valueBlock = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        if (valueBlock != null)
        {
            obj = eventObj.parseBlock(valueBlock);
        }
        if (obj != null)
        {
            if (obj.GetType().Equals(typeof(string)))
            {
                length = ((string)obj).Length;
            } else if(obj.GetType().Equals(typeof(BlocklyReference)))
            {
                length = ((BlocklyReference)obj).value.ToString().Length;
            } else
            {
                length = obj.ToString().Length;
            }
        }
        eventObj.parseNextBlock(element);
        return length;
    }

    private object parseTextChangeCase(XElement element)
    {
        string changeCase = null;
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        XElement valueBlock = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        string caseToConvert = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("CASE")).FirstOrDefault()?.Value;
        if (valueBlock != null)
        {
            obj = eventObj.parseBlock(valueBlock);
        }
        if (obj != null)
        {
            if (obj.GetType().Equals(typeof(string)))
            {
                changeCase = convertCases(caseToConvert, (string)obj);
            }
            else if (obj.GetType().Equals(typeof(BlocklyReference)))
            {
                changeCase = convertCases(caseToConvert, ((BlocklyReference)obj).value.ToString());
            }
            else
            {
                changeCase = convertCases(caseToConvert, obj.ToString());
            }
        }
        eventObj.parseNextBlock(element);
        return changeCase;
    }

    private object parseTextIndexOf(XElement element)
    {
        string baseString = null;
        string stringToFind = null;
        int index = 0;
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        XElement baseStringValueBlock = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("VALUE")).FirstOrDefault();
        XElement stringToFindValueBlock = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("FIND")).FirstOrDefault();
        string place = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("END")).FirstOrDefault()?.Value;

        if(baseStringValueBlock != null)
        {
            baseStringValueBlock = BlocklyUtil.applyNameSpace(baseStringValueBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
            if (baseStringValueBlock!= null)
            {
                obj = eventObj.parseBlock(baseStringValueBlock);
            }
        }
        baseString = BlocklyUtil.getStringFromObj(obj);

        if (stringToFindValueBlock != null)
        {
            stringToFindValueBlock = BlocklyUtil.applyNameSpace(stringToFindValueBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
            if (stringToFindValueBlock != null)
            {
                obj = eventObj.parseBlock(stringToFindValueBlock);
            }
        }
        stringToFind = BlocklyUtil.getStringFromObj(obj);

        if (place == null || baseString == null || baseString.Length < 1 || stringToFind == null || stringToFind.Length <= 0)
        {
            return 0;
        }
        if (place.Equals("FIRST"))
        {
            index = baseString.IndexOf(stringToFind, StringComparison.Ordinal);
        } else if (place.Equals("LAST"))
        {
            index = baseString.LastIndexOf(stringToFind, StringComparison.Ordinal);
        }
        
        eventObj.parseNextBlock(element);
        return index;
    }

    private object parseTextCharAt(XElement element)
    {
        string baseString = null;
        int indexToReturn = 0;
        char returnVal = ' ';
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        XElement baseStringValueBlock = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("VALUE")).FirstOrDefault();
        string where = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("WHERE")).FirstOrDefault()?.Value;
        XElement indexValueBlock = null;
        if (where != null && !(where.Equals("FIRST") || where.Equals("LAST") || where.Equals("RANDOM"))){
            indexValueBlock = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("AT")).FirstOrDefault();
        }

        if (baseStringValueBlock != null)
        {
            baseStringValueBlock = BlocklyUtil.applyNameSpace(baseStringValueBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
            if (baseStringValueBlock != null)
            {
                obj = eventObj.parseBlock(baseStringValueBlock);
            }
            baseString = BlocklyUtil.getStringFromObj(obj);
        }

        if (where == null || baseString == null || baseString.Length < 1)
        {
            return returnVal;
        }

        if (indexValueBlock != null)
        {
            indexValueBlock = BlocklyUtil.applyNameSpace(indexValueBlock).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
            if (indexValueBlock != null)
            {
                obj = eventObj.parseBlock(indexValueBlock);
            }
            indexToReturn = int.Parse(BlocklyUtil.getStringFromObj(obj));
        }

        returnVal = getLetterAtIndex(baseString, where, indexToReturn);

        eventObj.parseNextBlock(element);
        return returnVal;
    }

    private object parseTextTrim(XElement element)
    {
        string returnString = null;
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        XElement valueBlock = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        string mode = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("MODE")).FirstOrDefault()?.Value;

        if (valueBlock != null)
        {
            obj = eventObj.parseBlock(valueBlock);
            returnString = BlocklyUtil.getStringFromObj(obj);
        }

        switch (mode)
        {
            case "LEFT":
                returnString = returnString.TrimStart();
                break;
            case "RIGHT":
                returnString = returnString.TrimEnd();
                break;
            case "BOTH":
                returnString = returnString.Trim();
                break;
            default:
                break;

        }
        eventObj.parseNextBlock(element);
        return returnString;
    }

    private object parseTextAppend(XElement element)
    {
        string append = null;
        string appendTo = null;
        string resultVal = null;
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        XElement valueBlock = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        string varToAppend = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("VAR")).FirstOrDefault()?.Value;

        if (valueBlock != null)
        {
            obj = eventObj.parseBlock(valueBlock);
            append = BlocklyUtil.getStringFromObj(obj);
        }

        if(varToAppend != null)
        {
            if (BlocklyEvents.blocklyReferences.ContainsKey(varToAppend))
            {
                BlocklyReference bRef = BlocklyEvents.blocklyReferences[varToAppend];
                appendTo = BlocklyUtil.getStringFromObj(bRef);
                if (string.IsNullOrEmpty(append))
                {
                    resultVal = appendTo;
                }
                else
                {
                    resultVal = appendTo + append;
                }
                bRef.value = resultVal;
                BlocklyEvents.blocklyReferences.Remove(varToAppend);
                BlocklyEvents.blocklyReferences.Add(varToAppend, bRef);
            } else
            {
                //TODO LAX and Jitu to do further brainstorm if we should allow non initialized
                //variables to be automatically initialized
            }
            
        }
        
        eventObj.parseNextBlock(element);
        return resultVal;
    }

    private object parseTextPrint(XElement element)
    {
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        XElement valueBlock = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        if (valueBlock != null)
        {
            obj = eventObj.parseBlock(valueBlock);
        } else
        {
            //Might be the case of Shadow block, lets get the value directly
            XElement shadowBlock = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "shadow");
            if(shadowBlock != null)
            {
                obj = BlocklyUtil.applyNameSpace(shadowBlock).Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("TEXT")).FirstOrDefault()?.Value;
            }
        }
        if (obj != null)
        {
            if (obj.GetType().Equals(typeof(string)))
            {
                if (obj.ToString().Equals("GenerateReport"))
                {
                    //TODO LAX : Purely for testing
                    List<Tabel_DescriptionItem> tdiList = new List<Tabel_DescriptionItem>();
                    List<SelectionsItem> siList = new List<SelectionsItem>();
                    List<DataItem> dList = new List<DataItem>();
                    foreach(ARReportInfo1 rep in PopupButtons.reportDetails)
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

                    PopupUtilities.makePopupTable(null,null, json, true, null);
                }
                Debug.Log("<color=green> parseTextPrint : </color>" + obj);
            }
            else if (obj.GetType().Equals(typeof(BlocklyReference)))
            {
                if (((BlocklyReference)obj).value != null)
                {
                    Debug.Log("<color=green> parseTextPrint : </color>" + ((BlocklyReference)obj).value.ToString());
                } else
                {
                    Debug.Log("<color=green> parseTextPrint : null!! : Investigate this!</color>" );
                }
            }
            else
            {
                Debug.Log("<color=green> parseTextPrint : </color>" + obj.ToString());
            }
        }
        eventObj.parseNextBlock(element);
        //I have the info and the object sand stuing all, what i do here to display the table
        //get the object and get it jsons done
        //call the uitable popup
        return obj;
    }

    private string convertCases(string caseToConvert, string changeCase)
    {
        if (caseToConvert.Equals("UPPERCASE"))
        {
            changeCase = changeCase.ToUpper();
        }
        else if (caseToConvert.Equals("LOWERCASE"))
        {
            changeCase = changeCase.ToLower();
        }
        return changeCase;
    }

    private char getLetterAtIndex(string baseString, string where, int indexToReturn)
    {
        char letterAt = ' ';
        switch (where)
        {
            case "FROM_START":
                letterAt = baseString.ElementAt(indexToReturn);
                break;
            case "FROM_END":
                letterAt = baseString.ElementAt(baseString.Length-indexToReturn);
                break;
            case "FIRST":
                letterAt = baseString.First();
                break;
            case "LAST":
                letterAt = baseString.Last();
                break;
            case "RANDOM":
                int max = baseString.Length;
                Random r = new System.Random();
                letterAt = baseString.ElementAt(r.Next(0, max));
                break;
            default:
                break;
        }
        return letterAt;
    }

    public static string getHardCodeStringJason()
    {
        return "{\n" +
                        "    \"Tabel_Description\": {\n" +
                        "      \"ColumnSize\": 5,\n" +
                        "      \"DataStructure\": [\n" +
                        "        {\n" +
                        "          \"Name\": \"Entry1\",\n" +
                        "          \"Type\": \"String\"\n" +
                        "        },\n" +
                        "        {\n" +
                        "          \"Name\": \"Entry2\",\n" +
                        "          \"Type\": \"String\"\n" +
                        "        },\n" +
                        "        {\n" +
                        "          \"Name\": \"Entry3\",\n" +
                        "          \"Type\": \"String\"\n" +
                        "        },\n" +
                        "        {\n" +
                        "          \"Name\": \"Entry4\",\n" +
                        "          \"Type\": \"String\"\n" +
                        "        },\n" +
                        "        {\n" +
                        "          \"Name\": \"Entry5\",\n" +
                        "          \"Type\": \"Number\"\n" +
                        "        }\n" +
                        "      ]\n" +
                        "    },\n" +
                        "    \"Column_Selections\": {\n" +
                        "      \"SelectedColumns\": [\n" +
                        "        {\n" +
                        "          \"Name\": \"Entry3\",\n" +
                        "          \"Title\": \"Column1\",\n" +
                        "          \"Width\": \"33\",\n" +
                        "          \"Sortable\": \"true\"\n" +
                        "        },\n" +
                        "        {\n" +
                        "          \"Name\": \"Entry4\",\n" +
                        "          \"Title\": \"Column1\",\n" +
                        "          \"Width\": \"33\",\n" +
                        "          \"Sortable\": \"true\"\n" +
                        "        },\n" +
                        "        {\n" +
                        "          \"Name\": \"Entry5\",\n" +
                        "          \"Title\": \"Age\",\n" +
                        "          \"Width\": \"Auto\",\n" +
                        "          \"Sortable\": \"true\"\n" +
                        "        }\n" +
                        "      ]\n" +
                        "    },\n" +
                        "    \"Data\": {\n" +
                        "      \"Rows\": 5,\n" +
                        "      \"Data\": [\n" +
                        "        {\n" +
                        "          \"Group\": [\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry3\",\n" +
                        "              \"ColumnData\": \"Megha\"\n" +
                        "            },\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry4\",\n" +
                        "              \"ColumnData\": \"Sharma\"\n" +
                        "            },\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry5\",\n" +
                        "              \"ColumnData\": \"13\"\n" +
                        "            }\n" +
                        "          ]\n" +
                        "        },\n" +
                        "        {\n" +
                        "          \"Group\": [\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry3\",\n" +
                        "              \"ColumnData\": \"Lax\"\n" +
                        "            },\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry4\",\n" +
                        "              \"ColumnData\": \"Sharma\"\n" +
                        "            },\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry5\",\n" +
                        "              \"ColumnData\": \"87\"\n" +
                        "            }\n" +
                        "          ]\n" +
                        "        },\n" +
                        "        {\n" +
                        "          \"Group\": [\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry3\",\n" +
                        "              \"ColumnData\": \"Jitu\"\n" +
                        "            },\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry4\",\n" +
                        "              \"ColumnData\": \"Hooda\"\n" +
                        "            },\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry5\",\n" +
                        "              \"ColumnData\": \"65\"\n" +
                        "            }\n" +
                        "          ]\n" +
                        "        },\n" +
                        "        {\n" +
                        "          \"Group\": [\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry3\",\n" +
                        "              \"ColumnData\": \"Sushma\"\n" +
                        "            },\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry4\",\n" +
                        "              \"ColumnData\": \"Hooda\"\n" +
                        "            },\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry5\",\n" +
                        "              \"ColumnData\": \"16\"\n" +
                        "            }\n" +
                        "          ]\n" +
                        "        },\n" +
                        "        {\n" +
                        "          \"Group\": [\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry3\",\n" +
                        "              \"ColumnData\": \"Shaumik\"\n" +
                        "            },\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry4\",\n" +
                        "              \"ColumnData\": \"Hooda\"\n" +
                        "            },\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry5\",\n" +
                        "              \"ColumnData\": \"28\"\n" +
                        "            }\n" +
                        "          ]\n" +
                        "        },\n" +
                        "        {\n" +
                        "          \"Group\": [\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry3\",\n" +
                        "              \"ColumnData\": \"Soumya\"\n" +
                        "            },\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry4\",\n" +
                        "              \"ColumnData\": \"Sharma\"\n" +
                        "            },\n" +
                        "            {\n" +
                        "              \"ColumnName\": \"Entry5\",\n" +
                        "              \"ColumnData\": \"23\"\n" +
                        "            }\n" +
                        "          ]\n" +
                        "        }\n" +
                        "      ]\n" +
                        "    }\n" +
                        "  }";

    }

}
