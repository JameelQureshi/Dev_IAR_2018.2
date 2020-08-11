using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconButtonScript : MonoBehaviour {
    //List<string> iconList = new List<string>() { "Button_3", "Button_5", "Button_7" };
    // Use this for initialization
    public string info;
    public float posX;
    public float posY;
    public string prefix;
    public float sizeRatio;
    string url = "https://dl.dropboxusercontent.com/s/ju6lci5zjdpom06/TableData_update.json";
	void Start () {
		
	}

    public void  clicked()
    {
        //string spriteName = this.gameObject.GetComponent<Image>().sprite.name;
        Debug.Log("<color=green> @@@@@@@@@@ button name: </color>" + this.gameObject.name);
        Debug.Log("<color=green> @@@@@@@@@@ info: </color>" + info);
        Debug.Log("<color=green> @@@@@@@@@@ prefix: </color>" + prefix);
        if (!string.IsNullOrEmpty(info)){
            if (!string.IsNullOrEmpty(prefix))
            {
                if (prefix.Equals("info", System.StringComparison.OrdinalIgnoreCase))
                {
                    PopupUtilities.makePopupText(this.gameObject,null, info, true, null);
                }
                else if (prefix.Equals("table", System.StringComparison.OrdinalIgnoreCase))
                {
                    if(info.Contains("dropbox")){
                        url = info;
                    }
                    info = PopupUtilities.getTableStirngFromDropBox(url);
                    PopupUtilities.makePopupTable(this.gameObject,null, info, true, null);
                }
                else if (prefix.Equals("prompt", System.StringComparison.OrdinalIgnoreCase))
                {
                    PopupUtilities.makePopupYesNo(this.gameObject,null, info, true, null);
                }
                else
                {
                    //TODO Lax : Find if there are other options
                    PopupUtilities.makePopupText(this.gameObject,null, info, true, null);
                }
            }
            else{
                PopupUtilities.makePopupText(this.gameObject,null, info, true, null);
            }
        }
    }

}
