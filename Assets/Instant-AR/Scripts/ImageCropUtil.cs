using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ImageAndVideoPicker;
using System.IO;
using System.Xml;
using System.Net;
using System.Collections.Specialized;
using System;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;

public class ImageCropUtil : MonoBehaviour
{


    private static String myToken = GlobalVariables.DROPBOX_TOCKEN;
    private String targetFolder = GlobalVariables.DROPBOX_STAGING_FOLDER;
    private AuthManager _authManager;


    public void uploadImage(string imagePath)
    {
        StartCoroutine(testFileBrowserAsynch(imagePath));
    }



    private IEnumerator testFileBrowserAsynch(String sourcePath)
    {
        Debug.Log("<color=red> Inside testFileBrowserAsynch   </color>" + sourcePath);
        String targetPath;
        String uid = SystemInfo.deviceUniqueIdentifier + "/";
        String filename = sourcePath.Substring(sourcePath.LastIndexOf("/") + 1);
        String fileType = filename.Substring(filename.LastIndexOf(".") + 1);
        if ((fileType.ToLower() == "jpg") || (fileType.ToLower() == "png"))
        {
            _authManager = AuthManager.Instance;
            if (_authManager.IsLoggedIn)
            {
                string subFolder = _authManager.CurrentToken.username;
                //Substring(sourcePath.LastIndexOf("/") + 1);
                subFolder = subFolder.Substring(0, subFolder.LastIndexOf("."));
                Debug.Log(">>>>>>>>> Inside UploadImage..the subFolder is:  " + subFolder);
                targetPath = targetFolder + subFolder + "/" + filename;
            }
            else
            {
                targetPath = targetFolder + uid + filename;
            }

            Dictionary<string, string> postHeader = new Dictionary<string, string>();
            String param = "{\"path\": \"" + targetPath + "\",\"mode\": \"overwrite\",\"autorename\": false,\"mute\": false}";
            postHeader.Add("Authorization", myToken);
            postHeader.Add("Dropbox-API-Arg", param);
            postHeader.Add("Content-Type", "application/octet-stream");
            byte[] myData = File.ReadAllBytes(sourcePath);
            int sourceSize = myData.Length;
            Debug.Log(">>>>>>>>>>>>>>>>>>>>sourceSize is" + sourceSize);
            WWW www = new WWW("https://content.dropboxapi.com/2/files/upload", myData, postHeader);
            //yield return ShowDownloadProgress(www);
            StartCoroutine(ShowProgress(www));
            yield return www;
            if (www.error == null)
            {
                Debug.Log("<color=white>   >>>>>>success: </color>" + www.text);
            }
            else
            {
                Debug.Log("<color=white>   >>>>>>something wrong:  </color>" + www.text);
            }
            JituMessageBox.DisplayMessageBox("AR Image Upload Status", "Awesome!\nYour AR Experience Video would be active shortly.\n", true, CloseDialog);
        }
        else
        {
            JituMessageBox.DisplayMessageBox("Image Upload Status", "The " + fileType.ToUpper() + " image format is not supported.Please try a JPG or PNG Image only !", true, CloseDialog);
            //yield return null;
        }
    }

    private IEnumerator ShowProgress(WWW www)
    {
        while (!www.isDone)
        {
            yield return new WaitForSeconds(.1f);
        }

        Debug.Log("Done");
    }


    public void CloseDialog()
    {
        Debug.Log("<color=red> $$$$$$$ >>  CloseDialog   </color>");
    }

}
