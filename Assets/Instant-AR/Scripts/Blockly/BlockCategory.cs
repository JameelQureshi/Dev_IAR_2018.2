using System;
public enum BlockCategory
{
    //UI BLocks 
    colour_picker,
    updateProperty,
    newUpdateProperty,
    ARQueryAll,
    ARQuery,
    UITableReport,
    CallElement,
    SendElement,
    Elements,
    WaitElement,
    fieldPopOverVideo,
    simpleEvent, //Gonna be a dummy implement for this as its tackled in testbuttonscript

    //API blocs
    NodeValue,
    QueryAndOr,
    ARQueryNew,
    ARQueryAllNew,

    //Math blocks
    math_number,
    math_arithmetic,
    math_number_property,
    math_round,
    math_modulo,
    math_random_int,
    math_on_list,

    //Text Blocks
    text,
    text_join,
    text_prompt_ext,
    text_length,
    text_changeCase,
    text_print,
    text_indexOf,
    text_charAt,
    text_trim,
    text_append,

    //Logic Blocks
    logic_boolean,
    controls_if,
    logic_compare,
    logic_operation,
    logic_negate,
    logic_null,

    //Loop Blocks
    controls_forEach,
    controls_whileUntil,
    controls_repeat_ext,
    controls_for,

    //List Blocks
    lists_setIndex,
    lists_create_with,
    lists_indexOf,
    lists_isEmpty,
    lists_getSublist,
    lists_length,
    lists_split,
    lists_sort,

    //Variable Blocks
    variables_set,
    variables_get,

    //Code Blocks
    procedures_defnoreturn,
    procedures_defreturn,
    procedures_callnoreturn,
    procedures_callreturn,

    //App/ARTracking blocks
    ARTracking,
    AppTracking
}
