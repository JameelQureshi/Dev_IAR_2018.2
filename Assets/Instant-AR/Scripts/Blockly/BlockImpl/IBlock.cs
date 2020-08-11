using System;
using System.Xml.Linq;

public interface IBlock
{
    object parse(BlocklyEvents eventObject, string blockType, string codeBlockName, XElement element);
}
