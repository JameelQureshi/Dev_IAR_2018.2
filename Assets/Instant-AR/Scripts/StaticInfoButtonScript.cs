using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaticInfoButtonScript : MonoBehaviour
{
    public string url;
    public string string_value1;
    public string string_value2;
    public string string_value3;


    public void changeCurrentEquipment()
    {
        if (!string.IsNullOrEmpty(string_value1) && string_value1.ToLower().Equals("equipmentno"))
        {
            if (!string.IsNullOrEmpty(string_value2))
            {
                GlobalVariables.CURRENT_KEYSIGHT_ASSET = string_value2;
            }
            if (!string.IsNullOrEmpty(string_value3))
            {
                if(string_value3.ToLower().Equals("green")){
                    GlobalVariables.CURRENT_KEYSIGHT_HELATH = "green";
                }
                else if (string_value3.ToLower().Equals("yellow"))
                {
                    GlobalVariables.CURRENT_KEYSIGHT_HELATH = "yellow";
                }
                else if (string_value3.ToLower().Equals("red"))
                {
                    GlobalVariables.CURRENT_KEYSIGHT_HELATH = "red";
                }
            }
        }

    }
}
