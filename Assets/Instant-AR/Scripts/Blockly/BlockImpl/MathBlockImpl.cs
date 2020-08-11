using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public class MathBlockImpl : IBlock

{
    BlocklyEvents eventObj;
    public MathBlockImpl()
    {
    }

    public object parse(BlocklyEvents eventObject, string blockType, string codeBlockName, XElement element)
    {
        object obj = null;
        this.eventObj = eventObject;
        switch (blockType)
        {
            case "math_number":
                obj = parseMathNumber(element);
                break;
            case "math_arithmetic":
                obj = parseMathArithmetic(element);
                break;
            case "math_number_property":
                obj = parseMathNumberProperty(element);
                break;
            case "math_round":
                obj = parseMathRound(element);
                break;
            case "math_modulo":
                obj = parseMathModulo(element);
                break;
            case "math_random_int":
                obj = parseMathRandomInt(element);
                break;
            case "math_on_list":
                obj = parseMathOnList(element);
                break;
            default:
                Console.WriteLine("Default case");
                break;
        }
        return obj;
    }

    private object parseMathNumber(XElement element)
    {
        element = BlocklyUtil.applyNameSpace(element);
        return element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("NUM")).FirstOrDefault()?.Value;
    }

    private object parseMathNumberProperty(XElement element)
    {
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        string numberProperty = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("PROPERTY")).FirstOrDefault()?.Value;
        XElement valueBlock = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        if (valueBlock != null)
        {
            obj = eventObj.parseBlock(valueBlock);
        }
        if (obj != null)
        {
            if (obj.GetType().Equals(typeof(BlocklyReference)))
            {
                obj = getNumberProperty(int.Parse(((BlocklyReference)obj).value.ToString()), numberProperty);
            } else if (obj.GetType().Equals(typeof(int)))
            {
                obj = getNumberProperty(int.Parse(obj.ToString()), numberProperty);
            }
        }
        eventObj.parseNextBlock(element);
        return obj;
    }

    private object parseMathArithmetic(XElement element)
    {
        float returnVal = 0;
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

        //XElement aElement = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("A")).FirstOrDefault();
        //XElement bElement = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("B")).FirstOrDefault();
        XElement aElementShadow = BlocklyUtil.applyNameSpace(aElement).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "shadow");
        XElement bElementShadow = BlocklyUtil.applyNameSpace(bElement).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "shadow");
        aElement = BlocklyUtil.applyNameSpace(aElement).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        bElement = BlocklyUtil.applyNameSpace(bElement).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        if (aElement == null && aElementShadow != null)
        {
            //might be a shadow case where user typed value inside shadow block
            aElement = aElementShadow;
        }
        if (bElement == null && bElementShadow != null)
        {
            //might be a shadow case where user typed value inside shadow block
            bElement = bElementShadow;
        }
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

        if (a.GetType().Equals(typeof(BlocklyReference)))
        {
            BlocklyReference bRefTemp = (BlocklyReference)a;
            a = bRefTemp.value;
        }
        if (b.GetType().Equals(typeof(BlocklyReference)))
        {
            BlocklyReference bRefTemp = (BlocklyReference)b;
            b = bRefTemp.value;
        }

        if (a != null && b != null)
        {
            returnVal = applyArithmetic(float.Parse(a.ToString()), float.Parse(b.ToString()), op);
        }
        eventObj.parseNextBlock(element);
        return returnVal;
    }

    private object parseMathRound(XElement element)
    {
        double numberToRound = 0;
        int roundedNumber = 0;
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        string op = element.Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("OP")).FirstOrDefault()?.Value;
        XElement valueBlock = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        if (valueBlock == null)
        {
            XElement shadowBlock = element.Descendants(BlocklyUtil.ns + "shadow").Where(child => child.Attribute("type").Value.Equals("math_number")).FirstOrDefault();
            string stringNumber = BlocklyUtil.applyNameSpace(shadowBlock).Element(BlocklyUtil.ns + "shadow").Element(BlocklyUtil.ns + "field").Value;
            if (stringNumber != null)
            {
                numberToRound = float.Parse(stringNumber);
            }
        } else
        {
            obj = eventObj.parseBlock(valueBlock);
            string stringNumber = BlocklyUtil.getStringFromObj(obj);
            if (stringNumber != null)
            {
                numberToRound = double.Parse(stringNumber);
            }
        }
        switch (op)
        {
            case "ROUND":
                roundedNumber = (int)Math.Round(numberToRound, MidpointRounding.AwayFromZero);
                break;
            case "ROUNDUP":
                roundedNumber = (int)Math.Ceiling(numberToRound);
                break;
            case "ROUNDDOWN":
                roundedNumber = (int)Math.Floor(numberToRound);
                break;
            default:
                break;

        }
        eventObj.parseNextBlock(element);
        return roundedNumber;
    }

    private object parseMathModulo(XElement element)
    {
        object obj = null;
        element = BlocklyUtil.applyNameSpace(element);
        XElement valueDivident = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("DIVIDEND")).FirstOrDefault();
        XElement valueDividentBlock = BlocklyUtil.applyNameSpace(valueDivident).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        float divident = 0;


        XElement valueDivisor = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("DIVISOR")).FirstOrDefault();
        XElement valueDivisorBlock = BlocklyUtil.applyNameSpace(valueDivisor).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        float divisor = 0;

        if (valueDividentBlock != null)
        {
            obj = eventObj.parseBlock(valueDividentBlock);
            divident = float.Parse(BlocklyUtil.getStringFromObj(obj));
        } else
        {
            string shadowDivident = BlocklyUtil.applyNameSpace(valueDivident).Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("NUM")).FirstOrDefault()?.Value;
            divident = float.Parse(shadowDivident);
        }
        if (valueDivisorBlock != null)
        {
            obj = eventObj.parseBlock(valueDivisorBlock);
            divisor = float.Parse(BlocklyUtil.getStringFromObj(obj));
        } else
        {
            string shadowDivisor = BlocklyUtil.applyNameSpace(valueDivisor).Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("NUM")).FirstOrDefault()?.Value;
            divisor = float.Parse(shadowDivisor);
        }
        
        eventObj.parseNextBlock(element);
        return (divident%divisor);
    }

    private object parseMathRandomInt(XElement element)
    {
        object obj = null;
        float returnVal = 0;
        element = BlocklyUtil.applyNameSpace(element);
        XElement valueFrom = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("FROM")).FirstOrDefault();
        XElement valueFromBlock = BlocklyUtil.applyNameSpace(valueFrom).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        int from = 0;


        XElement valueTo = element.Descendants(BlocklyUtil.ns + "value").Where(child => child.Attribute("name").Value.Equals("TO")).FirstOrDefault();
        XElement valueToBlock = BlocklyUtil.applyNameSpace(valueTo).Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");
        int to = 0;

        if (valueFromBlock != null)
        {
            obj = eventObj.parseBlock(valueFromBlock);
            int.TryParse(BlocklyUtil.getStringFromObj(obj), out from);
        } else
        {
            string shadowFrom = BlocklyUtil.applyNameSpace(valueFrom).Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("NUM")).FirstOrDefault()?.Value;
            int.TryParse(shadowFrom, out from);
        }
        if (valueToBlock != null)
        {
            obj = eventObj.parseBlock(valueToBlock);
            int.TryParse(BlocklyUtil.getStringFromObj(obj), out to);
        } else
        {
            string shadowTo = BlocklyUtil.applyNameSpace(valueTo).Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("NUM")).FirstOrDefault()?.Value;
            int.TryParse(shadowTo, out to);
        }
        if (from > to)
        {
            returnVal = 0;
        } else
        {
            Random r = new System.Random();
            eventObj.parseNextBlock(element);
            returnVal = r.Next(from, to);
        }

        return returnVal;
    }

    private object parseMathOnList(XElement element)
    {
        object obj = null;
        float returnVal = 0;
        element = BlocklyUtil.applyNameSpace(element);
        string op = BlocklyUtil.applyNameSpace(element).Descendants(BlocklyUtil.ns + "field").Where(child => child.Attribute("name").Value.Equals("OP")).FirstOrDefault()?.Value;
        XElement valueBlock = element.Element(BlocklyUtil.ns + "block").Element(BlocklyUtil.ns + "value").Element(BlocklyUtil.ns + "block");

        if(valueBlock != null)
        {
            obj = eventObj.parseBlock(valueBlock);
            if (obj == null)
            {
                returnVal = 0;
            }
            if (obj.GetType().Equals(typeof(string)))
            {
                float.TryParse((string)obj, out returnVal);
            }
            else if (obj.GetType().Equals(typeof(BlocklyReference)))
            {
                List<float> values = new List<float>();
                BlocklyReference bRef = (BlocklyReference)obj;
                if (bRef.type.Equals("list") || bRef.type.Equals("ARQueryAll") || bRef.type.Equals("ARQuery"))
                {
                    if (bRef.value.GetType().Equals(typeof(List<float>)))
                    {
                        values = (List<float>)bRef.value;
                    } else if (bRef.value.GetType().Equals(typeof(List<string>)))
                    {
                        float temp = 0;
                        //values = ((List<string>)bRef.value).Select(s => float.Parse(s)).ToList();
                        //values = ((List<string>)bRef.value).Where(x => float.TryParse(x, out temp)).Select(x => temp).ToList();
                        values = ((List<string>)bRef.value).Where(x => float.TryParse(x, out temp)).Select(x => temp).ToList();
                    }
                    else if (bRef.value.GetType().Equals(typeof(List<object>)))
                    {
                        float temp = 0;
                        //values = ((List<object>)bRef.value).Select(s => float.Parse(s.ToString())).ToList();
                        values = ((List<object>)bRef.value).Where(x => float.TryParse(x.ToString(), out temp)).Select(x => temp).ToList();
                    }
                    else
                    {
                        float temp = 0;
                        //TODO LAX : fix it later with more appropriate implementation
                        //values = ((List<object>)bRef.value).Select(s => (string)s).ToList();
                        values = ((List<object>)bRef.value).Where(x => float.TryParse(x.ToString(), out temp)).Select(x => temp).ToList();
                    }
                    returnVal = applyMathOnList(values, op);
                } else
                {
                    float temp = 0;
                    //TODO LAX : not sure what to do here!!
                    //float.TryParse(bRef.value.ToString(), out returnVal);
                    values = ((List<object>)bRef.value).Where(x => float.TryParse(x.ToString(), out temp)).Select(x => temp).ToList();
                }
                returnVal = applyMathOnList(values, op);
                //returnVal = float.Parse(((BlocklyReference)obj).value.ToString());
            }
            else
            {
                float.TryParse(obj.ToString(), out returnVal);
            }
        }
        eventObj.parseNextBlock(element);
        return returnVal;
    }

    private bool getNumberProperty(int v, string numberProperty)
    {
        switch (numberProperty)
        {
            case "EVEN":
                if (v % 2 == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "ODD":
                if (v % 2 != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "PRIME":
                return BlocklyUtil.chkprime(v);
            case "WHOLE":
                return (v % 1 == 0) ? true : false;
            case "POSITIVE":
                return (v > 0) ? true : false;
            case "NEGATIVE":
                return (v < 0) ? true : false;
            case "DIVISIBLE_BY":
                //TODO implement it, need to get hold of XElement and then do parseblock
                break;
            default:
                break;
        }
        return false;
    }

    private float applyArithmetic(float v1, float v2, string op)
    {
        float retunrVal = 0;
        switch (op)
        {
            case "ADD":
                retunrVal = v1 + v2;
                break;
            case "MINUS":
                retunrVal = v1 - v2;
                break;
            case "MULTIPLY":
                retunrVal = v1 * v2;
                break;
            case "DIVIDE":
                retunrVal = v1 / v2;
                break;
            case "POWER":
                retunrVal = (float)Math.Pow(v1, v2);
                break;
            default:
                break;
        }
        return retunrVal;
    }

    private float applyMathOnList(List<float> values, string op)
    {
        float retunrVal = 0;
        //List<float> floatVals = convertToFloat(values);
        switch (op)
        {
            case "SUM":
                foreach(float v in values)
                {
                    retunrVal = retunrVal + v;
                }
                break;
            case "MIN":
                retunrVal = values.Min();
                break;
            case "MAX":
                retunrVal = values.Max();
                break;
            case "AVERAGE":
                retunrVal = values.Average();
                break;
            case "MEDIAN":
                //TODO implement it
                break;
            case "RANDOM":
                //TODO implement it
                break;
            default:
                break;
        }
        return retunrVal;
    }

    private List<float> convertToFloat(List<string> values)
    {
        List<float> floatVal = new List<float>();
        foreach(string v in values)
        {
            float val = 0;
            bool result = float.TryParse(v, out val);
            if (result)
            {
                floatVal.Add(float.Parse(v));
            }
        }
        return floatVal;
    }
}
