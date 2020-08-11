
/*  * ==============================================================================   * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.  *   * @Author : Jitender Hooda   *   ==============================================================================  */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetInvokeHandler : MonoBehaviour {

    public string assetType;
    public string assetURL;

	void Start () {
		
	}

    public void InvokeAsset()
    {
        if (assetType != null)
        {

            switch (assetType)
            {
                case "Image":
                    //Audio, Video, ThreeDObject, Image, Document, Icon
                    //PopupUtilities.makePopupImage(null, null, true, true, assetURL, true, Color.red, 8);
                    break;

                case "Video":
                    PopupUtilities.makePopupVideo(null, null, true, true, assetURL, true, Color.red, 8);
                    break;

                case "Audio":
                    //PopupUtilities.makePopupAudio(null, null, true, true, assetURL, true, Color.red, 8);
                    break;

                case "Document":
                    //PopupUtilities.makePopupDocument(null, null, true, true, assetURL, true, Color.red, 8);
                    break;

                case "Icon":
                    //PopupUtilities.makePopupIcon(null, null, true, true, assetURL, true, Color.red, 8);
                    break;

                case "ThreeDObject":
                    //PopupUtilities.makePopupThreeDObject(null, null, true, true, assetURL, true, Color.red, 8);
                    break;

            }





        }
    }


}
