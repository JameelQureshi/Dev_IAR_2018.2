using System;
using System.Collections.Generic;
using System.Xml.Linq;

public class BlockFactory
{
    private readonly Dictionary<BlockCategory, IBlock> blockImpl;

    public BlockFactory()
    {
        blockImpl = new Dictionary<BlockCategory, IBlock>
        {
            //UI BLocks 
            {BlockCategory.procedures_defnoreturn, new CodeBlocksBlockImpl()},
            {BlockCategory.procedures_defreturn, new CodeBlocksBlockImpl()},
            {BlockCategory.procedures_callnoreturn, new CodeBlocksBlockImpl()},
            {BlockCategory.procedures_callreturn, new CodeBlocksBlockImpl()},

            //Math blocks
            {BlockCategory.math_number, new MathBlockImpl()},
            {BlockCategory.math_arithmetic, new MathBlockImpl()},
            {BlockCategory.math_number_property, new MathBlockImpl()},
            {BlockCategory.math_round, new MathBlockImpl()},
            {BlockCategory.math_modulo, new MathBlockImpl()},
            {BlockCategory.math_random_int, new MathBlockImpl()},
            {BlockCategory.math_on_list, new MathBlockImpl()},

            //Text Blocks
            {BlockCategory.text, new TextBlockImpl()},
            {BlockCategory.text_join, new TextBlockImpl()},
            {BlockCategory.text_prompt_ext, new TextBlockImpl()},
            {BlockCategory.text_length, new TextBlockImpl()},
            {BlockCategory.text_changeCase, new TextBlockImpl()},
            {BlockCategory.text_print, new TextBlockImpl()},
            {BlockCategory.text_indexOf, new TextBlockImpl()},
            {BlockCategory.text_charAt, new TextBlockImpl()},
            {BlockCategory.text_trim, new TextBlockImpl()},
            {BlockCategory.text_append, new TextBlockImpl()},
            
            //Logic Blocks
            {BlockCategory.logic_boolean, new LogicBlockImpl()},
            {BlockCategory.controls_if, new LogicBlockImpl()},
            {BlockCategory.logic_compare, new LogicBlockImpl()},
            {BlockCategory.logic_operation, new LogicBlockImpl()},
            {BlockCategory.logic_negate, new LogicBlockImpl()},
            {BlockCategory.logic_null, new LogicBlockImpl()},

            //Loop Blocks
            {BlockCategory.controls_forEach, new LoopsBlockImpl()},
            {BlockCategory.controls_whileUntil, new LoopsBlockImpl()},
            {BlockCategory.controls_repeat_ext, new LoopsBlockImpl()},
            {BlockCategory.controls_for, new LoopsBlockImpl()},

            //List Blocks
            {BlockCategory.lists_setIndex, new ListsBlockImpl()},
            {BlockCategory.lists_create_with, new ListsBlockImpl()},
            {BlockCategory.lists_indexOf, new ListsBlockImpl()},
            {BlockCategory.lists_isEmpty, new ListsBlockImpl()},
            {BlockCategory.lists_getSublist, new ListsBlockImpl()},
            {BlockCategory.lists_length, new ListsBlockImpl()},
            {BlockCategory.lists_split, new ListsBlockImpl()},
            {BlockCategory.lists_sort, new ListsBlockImpl()},

            //Variable Blocks
            {BlockCategory.variables_set, new VariablesBlockImpl()},
            {BlockCategory.variables_get, new VariablesBlockImpl()},

            //Code Blocks
            {BlockCategory.colour_picker, new UIBlocksBlockImpl()},
            {BlockCategory.updateProperty, new UIBlocksBlockImpl()},
            {BlockCategory.newUpdateProperty, new UIBlocksBlockImpl()},
            {BlockCategory.ARQueryAll, new UIBlocksBlockImpl()},
            {BlockCategory.ARQuery, new UIBlocksBlockImpl()},
            {BlockCategory.UITableReport, new UIBlocksBlockImpl()},
            {BlockCategory.CallElement, new UIBlocksBlockImpl()},
            {BlockCategory.SendElement, new UIBlocksBlockImpl()},
            {BlockCategory.Elements, new UIBlocksBlockImpl()},
            {BlockCategory.WaitElement, new UIBlocksBlockImpl()},
            {BlockCategory.fieldPopOverVideo, new UIBlocksBlockImpl()},
            {BlockCategory.simpleEvent, new UIBlocksBlockImpl()},

            //API Blocks
            {BlockCategory.NodeValue, new ApiBlockImpl()},
            {BlockCategory.QueryAndOr, new ApiBlockImpl()},
            {BlockCategory.ARQueryNew, new ApiBlockImpl()},
            {BlockCategory.ARQueryAllNew, new ApiBlockImpl()},

            //App, ARTracking Blocks
            {BlockCategory.ARTracking, new ARTrackingBlockImpl()},
            {BlockCategory.AppTracking, new ARTrackingBlockImpl()}
        };

        //foreach (BlockCategory category in Enum.GetValues(typeof(BlockCategory)))
        //{
        //    string categoryName = Enum.GetName(typeof(BlockCategory), category);
        //    categoryName = categoryName.Substring(0, categoryName.IndexOf("_", StringComparison.Ordinal));
        //    Type type = Type.GetType(categoryName + "BlockImpl");
        //    IBlock impl = (IBlock)Activator.CreateInstance(type) as IBlock;
        //    blockImpl.Add(category, impl);
        //}
    }

    public object parse(BlocklyEvents eventObject, BlockCategory category, string blockType, string codeBlockName, XElement element)
        => blockImpl[category].parse(eventObject, blockType, codeBlockName, element);
}
