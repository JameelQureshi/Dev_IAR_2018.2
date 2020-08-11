using System.Collections;
using System.Linq;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This class should be considered the main interface for making network calls to the backend. This will automatically
/// handle authentication and return errors if authentication fails. The methods within this class will also try to
/// refresh any expired auth token; if the token cannot be refreshed then they will notify the caller if a login is
/// required.
///
/// If you are not making a request to the custom backend (i.e. to another API or network call) then please use the
/// methods found within RestfulApi.cs as those are the raw functions used for making RESTful calls.
/// </summary>
public class SecureRestfulApi {

    private static SecureRestfulApi _instance;
    private static string TAG = "NetworkManager: ";

    public static SecureRestfulApi Instance(AuthManager authManager) {
        if (_instance == null) {
            _instance = new SecureRestfulApi(authManager);
        }

        return _instance;
    }

    // Reference to the AuthManager for authenticating requests
    private AuthManager _authManager;
    
    private SecureRestfulApi(AuthManager authManager) {
        _authManager = authManager;
    }
    
    #region Secure RESTful API's

    /// <summary>
    /// Sends a simple network request using optional values. This is a wrapper for the RestfulApi.cs's version and will
    /// automatically authenticate the request. This will also refresh any auth token that is expired or will expire
    /// so there will be no need to re-login.
    /// </summary>
    /// <remarks>
    /// The "caller" is necessary to call any subsequent calls that are needed. Usually this is to re-authenticate the
    /// auth token. 
    /// </remarks>
    /// <param name="caller">Any Monobehaviour (game object) that will live for the duration of the call.</param>
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
    public void sendRequest(
        MonoBehaviour caller,
        HttpMethod method,
        string url,
        SimpleRequestCallback simpleCallback = null,
        JsonRequestCallback jsonCallback = null,
        Dictionary<string, string> queryParams = null,
        Dictionary<string, string> headers = null) {

        if (!_authManager.IsLoggedIn) {
            // User is not logged in. Call any callbacks and set their "loginNeeded" flag to true so the caller can
            // log the user out or handle the case in another way.
            HandleNotLoggedIn(simpleCallback, jsonCallback);
            return; // For conformance with return type.
        }
        
        // Refresh the auth token if needed.
        if (_authManager.CurrentToken.IsTokenExpired()) {
            // The token is expired. Refresh the token and pass the requested functionality into the completion block.
            SimpleRequestCallback refreshTokenCallback = delegate(string response, UnityWebRequest obj, bool error, bool needed) {
                if (!error && _authManager.IsLoggedIn) {
                    // Re-call this method with the refreshed token
                    sendRequest(caller, method, url, simpleCallback, jsonCallback, queryParams, headers);
                    return;
                }
                
                // For some reason the user is still not logged in. Return error case and notify caller that
                // the user needs to be logged in again.
                HandleNotLoggedIn(simpleCallback, jsonCallback);
            };

            caller.StartCoroutine(_authManager.RefreshToken(refreshTokenCallback));
            return;
        }

        Dictionary<string, string> allHeaders = AddAuthHeaders(headers);

        // Finally make the request using the vanilla RESTful API.
        caller.StartCoroutine(RestfulApi.sendRequest(method, url, simpleCallback, jsonCallback, queryParams, allHeaders));
    }

    /// <summary>
    /// Sends a request with a JSON body. You can configure to either use a JSON callback or a Simple callback that will
    /// expect a string response. You can also use more than one callback if necessary, although normally not needed.
    ///
    /// This is a wrapper for the RestfulApi.cs's version and will
    /// automatically authenticate the request. This will also refresh any auth token that is expired or will expire
    /// so there will be no need to re-login.
    /// </summary>
    /// <param name="caller">Any Monobehaviour (game object) that will live for the duration of the call.</param>
    /// <param name="method">The HttpMethod to use for the request.</param>
    /// <param name="url">The url to use for the request</param>
    /// <param name="jsonDict">Dictionary containing the JSON data.</param>
    /// <param name="simpleCallback">Callback code to run after the request. Expects a simple string response.</param>
    /// <param name="jsonCallback">Callback code to run after the request finishes. This should expect a JSON response.</param>
    /// <param name="headers">Optional. Dictionary of HTTP headers to use for the request.</param>
    /// <returns></returns>
    public void sendJsonRequest(
        MonoBehaviour caller,
        HttpMethod method,
        string url,
        Dictionary<string, object> jsonDict,
        SimpleRequestCallback simpleCallback = null,
        JsonRequestCallback jsonCallback = null,
        Dictionary<string, string> headers = null) {
        
        if (!_authManager.IsLoggedIn) {
            // User is not logged in. Call any callbacks and set their "loginNeeded" flag to true so the caller can
            // log the user out or handle the case in another way.
            HandleNotLoggedIn(simpleCallback, jsonCallback);
            return; // For conformance with return type.
        }
        
        // Refresh the auth token if needed.
        if (_authManager.CurrentToken.IsTokenExpired()) {
            // The token is expired. Refresh the token and pass the requested functionality into the completion block.
            SimpleRequestCallback refreshTokenCallback = delegate(string response, UnityWebRequest obj, bool error, bool needed) {
                if (!error && _authManager.IsLoggedIn) {
                    // Re-call this method with the refreshed token
                    sendJsonRequest(caller, method, url, jsonDict, simpleCallback, jsonCallback, headers);
                    return;
                }
                
                // For some reason the user is still not logged in. Return error case and notify caller that
                // the user needs to be logged in again.
                HandleNotLoggedIn(simpleCallback, jsonCallback);
            };

            caller.StartCoroutine(_authManager.RefreshToken(refreshTokenCallback));
            return;
        }

        Dictionary<string, string> allHeaders = AddAuthHeaders(headers);

        // Finally make the request using the vanilla RESTful API.
        caller.StartCoroutine(RestfulApi.sendJsonRequest(method, url, jsonDict, simpleCallback, jsonCallback, allHeaders));
    }

    /// <summary>
    /// Sends a HTTP form data request using a POST method. This is mainly used for authentication since the
    /// authentication backend requires form data.
    ///
    /// This is a wrapper for the RestfulApi.cs's version and will
    /// automatically authenticate the request. This will also refresh any auth token that is expired or will expire
    /// so there will be no need to re-login.
    /// </summary>
    /// <param name="caller">Any Monobehaviour (game object) that will live for the duration of the call.</param>
    /// <param name="url">The url to use for the request</param>
    /// <param name="formDict">Dictionary containing the form data.</param>
    /// <param name="simpleCallback">Callback code to run after the request. Expects a simple string response.</param>
    /// <param name="jsonCallback">Callback code to run after the request finishes. This should expect a JSON response.</param>
    /// <param name="headers">Optional. Dictionary of HTTP headers to use for the request.</param>
    /// <returns>IEnumerator object to use with StartCoroutine from a Monobehaviour object.</returns>
    public void sendFormRequest(
        MonoBehaviour caller,
        string url,
        Dictionary<string, string> formDict,
        SimpleRequestCallback simpleCallback = null,
        JsonRequestCallback jsonCallback = null,
        Dictionary<string, string> headers = null) {
        
        if (!_authManager.IsLoggedIn) {
            // User is not logged in. Call any callbacks and set their "loginNeeded" flag to true so the caller can
            // log the user out or handle the case in another way.
            HandleNotLoggedIn(simpleCallback, jsonCallback);
            return; // For conformance with return type.
        }
        
        // Refresh the auth token if needed.
        if (_authManager.CurrentToken.IsTokenExpired()) {
            // The token is expired. Refresh the token and pass the requested functionality into the completion block.
            SimpleRequestCallback refreshTokenCallback = delegate(string response, UnityWebRequest obj, bool error, bool needed) {
                if (!error && _authManager.IsLoggedIn) {
                    // Re-call this method with the refreshed token
                    sendFormRequest(caller, url, formDict, simpleCallback, jsonCallback, headers);
                    return;
                }
                
                // For some reason the user is still not logged in. Return error case and notify caller that
                // the user needs to be logged in again.
                HandleNotLoggedIn(simpleCallback, jsonCallback);
            };

            caller.StartCoroutine(_authManager.RefreshToken(refreshTokenCallback));
            return;
        }

        Dictionary<string, string> allHeaders = AddAuthHeaders(headers);

        // Finally make the request using the vanilla RESTful API.
        caller.StartCoroutine(RestfulApi.sendFormRequest(url, formDict, simpleCallback, jsonCallback, allHeaders));
    }

    #endregion
    
    #region Utility Methods
    /// <summary>
    /// Prints log error and calls any callbacks to notify them that a login is required.
    /// </summary>
    /// <param name="simpleCallback"></param>
    /// <param name="jsonCallback"></param>
    private void HandleNotLoggedIn(SimpleRequestCallback simpleCallback = null, JsonRequestCallback jsonCallback = null) {
        Debug.Log(TAG + "User is not logged in. Cannot send authenticated request");
        var message = "User is not logged in";
        
        if (simpleCallback != null) {
            simpleCallback(message, null, true, loginNeeded: true);
        }

        if (jsonCallback != null) {
            var dict = new Dictionary<string, object>();
            dict.Add("error", message);
            jsonCallback(dict, null, true, loginNeeded: true);
        }
    }

    /// <summary>
    /// Takes any current headers (if any exist) and adds an authorization header to them. This will create the headers
    /// dictionary if none are passed in.
    /// </summary>
    /// <param name="currentHeaders">Nullable, the current list of headers, if any.</param>
    /// <returns>A dictionary of HTTP headers with the new Authorization headers added.</returns>
    private Dictionary<string, string> AddAuthHeaders(Dictionary<string, string> currentHeaders = null) {
        var allHeaders = new Dictionary<string, string>();
        allHeaders.Add("Authorization", "Bearer " + _authManager.CurrentToken.token);
        
        // Add current headers to full header dictionary
        if (currentHeaders != null) {
            foreach (var keyValuePair in currentHeaders) {
                allHeaders.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        return allHeaders;
    }
    #endregion
}