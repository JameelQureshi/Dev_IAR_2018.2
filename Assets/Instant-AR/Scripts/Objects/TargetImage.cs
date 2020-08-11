/*
 * ============================================================================== 
 * Copyright (c) 2019-2020 EmachineLabs Corp. All Rights Reserved.
 * 
 * @Author : Jitender Hooda 
 * 
 ==============================================================================
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;
public class TargetImage
{
    public string targetID;     public string metaData;     public string fileName;     public string fileType;     public string aspectRatio;     public string byteSize;     public string serverImagePath;     public string dbxVideoURL;     public string vuforiaImageStatus;     public string uniqueID;     public string uniqueTargetId; //its duplicate of uniqueID but will be removed after resulution with ImageDetils     public string videoPortrait;     public string imagePortrait;     public string defaultView;     public string imageWidth;     public string imageHeight;     public string dbxImageURL;     public string userID;     public string dbxJsonURL;     public string createDate;     public string modifyDate;     public string availableNewIconNames;     public string videoType;      public string infoDataKey;     public string infoDataPublicKey;      public string dbxDocumentURL1;     public string dbxDocumentURL2;     public string dbxDocumentURL3;      public string inGroup;     public string groupID;      public string modifyDatestring;


    public ButtonDetails[] buttons;
    public VideoData[] videos;
    public DocumentData[] docs;
    public NotesData[] notes;


    //public TargetInfo[] targetInfos;


}
