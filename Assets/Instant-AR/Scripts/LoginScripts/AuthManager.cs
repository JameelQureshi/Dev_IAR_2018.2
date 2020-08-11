using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using MiniJSON;
//using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This singleton class is used to access all auth methods such as Login, Logout, etc...
///
/// Use this within the game as the main interface for the following:
/// - Registering a user
/// - Logging in a user
/// - Logging out a user
/// - Submitting Forgot password request
/// - Checking if a user is currently logged in.
/// </summary>
public class AuthManager {

    private static AuthManager _instance;
    
    /// <summary>
    /// Instance property to conform to singleton pattern.
    /// </summary>
    public static AuthManager Instance {
        get {
            if (_instance == null) {
                _instance = new AuthManager();
            }

            return _instance;
        }
    }

    private TokenRepository _tokenRepository;

    /// <summary>
    /// The token data
    /// </summary>
    public TokenData CurrentToken {
        get {
            return _tokenRepository.GetCurrentToken();
        }
        set {
            _tokenRepository.SaveToken(value);
        }
    }
    

    /// <summary>
    /// Semantically convenient property that returns whether the user is logged in or not.
    /// </summary>
    public bool IsLoggedIn {
        get { return CurrentToken != null; }
    }

    private AuthManager() {
        _tokenRepository = new TokenRepository();
    }

    #region Account Methods
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="email">Email to sign up with</param>
    /// <param name="password">Password to use for signup.</param>
    /// <param name="confirmPassword">Password confirmation</param>
    /// <param name="callback">Callback to process response from server.</param>
    /// <returns>IEnumerator to be used within a StartCoroutine from a Monobehaviour object.</returns>
    public IEnumerator Register(
        string email, 
        string password, 
        string confirmPassword, 
        SimpleRequestCallback callback = null
        ) {

        var url = ConfigurationManager.REGISTER_URL;
        
        // Configure the JSON payload
        var formDict = new Dictionary<string, string>();
        formDict.Add("email", email);
        formDict.Add("password", password);
        formDict.Add("confirm_password", confirmPassword);

        var basicAuthString = "Basic " + ConfigurationManager.BASIC_AUTH_CREDENTIALS;

        // Add basic authentication headers to the request
        var headers = new Dictionary<string, string>();
        headers.Add("Authorization", basicAuthString);
        
        JsonRequestCallback jsonCallback = delegate(Dictionary<string, object> data, UnityWebRequest obj, bool error, bool loginNeeded) {

            if (error) {
                string message = "Error registering user.";
                
                if (obj.downloadHandler.text != null && callback != null) {
                    var json = Json.Deserialize(obj.downloadHandler.text) as Dictionary<string, object>;
                    if (json != null) {
                        // Print the first error for brevity's sake
                        var errors = json["errors"] as List<object>;
                        message = errors[0] as string;
                    }
                    else {
                        message = obj.downloadHandler.text;
                    }
                }

                if (callback != null) {
                    callback(message, obj, error);    
                }
                return;
            }

            if (callback != null) {
                string message = "Success. An activation email has been sent to " + email;
                callback(message, obj, error);
            }
        };

        return RestfulApi.sendFormRequest(url, formDict, jsonCallback: jsonCallback, headers: headers);
    }

    /// <summary>
    /// Attempts to log in a user.
    /// </summary>
    /// <param name="email">The email to log in with.</param>
    /// <param name="password">The password to use to login with.</param>
    /// <param name="callback">Code to call after a response from the server.</param>
    /// <returns></returns>
    public IEnumerator Login(string email, string password, SimpleRequestCallback callback = null) {

        var url = ConfigurationManager.LOGIN_URL;

        var formData = new Dictionary<string, string>();
        formData.Add("grant_type", "password");
        formData.Add("client_id", ConfigurationManager.OAUTH_CLIENT_ID);
        formData.Add("username", email);
        formData.Add("password", password);
        
        // Parse the response to save the new token data. Afterwards pass all data back to the caller.
        JsonRequestCallback tokenCallback = delegate(Dictionary<string, object> data, UnityWebRequest obj, bool error, bool loginNeeded) {

            TokenData tokenData = ParseToken(data, email);
            string message = "Success";
            
            if (error) {
                // Error came from the server
                if (data.ContainsKey("error_description")) {
                    message = data["error_description"] as string;
                }
                else {
                    message = obj.downloadHandler.text;
                }
                
            }
            else if (tokenData == null) {
                // Error while parsing
                error = true;
                message = "Failed to log in.";
            }
            else {
                // Success
                CurrentToken = tokenData;
            }

            if (callback != null) {
                callback(message, obj, error);    
            }
        };

        return RestfulApi.sendFormRequest(url, formData, jsonCallback: tokenCallback);
    }

    /// <summary>
    /// Logs out the current user by removing 
    /// </summary>
    /// <param name="callback">Code to run after response from the server.</param>
    /// <returns>IEnumerator to use with StartCoroutine from a MonoBehaviour object.</returns>
    public IEnumerator Logout(SimpleRequestCallback callback = null) {

        // If null then the network request will return the error that we aren't handling here.
        var currentToken = _tokenRepository.GetCurrentToken();
        _tokenRepository.DeleteTokenData();

        var url = ConfigurationManager.LOGOUT_URL;
        
        var formDict = new Dictionary<string, string>();
        formDict.Add("client_id", ConfigurationManager.OAUTH_CLIENT_ID);
        formDict.Add("token", currentToken.token);

        return RestfulApi.sendFormRequest(url, formDict, callback);
    }

    /// <summary>
    /// Sends a request to change a users password to the server. The server will then send an activation email
    /// to the email provided.
    /// </summary>
    /// <param name="email">Email to recover password for.</param>
    /// <param name="callback">Code to run after server response.</param>
    /// <returns></returns>
    public IEnumerator ForgotPassword(string email, SimpleRequestCallback callback) {

        var url = ConfigurationManager.RECOVERY_URL;

        var formDict = new Dictionary<string, string>();
        formDict.Add("email", email);
        
        var basicAuthString = "Basic " + ConfigurationManager.BASIC_AUTH_CREDENTIALS;

        // Add basic authentication headers to the request
        var headers = new Dictionary<string, string>();
        headers.Add("Authorization", basicAuthString);

        return RestfulApi.sendFormRequest(url, formDict, callback, headers: headers);
    }

    /// <summary>
    /// Refreshes the current token using a refresh token. If that does not work, or if the refresh token is not valid
    /// then this will return an error within the callback. It is up to the caller to handle those situations.
    /// </summary>
    /// <param name="callback">Code to run after the request has finished.</param>
    /// <returns>IEnumerator to be run with StartCoroutine from a MonoBehaviour object.</returns>
    public IEnumerator RefreshToken(SimpleRequestCallback callback) {
        var url = ConfigurationManager.LOGIN_URL;

        var token = CurrentToken;
        string username = token.username;

        var formDict = new Dictionary<string, string>();
        formDict.Add("grant_type", "refresh_token");
        formDict.Add("client_id", ConfigurationManager.OAUTH_CLIENT_ID);
        formDict.Add("refresh_token", token.refreshToken);
        
        // Wrap the original callback in another one that will parse and save the token data.
        JsonRequestCallback tokenCallback = delegate(Dictionary<string, object> data, UnityWebRequest obj, bool error, bool loginNeeded) {

            TokenData tokenData = ParseToken(data, username);

            if (tokenData == null) {
                callback("Failed", obj, true);
                return;
            }

            // Save the token data as the new token.
            CurrentToken = tokenData;

            callback("Success", obj, error);
        };
        

        return RestfulApi.sendFormRequest(url, formDict, jsonCallback: tokenCallback);
    }
    #endregion
    
    /// <summary>
    /// Tries to create a TokenData object from a dictionary containing json values from the server.
    /// </summary>
    /// <param name="data">A Dictionary object containing parsed JSON data.</param>
    /// <returns>TokenData or null if an error occured.</returns>
    private TokenData ParseToken(Dictionary<string, object> data, string username) {
        try {
            TokenData tokenData = new TokenData(data, username);
            return tokenData;
        }
        catch (Exception e) {
            Debug.Log("AuthManager: Error parsing token: " + e);
            return null;
        }
    }
}
