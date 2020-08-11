using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

using MiniJSON;

/// <summary>
/// HTTP method verbs to use for the RESTful api. These correspond to the UnityWebRequest's kHttp strings.
/// </summary>
public enum HttpMethod {
	CREATE,
	GET,
	POST,
	PUT,
	DELETE
	
}

/// <summary>
/// This class is the raw RESTful API. Use this whenever you need to make un-authenticated requests to any generic
/// RESTful API.
///
/// If you need calls that are automatically authenticated with a logged in user then use the calls within
/// SecureRestfulApi.cs
/// </summary>
public class RestfulApi {

	/// <summary>
	/// Sends a simple network request using optional url parameters.
	/// </summary>
	/// <param name="method">The HttpMethod to use for the request.</param>
	/// <param name="url">The url to use for the request</param>
	/// <param name="simpleCallback">Callback code to run after the request. Expects a simple string response.</param>
	/// <param name="jsonCallback">Callback code to run after the request finishes. This should expect a JSON response.</param>
	/// <param name="queryParams">
	/// Parameters to use for the request. They will be appended to the url upon request. The key/value pairs should
	/// be organized as "variable:value". The key/value pairs will be appended to the url before sending the request,
	/// ex. "http://www.mysite.com?key1=value1&key2=value2..."
	/// </param>
	/// <param name="headers">Optional. Dictionary of HTTP headers to use for the request.</param>
	public static IEnumerator sendRequest(
		HttpMethod method,
		string url, 
		SimpleRequestCallback simpleCallback = null,
		JsonRequestCallback jsonCallback = null, 
		Dictionary<string, string> queryParams = null,
		Dictionary<string, string> headers = null
	) {

		using (UnityWebRequest request = ConfigureRequestWithUrlParams(url, queryParams, headers)) {
			request.method = method.ToString();
			
			// Send the network request and wait for the response.
			yield return request.SendWebRequest();

			// If the request returns a response code between 400-599 then there was an error
			// Or if the request's "isNetworkError" is true as well.
			var isError = 
				(request.responseCode >= 400 && request.responseCode < 600)
				|| request.isNetworkError;
			
			if (simpleCallback != null) {
				simpleCallback(request.downloadHandler.text, request, isError);
			}

			if (jsonCallback != null) {
				Dictionary<string, object> responseData = ParseJson(request.downloadHandler.text);
				jsonCallback(responseData, request, isError);
			}
		}
	}
	
	/// <summary>
	/// Sends a request with a JSON body. You can configure to either use a JSON callback or a Simple callback that will
	/// expect a string response. You can also use more than one callback if necessary, although normally not needed.
	/// </summary>
	/// <param name="method">The HttpMethod to use for the request.</param>
	/// <param name="url">The url to use for the request</param>
	/// <param name="jsonDict">Dictionary containing the JSON data.</param>
	/// <param name="simpleCallback">Callback code to run after the request. Expects a simple string response.</param>
	/// <param name="jsonCallback">Callback code to run after the request finishes. This should expect a JSON response.</param>
	/// <param name="headers">Optional. Dictionary of HTTP headers to use for the request.</param>
	/// <returns></returns>
	public static IEnumerator sendJsonRequest(
		HttpMethod method,
		string url,
		Dictionary<string, object> jsonDict,
		SimpleRequestCallback simpleCallback = null,
		JsonRequestCallback jsonCallback = null,
		Dictionary<string, string> headers = null
	) {

		using (UnityWebRequest request = ConfigureRequestForJson(url, jsonDict, headers)) {
			request.method = method.ToString();
			
			yield return request.SendWebRequest(); // Send and wait for the request to return.

			// If the request returns a response code between 400-599 then there was an error
			// Or if the request's "isNetworkError" is true as well.
			var isError = 
				(request.responseCode >= 400 && request.responseCode < 600)
				|| request.isNetworkError;

			if (simpleCallback != null) {
				simpleCallback(request.downloadHandler.text, request, isError);
			}

			if (jsonCallback != null) {
				Dictionary<string, object> responseData = ParseJson(request.downloadHandler.text);
				jsonCallback(responseData, request, isError);
			}
		}
		
	}

	/// <summary>
	/// Sends a HTTP form data request using a POST method. This is mainly used for authentication since the
	/// authentication backend requires form data.
	/// </summary>
	/// <param name="url">The url to use for the request</param>
	/// <param name="formDict">Dictionary containing the form data.</param>
	/// <param name="simpleCallback">Callback code to run after the request. Expects a simple string response.</param>
	/// <param name="jsonCallback">Callback code to run after the request finishes. This should expect a JSON response.</param>
	/// <param name="headers">Optional. Dictionary of HTTP headers to use for the request.</param>
	/// <returns>IEnumerator object to use with StartCoroutine from a Monobehaviour object.</returns>
	public static IEnumerator sendFormRequest(
		string url,
		Dictionary<string, string> formDict,
		SimpleRequestCallback simpleCallback = null,
		JsonRequestCallback jsonCallback = null,
		Dictionary<string, string> headers = null) {
		
		using (UnityWebRequest request = ConfigureRequestWithFormData(url, formDict, headers)) {			
			yield return request.SendWebRequest();

			// If the request returns a response code between 400-599 then there was an error
			// Or if the request's "isNetworkError" is true as well.
			var isError = 
				(request.responseCode >= 400 && request.responseCode < 600)
				|| request.isNetworkError;

			if (simpleCallback != null) {
				simpleCallback(request.downloadHandler.text, request, isError);
			}

			if (jsonCallback != null) {
				Dictionary<string, object> responseData = ParseJson(request.downloadHandler.text);
				jsonCallback(responseData, request, isError);
			}
		}
	}

	#region Request Configuration
	/// <summary>
	/// Configures a new UnityWebRequest to send a GET request with optional parameters and headers
	/// </summary>
	/// <param name="url">The url string to use for the request</param>
	/// <param name="queryParams">Optional HTTP GET parameters to use for the request</param>
	/// <param name="headers">Optional HTTP header parameters to use for the request.</param>
	/// <returns>A new UnityWebRequest configured for a GET request</returns>
	private static UnityWebRequest ConfigureRequestWithUrlParams(
		string url, 
		Dictionary<string, string> queryParams = null,
		Dictionary<string, string> headers = null) {
		
		// If the last character is a slash then remove it for the request for better GET formatting
		if (url.EndsWith("/")) {
			url = url.Substring(0, url.Length - 1);
		}

		// If there are any query parameters then add them to the url before making the request.
		if (queryParams != null && queryParams.Count > 0) {
			var count = 0;
			url = url + "?"; // Add the query parameters start character. 
			
			foreach (var keyValue in queryParams) {
				if (count > 0) {
					// Append "new parameter" character to add the next parameter to url.
					url = url + "&";
				}

				// Url encode any special characters
				var key = System.Uri.EscapeDataString(keyValue.Key);
				var value = System.Uri.EscapeDataString(keyValue.Value);

				url = url + key + "=" + value;

				count++;
			}
		}
		
		var request = new UnityWebRequest(url);
		request.downloadHandler = new DownloadHandlerBuffer();

		request = AddHeaders(request, headers);
		
		return request;
	}

	/// <summary>
	/// Configures a new UnityWebRequest to send a JSON request.
	/// </summary>
	/// <param name="url">The url string to use for the request</param>
	/// <param name="formDict">A dictionary containing the JSON data.</param>
	/// <param name="headers">Optional dictionary of HTTP headers to use for the request</param>
	/// <returns>A new UnityWebRequest properly configured for sending JSON data</returns>
	private static UnityWebRequest ConfigureRequestForJson(
		string url, 
		Dictionary<string, object> formDict, 
		Dictionary<string, string> headers = null) {
		
		// If the url doesn't end in a / then add one so Django doesn't complain.
		if (!url.EndsWith("/")) {
			url = url + "/";
		}

		// We need to turn the JSON into bytes to make the UnityWebRequest use it for the post data.
		string jsonString = Json.Serialize(formDict);
		byte[] postData = Encoding.UTF8.GetBytes(jsonString);
		
		UnityWebRequest request = new UnityWebRequest(url);
		
		// Set a custom upload handler so we can actually use the json data
		request.uploadHandler = new UploadHandlerRaw(postData);
		request.uploadHandler.contentType = "application/json";

		request.downloadHandler = new DownloadHandlerBuffer();

		request = AddHeaders(request, headers);

		return request;
	}
	
	/// <summary>
	/// Configures a new UnityWebRequest to send HTTP form data.
	/// </summary>
	/// <param name="url">The url string to use for the request</param>
	/// <param name="formDict">A dictionary containing the JSON data.</param>
	/// <param name="headers">Optional dictionary of HTTP headers to use for the request</param>
	/// <returns>A new UnityWebRequest properly configured for sending JSON data</returns>
	private static UnityWebRequest ConfigureRequestWithFormData(
		string url, 
		Dictionary<string, string> formDict, 
		Dictionary<string, string> headers = null) {
		
		// If the url doesn't end in a / then add one so Django doesn't complain.
		if (!url.EndsWith("/")) {
			url = url + "/";
		}

		List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
		foreach (var keyValuePair in formDict) {
			formData.Add(
					new MultipartFormDataSection(keyValuePair.Key, keyValuePair.Value)
				);
		}
		
		UnityWebRequest request = UnityWebRequest.Post(url, formData);
		request.chunkedTransfer = false;

		request = AddHeaders(request, headers);

		return request;
	}

	/// <summary>
	/// Adds headers to the UnityWebRequest.
	/// </summary>
	/// <param name="request">The UnityWebRequest object to add headers to.</param>
	/// <param name="headers">The Dictionary object containing the headers.</param>
	/// <returns></returns>
	private static UnityWebRequest AddHeaders(UnityWebRequest request, Dictionary<string, string> headers = null) {
		// Set any other request headers
		if (headers != null) {
			foreach (KeyValuePair<string, string> header in headers) {
				request.SetRequestHeader(header.Key, header.Value);
			}
		}

		return request;
	}
	#endregion
	
	#region Helper Methods
	/// <summary>
	/// Tries to parse a string of JSON data.
	/// </summary>
	/// <param name="jsonString">A string containing the JSON data</param>
	/// <returns>
	/// A dictionary containing the JSON data. Null if the JSON string was null or if the string could not be parsed.
	/// </returns>
	/// <remarks>
	///	We are allowing null to be passed in as a parameters to save some code that checks for null within the
	/// network request methods.
	/// </remarks>
	private static Dictionary<string, object> ParseJson(string jsonString = null) {
		Dictionary<string, object> parsedJson = null;
		
		if (jsonString != null) {
			var jsonResponse = new JsonResponse(jsonString);
			parsedJson = jsonResponse.data;
		}

		return parsedJson;
	}
	#endregion
}
