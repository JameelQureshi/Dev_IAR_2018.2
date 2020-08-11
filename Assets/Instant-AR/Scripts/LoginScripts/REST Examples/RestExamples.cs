using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RestExamples: MonoBehaviour {

	private AuthManager _authManager;
	private SecureRestfulApi _secureRestfulApi;

	void Awake() {
		_authManager = AuthManager.Instance;
		_secureRestfulApi = SecureRestfulApi.Instance(_authManager);
		
	}

	/// <summary>
	/// Place any test code you would like to run within this. Usually you would make a test method within this class
	/// and call it from within this method. That way the tests can be swapped out quickly.
	/// </summary>
	public void TestRequest() {
		TestJsonAuthRequest();
		
		
		/*
		 * NOTE: If you would like to test the raw RESTful API's then use the RESTFUL_TEST_URL_INSECURE url.
		 * 		 That url is insecure so you don't need to use any authentication with it. That means that you
		 * 		 can use the raw RestfulApi.cs methods. All of the endpoints within the RESTFUL_TEST_URL_INSECURE
		 * 		 url return JSON data.
		 */
		
	}

	#region Raw RESTful tests
	/// <summary>
	/// This method tests a simple request that will be sent to the server. This usually involves just a GET method
	/// that will return some JSON data or a simple string response.
	///
	/// You can add any url query parameters to the request as well. You can do this by either manually adding the
	/// parameters to the url OR you can pass in a dictionary of key/value pairs and allow the API to handle adding
	/// the parameters for you (recommended).
	/// </summary>
	private void TestSimpleRequest() {
		// Set any url (you can use the prebuilt ones or try a custom one.
		var url = ConfigurationManager.RESTFUL_TEST_URL_INSECURE;

		// Create any url parameters that should be appended to the url
		var qParams = new Dictionary<string, string>();
		qParams.Add("param1", "value1");
		qParams.Add("param2", "value2");
		
		// Set any callbacks. These are code blocks that will be called after the request has finished. They will be
		// passed certain information from the request that you can use to trigger events or process any functionality.		
		// Please see docs within "requestCallbacks.cs for more information about callbacks
		
//		SimpleRequestCallback callback = delegate(string response, UnityWebRequest obj, bool error) {
//			Debug.Log("Sent request");
//		};
		
		// 
		JsonRequestCallback callback = delegate(Dictionary<string, object> data, UnityWebRequest obj, bool error, bool loginNeeded) { 
			Debug.Log("Recieved Response");
		};
		
		// Each of the "RestfulApi" calls need to be started from a MonoBehaviour class so the 
		// StartCoroutine method can be used. This is so the main thread isn't blocked.
		StartCoroutine(RestfulApi.sendRequest(HttpMethod.GET, url, jsonCallback: callback, queryParams: qParams));
	}

	/// <summary>
	/// This method tests a request that sends JSON to an endpoint/server. This may include any HTTP verb.
	///
	/// Changing the HTTP verbe should be done by passing in a HttpMethod enum value. The values are defined within
	/// the RestfulApi.cs file at the top of the file.
	/// </summary>
	private void TestJsonRequest() {	
		// Set any url (you can use the prebuilt ones or try a custom one.
		var url = ConfigurationManager.RESTFUL_TEST_URL_INSECURE;

		// Get a sample Dictionary containing JSON data to send to the server.
		var jsonDict = GetTestJsonDict();
		
		// Set any callbacks. These are code blocks that will be called after the request has finished. They will be
		// passed certain information from the request that you can use to trigger events or process any functionality.		
		// Please see docs within "requestCallbacks.cs for more information about callbacks
		
//		SimpleRequestCallback callback = delegate(string response, UnityWebRequest obj, bool error) {
//			Debug.Log("Sent request");
//		};
		
		JsonRequestCallback callback = delegate(Dictionary<string, object> data, UnityWebRequest obj, bool error, bool loginNeeded) { 
			Debug.Log("Recieved Response");
		};
		
		// Each of the "RestfulApi" calls need to be started from a MonoBehaviour class so the 
		// StartCoroutine method can be used. This is so the main thread isn't blocked.
		// Can be used with ANY Http verb (i.e. HttpMethod.POST, GET, CREATE, PUT, DELETE)
		StartCoroutine(RestfulApi.sendJsonRequest(HttpMethod.PUT, url, jsonDict, jsonCallback: callback ));
	}
	
	/// <summary>
	/// This method tests a request that sends form data to an endpoint/server. This may include any HTTP verb.
	/// 
	/// Changing the HTTP verbe should be done by passing in a HttpMethod enum value. The values are defined within
	/// the RestfulApi.cs file at the top of the file.
	/// </summary>
	private void TestFormRequest() {
		// Set any url (you can use the prebuilt ones or try a custom one.
		var url = ConfigurationManager.RESTFUL_TEST_URL_INSECURE;

		// Get a sample Dictionary containing JSON data to send to the server.
		var formData = new Dictionary<string, string>();
		formData.Add("key1", "value1");
		formData.Add("key2", "value2");
		formData.Add("key3", "value3");
		
		// Set any callbacks. These are code blocks that will be called after the request has finished. They will be
		// passed certain information from the request that you can use to trigger events or process any functionality.		
		// Please see docs within "requestCallbacks.cs for more information about callbacks
		
//		SimpleRequestCallback callback = delegate(string response, UnityWebRequest obj, bool error) {
//			Debug.Log("Sent request");
//		};
		
		JsonRequestCallback callback = delegate(Dictionary<string, object> data, UnityWebRequest obj, bool error, bool loginNeeded) { 
			Debug.Log("Recieved Response");
		};
		
		// Each of the "RestfulApi" calls need to be started from a MonoBehaviour class so the 
		// StartCoroutine method can be used. This is so the main thread isn't blocked.
		// This currently only uses the POST HTTP verb.
		StartCoroutine(RestfulApi.sendFormRequest(url, formData, jsonCallback: callback ));
	}
	#endregion
	
	#region Secure RESTful tests
	/// <summary>
	/// This method tests a simple authenticated call using a GET request.
	///
	/// NOTE: YOU MUST BE LOGGED IN FOR THIS TO WORK.
	/// </summary>
	/// <remarks>
	/// ALL secure RESTful API's must be passed a Monobehaviour object. This is so the secure methods can call
	/// Coroutines to refresh any authentication token.
	/// </remarks>
	private void TestSimpleAuthRequest() {
		// Set any url (you can use the prebuilt ones or try a custom one.
		var url = ConfigurationManager.RESTFUL_TEST_URL;
//		var url = ConfigurationManager.RESTFUL_JSON_TEST_URL;
//		var url = ConfigurationManager.RESTFUL_ERROR_TEST_URL;

		// Create any url parameters that should be appended to the url
		var qParams = new Dictionary<string, string>();
		qParams.Add("param1", "value1 2");
		qParams.Add("param2", "value2");
		
		// Set any callbacks. These are code blocks that will be called after the request has finished. They will be
		// passed certain information from the request that you can use to trigger events or process any functionality.		
		// Please see docs within "requestCallbacks.cs for more information about callbacks
		
		SimpleRequestCallback callback = delegate(string response, UnityWebRequest obj, bool error, bool loginNeeded) {
			Debug.Log("Sent request");
		};
		 
		// Sample JsonRequest Callback
//		JsonRequestCallback callback = delegate(Dictionary<string, object> data, UnityWebRequest obj, bool error, bool loginNeeded) { 
//			Debug.Log("Recieved Response");
//		};
		
		// Each of the "SecureRestfulApi"s need to have a MonoBehaviour object passed in so it can make the 
		// network requests with StartCourouting.
		_secureRestfulApi.sendRequest(this, HttpMethod.GET, url, callback, queryParams: qParams);
	}
	
	/// <summary>
	/// This method tests a request that sends JSON to an endpoint/server. This may include any HTTP verb.
	///
	/// Changing the HTTP verbe should be done by passing in a HttpMethod enum value. The values are defined within
	/// the RestfulApi.cs file at the top of the file.
	/// 
	/// NOTE: YOU MUST BE LOGGED IN FOR THIS TO WORK.
	/// </summary>
	/// <remarks>
	/// ALL secure RESTful API's must be passed a Monobehaviour object. This is so the secure methods can call
	/// Coroutines to refresh any authentication token.
	/// </remarks>
	private void TestJsonAuthRequest() {	
		// Set any url (you can use the prebuilt ones or try a custom one.
//		var url = ConfigurationManager.RESTFUL_TEST_URL;
		var url = ConfigurationManager.RESTFUL_JSON_TEST_URL;
//		var url = ConfigurationManager.RESTFUL_ERROR_TEST_URL;

		// Get a sample Dictionary containing JSON data to send to the server.
		var jsonDict = GetTestJsonDict();
		
		// Set any callbacks. These are code blocks that will be called after the request has finished. They will be
		// passed certain information from the request that you can use to trigger events or process any functionality.		
		// Please see docs within "requestCallbacks.cs for more information about callbacks
		
//		SimpleRequestCallback callback = delegate(string response, UnityWebRequest obj, bool error) {
//			Debug.Log("Sent request");
//		};
		
		JsonRequestCallback callback = delegate(Dictionary<string, object> data, UnityWebRequest obj, bool error, bool loginNeeded) { 
			Debug.Log("Recieved Response");
		};
		
		// Each of the "RestfulApi" calls need to be started from a MonoBehaviour class so the 
		// StartCoroutine method can be used. This is so the main thread isn't blocked.
		// Can be used with ANY Http verb (i.e. HttpMethod.POST, GET, CREATE, PUT, DELETE)
		_secureRestfulApi.sendJsonRequest(this, HttpMethod.POST, url, jsonDict, jsonCallback: callback);
	}
	#endregion

	
	/// <summary>
	/// Used to create a test dictionary to send to the server. Mainly used to test sending JSON data to the server.
	/// </summary>
	/// <returns>A dictionary object representing the JSON data that will be sent to the server.</returns>
	private Dictionary<string, object> GetTestJsonDict() {
		// First create sub-lists or sub-dictionaries to add later to the json dictionary
		var subList = new List<string>();
		subList.Add("Lisa");
		subList.Add("Bobert");
		
		var subDict = new Dictionary<string, object>();
		subDict.Add("hair_color", "blonde");
		subDict.Add("height", "7'");
		
		// Create the actual JSON dictionary that represents the shape/form of the JSON data that will be used later on.
		var jsonDict = new Dictionary<string, object>();
		jsonDict.Add("sent_from", "Unity3D");
		jsonDict.Add("age", 37);
		jsonDict.Add("tall", true);
		jsonDict.Add("family", subList);
		jsonDict.Add("attributes", subDict);

		return jsonDict;
	}
    
}