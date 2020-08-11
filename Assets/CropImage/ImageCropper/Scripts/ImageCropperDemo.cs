using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;


namespace ImageCropperNamespace
{
	public class ImageCropperDemo : MonoBehaviour
	{

        private static string myToken = GlobalVariables.DROPBOX_TOCKEN;
        private string targetFolder = GlobalVariables.DROPBOX_STAGING_FOLDER;
        private AuthManager _authManager;


        public void Crop(Texture2D tex)
		{
			// If image cropper is already open, do nothing
			if( ImageCropper.Instance.IsOpen )
				return;

			StartCoroutine( TakeScreenshotAndCrop(tex) );
		}

        Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

        private IEnumerator TakeScreenshotAndCrop(Texture2D tex)
		{
			yield return new WaitForEndOfFrame();

			bool autoZoom = false;

			//Texture2D screenshot = new Texture2D( Screen.width, Screen.height/2, TextureFormat.RGB24, false );
			//screenshot.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height/2 ), 0, 0 );
			//screenshot.Apply();

            ImageCropper.Instance.Show(tex, (bool result, Texture originalImage, Texture2D croppedImage) =>
           {
                // If screenshot was cropped successfully
                if (result)
               {
                   string stFileName = System.DateTime.Now.ToString("yyyyMMdd_") +
                                     System.DateTime.Now.Hour.ToString("D2") +
                                     System.DateTime.Now.Minute.ToString("D2") +
                                     System.DateTime.Now.Second.ToString("D2") + ".jpg";

                   FindObjectOfType<DemoScene>().pickPreiveimage.sprite = Sprite.Create(croppedImage,
                      new Rect(Vector2.zero, new Vector2(croppedImage.width, croppedImage.height)),
                      Vector2.one * 0.5f);

                   NativeGallery.SaveImageToGallery(duplicateTexture(croppedImage), "/", stFileName);
                   UploadImage2DBX(duplicateTexture(croppedImage), stFileName);
               }
               else
               {

               }

                // Destroy the screenshot as we no longer need it in this case
                Destroy(tex);
           },
            settings: new ImageCropper.Settings()
            {
                autoZoomEnabled = autoZoom
			},
			croppedImageResizePolicy: ( ref int width, ref int height ) =>
			{
				// uncomment lines below to save cropped image at half resolution
				//width /= 2;
				//height /= 2;
			} );
		}

        //jitu adding

       
        public void UploadImage2DBX(Texture2D image,string sourcePath)
        {
            StartCoroutine(UploadImage2DBXAsynch(image, sourcePath));
        }




        private IEnumerator UploadImage2DBXAsynch(Texture2D image, string sourcePath)
        {

            byte[] myData = null;

            Debug.Log("<color=red> Inside testFileBrowserAsynch   </color>" + sourcePath);
            string targetPath;
            string uid = SystemInfo.deviceUniqueIdentifier + "/";
            string filename = sourcePath.Substring(sourcePath.LastIndexOf("/") + 1);
            string fileType = filename.Substring(filename.LastIndexOf(".") + 1);

           
            if ((fileType.ToLower() == "jpg") || (fileType.ToLower() == "jpeg") || (fileType.ToLower() == "png"))
            {
                if((fileType.ToLower() == "jpg") || (fileType.ToLower() == "jpeg"))
                {
                    myData = image.EncodeToJPG(100);
                }
                else if ((fileType.ToLower() == "png"))
                {
                    myData = image.EncodeToPNG();
                }
                else
                {
                    myData = image.EncodeToPNG();
                }

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

                Debug.Log(">>>>>>>>> Image targetPath is:  " + targetPath);

                Dictionary<string, string> postHeader = new Dictionary<string, string>();
                string param = "{\"path\": \"" + targetPath + "\",\"mode\": \"overwrite\",\"autorename\": false,\"mute\": false}";
                postHeader.Add("Authorization", myToken);
                postHeader.Add("Dropbox-API-Arg", param);
                postHeader.Add("Content-Type", "application/octet-stream");
                int sourceSize = myData.Length;
                Debug.Log(">>>>>>>>>>>>>>>>>>>>sourceSize is" + sourceSize);
                WWW www = new WWW("https://content.dropboxapi.com/2/files/upload", myData, postHeader);
                //yield return ShowDownloadProgress(www);
                StartCoroutine(smoothUpload(www));
                yield return www;
                if (www.error == null)
                {
                    Debug.Log("<color=white>   >>>>>>success: </color>" + www.text);
                }
                else
                {
                    Debug.Log("<color=white>   >>>>>>something wrong:  </color>" + www.text);
                }
                JituMessageBox.DisplayMessageBox("AR Image Status", "Awesome!\nYour AR Experience for this Image would be active shortly.\n", true, CloseDialog);
            }
            else
            {
                JituMessageBox.DisplayMessageBox("Image Upload Status", "The " + fileType.ToUpper() + " image format is not supported.Please try a JPG or PNG Image only !", true, CloseDialog);
            }
        }

        private IEnumerator smoothUpload(WWW www)
        {
            while (!www.isDone)
            {
                yield return new WaitForSeconds(.1f);
            }
        }


        public void CloseDialog()
        {
            Debug.Log("<color=red> $$$$$$$ >>  CloseDialog   </color>");
        }





    }
}