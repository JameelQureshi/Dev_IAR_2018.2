using System;
using System.Linq;
using System.Xml.Linq;

public class LogicBlockImpl : IBlock
{
    BlocklyEvents eventObj;
    public LogicBlockImpl()
    {
    }

    public object parse(BlocklyEvents eventObject, string blockType, string codeBlockName, XElement element)
    {
        object obj = null;
        this.eventObj = eventObject;
        switch (blockType)
        {
            case "logic_boolean":
                obj = parseLogicBoolean(element);
                break;
            case "logic_compare":
                obj = parseLogicCompare(element);
                break;
            case "controls_if":
                obj = parseControlsIf1(element);
                break;
            case "logic_operation":
                obj = parseLogicOperation(element);
                break;
            case "logic_negate":
                obj = parseLogicNegate(element);
                break;
            case "logic_null":
                obj = parseLogicNull(element);
                break;
            default:
                Console.WriteLine("Default case");
                break;
        }
        return obj;
    }

    private object parseLogicBoolean(XElement element)
    {
        element = BlocklyUtil.applyNameSpace(element);
        return element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("BOOL")).FirstOrDefault()?.Value;
        //return element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "field").Value;
    }

    private object parseLogicCompare(XElement element)
    {
        bool returnType = false;
        element = BlocklyUtil.applyNameSpace(element);
        string op = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("OP")).FirstOrDefault()?.Value;
        var valueBlocks = element.Element(BlocklyUtil.ns + "block").Elements(BlocklyUtil.ns + "value");
        XElement aElement = null;
        XElement bElement = null;
        foreach (XElement value in valueBlocks)
        {
            if (value.Attribute("name").Value.Equals("A"))
            {
                aElement = value;
            }
            else if (value.Attribute("name").Value.Equals("B"))
            {
                bElement = value;
            }
        }
        aElement = BlocklyUtil.applyNameSpace(aElement).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        bElement = BlocklyUtil.applyNameSpace(bElement).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        object a = null;
        object b = null;

        if (aElement != null)
        {
            a = eventObj.parseBlock(aElement);
        }
        if (bElement != null)
        {
            b = eventObj.parseBlock(bElement);
        }
        if (a != null && a.GetType().Equals(typeof(BlocklyReference)))
        {
            BlocklyReference bRefTemp = (BlocklyReference)a;
            a = bRefTemp.value;
        }
        if (b != null && b.GetType().Equals(typeof(BlocklyReference)))
        {
            BlocklyReference bRefTemp = (BlocklyReference)b;
            b = bRefTemp.value;
        }
        if (a == null && b == null)
        {
            returnType = true;
        }

        if (a != null && b != null)
        {
            returnType = compareValues(a, b, op);
        }
        eventObj.parseNextBlock(element);
        return returnType;
    }

    private object parseControlsIf(XElement element)
    {
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        XElement doStatements = element.Descendants(BlocklyUtil.ns + "statement").Where(child => child.Attribute("name").Value.StartsWith("DO", StringComparison.Ordinal)).FirstOrDefault();
        XElement logicBlock = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        obj = eventObj.parseBlock(logicBlock);
        bool b = false;
        if (obj.GetType().Equals(typeof(BlocklyReference)))
        {
            //b = (bool)((BlocklyReference)obj).value;
            bool.TryParse(((BlocklyReference)obj).value.ToString(), out b);
        } else
        {
            //b = (bool)obj;
            bool.TryParse(obj.ToString(),out b);
        }
        if (b)
        {
            doStatements = BlocklyUtil.applyNameSpace(doStatements).Element(BlocklyUtil.ns + "statement").Element(BlocklyUtil.ns + "block");
            obj = eventObj.parseBlock(doStatements);
        }
        else
        {
            //TODO implement this
        }
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseControlsIf1(XElement element)
    {
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        XElement elseStatement = element.Descendants(BlocklyUtil.ns + "statement").Where(child => child.Attribute("name").Value.Equals("ELSE", StringComparison.Ordinal)).FirstOrDefault();

        var conditions = element.Element(BlocklyUtil.ns + "block").Elements(BlocklyUtil.ns + "value");
        bool b = false;
        foreach (XElement elem in conditions)
        {
            int iterations = 0;
            string conditionName = elem.Attribute("name").Value;
            conditionName = conditionName.Remove(0, "IF".Length); //Essentially getting the digit part of the value like IF0, then get 0
            int.TryParse(conditionName, out iterations);
            XElement doStatements = element.Descendants(BlocklyUtil.ns + "statement").Where(child => child.Attribute("name").Value.Equals("DO"+ iterations, StringComparison.Ordinal)).FirstOrDefault();
            XElement logicBlock = BlocklyUtil.applyNameSpace(elem).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
            obj = eventObj.parseBlock(logicBlock);
            
            if (obj.GetType().Equals(typeof(BlocklyReference)))
            {
                //b = (bool)((BlocklyReference)obj).value;
                bool.TryParse(((BlocklyReference)obj).value.ToString(), out b);
            }
            else
            {
                //b = (bool)obj;
                bool.TryParse(obj.ToString(), out b);
            }
            if (b)
            {
                doStatements = BlocklyUtil.applyNameSpace(doStatements).Element(BlocklyUtil.ns + "statement").Element(BlocklyUtil.ns + "block");
                obj = eventObj.parseBlock(doStatements);
                break;
            }
            else
            {
                //TODO implement this, not sure what to do
                continue;
            }
        }
        if (!b && elseStatement != null)
        {
            elseStatement = BlocklyUtil.applyNameSpace(elseStatement).Element(BlocklyUtil.ns + "statement").Element(BlocklyUtil.ns + "block");
            obj = eventObj.parseBlock(elseStatement);
        }
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseLogicOperation(XElement element)
    {
        bool returnType = false;
        element = BlocklyUtil.applyNameSpace(element);
        string op = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("OP")).FirstOrDefault()?.Value;
        var valueBlocks = element.Element(BlocklyUtil.ns + "block").Elements(BlocklyUtil.ns + "value");
        XElement aElement = null;
        XElement bElement = null;
        foreach (XElement value in valueBlocks)
        {
            if (value.Attribute("name").Value.Equals("A"))
            {
                aElement = value;
            } else if (value.Attribute("name").Value.Equals("B"))
            {
                bElement = value;
            }
        }
        
        aElement = BlocklyUtil.applyNameSpace(aElement).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        bElement = BlocklyUtil.applyNameSpace(bElement).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        object a = null;
        object b = null;
        bool aSide = false;
        bool bSide = false;

        if (aElement != null)
        {
            a = eventObj.parseBlock(aElement);
        }
        if (bElement != null)
        {
            b = eventObj.parseBlock(bElement);
        }
        if (a != null)
        {
            bool.TryParse(BlocklyUtil.getStringFromObj(a), out aSide);
        }
        if (b != null)
        {
            bool.TryParse(BlocklyUtil.getStringFromObj(b), out bSide);
        }

        if (op.Equals("AND"))
        {
            returnType = aSide && bSide;
        } else if (op.Equals("OR"))
        {
            returnType = aSide || bSide;
        }

        eventObj.parseNextBlock(element);
        return returnType;
    }

    private object parseLogicNegate(XElement element)
    {
        bool returnType = false;
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        XElement logicBlock = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        if(logicBlock != null)
        {
            obj = eventObj.parseBlock(logicBlock);
        }
        if (obj != null)
        {
            bool.TryParse(BlocklyUtil.getStringFromObj(obj), out returnType);
        }

        return !returnType;
    }

    private object parseLogicNull(XElement element)
    {
        eventObj.parseNextBlock(element);
        return null;
    }

    private bool compareValues(object a, object b, string op)
    {
        switch (op)
        {
            case "EQ":
                if (a.Equals(b) || a.ToString().Equals(b.ToString()))
                {
                    return true;
                }
                else
                {
                    //TODO may be there is a better way of doing it
                    return false;
                }
            case "NEQ":
                if (a.Equals(b) || a.ToString().Equals(b.ToString()))
                {
                    return false;
                }
                else
                {
                    //TODO may be there is a better way of doing it
                    return true;
                }
            case "LT":
                double firstLT = double.Parse(a.ToString());
                double secondLT = double.Parse(b.ToString());
                if (firstLT < secondLT)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "LTE":
                double firstLTE = double.Parse(a.ToString());
                double secondLTE = double.Parse(b.ToString());
                if (firstLTE <= secondLTE)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "GT":
                double firstGT = double.Parse(a.ToString());
                double secondGT = double.Parse(b.ToString());
                if (firstGT > secondGT)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "GTE":
                double firstGTE = double.Parse(a.ToString());
                double secondGTE = double.Parse(b.ToString());
                if (firstGTE >= secondGTE)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                break;
        }
        return false;
    }
}
