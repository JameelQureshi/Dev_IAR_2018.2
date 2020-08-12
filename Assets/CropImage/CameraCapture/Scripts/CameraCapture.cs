using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CameraCapture : MonoBehaviour {

	#if !UNITY_EDITOR && UNITY_ANDROID
	private static AndroidJavaClass mPVC = null;
	private static AndroidJavaClass PVC
	{
		get
		{
			if( mPVC == null )
			mPVC = new AndroidJavaClass( "com.Wili.CameraCapture.CameraCapture" );
			return mPVC;
		}
	}
	#endif


	#if !UNITY_EDITOR && UNITY_IOS

	//[System.Runtime.InteropServices.DllImport( "__Internal" )]
	//private static extern void iTakePhoto();

	//[System.Runtime.InteropServices.DllImport( "__Internal" )]
	//private static extern void iPickPhoto();

	#endif

	public delegate void MediaDelegate(string path);
	public delegate void ErrorDelegate(string message);

	public event MediaDelegate PickCompleted;

	public  event MediaDelegate TakePhotoCompleted;

	public event ErrorDelegate Failed;

	public void takePhoto()
	{
		//#if !UNITY_EDITOR && UNITY_ANDROID
		//if(PVC != null)
		//{
		//    PVC.CallStatic("TakePhoto");
		//}
		//#elif !UNITY_EDITOR && UNITY_IOS
		//iTakePhoto();
		//#endif
	}

	public void pickPhoto()
	{
		//#if !UNITY_EDITOR && UNITY_ANDROID
		//if(PVC != null)
		//{
		//	PVC.CallStatic("PickPhoto");
		//}
		//#elif !UNITY_EDITOR && UNITY_IOS
		//iPickPhoto();
		//#endif
	}

	private void OnTakePhotoComplete(string path)
	{
		//var handler = TakePhotoCompleted;
		//if (handler != null)
		//{
		//	handler(path);
		//}
	}

	private void OnPickComplete(string path)
	{
		//var handler = PickCompleted;
		//if (handler != null)
		//{
		//	handler(path);
		//}
	}

	private void OnFailure(string message)
	{
		//var handler = Failed;
		//if (handler != null)
		//{
		//	handler(message);
		//}
	}
}
