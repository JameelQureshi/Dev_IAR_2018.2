//#define LOCAL_ENV // Comment this line out to use production/live server constants/variables

using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

/// <summary>
/// This class is used to house any global game configuration. While the contents of this class could be broken up
/// and placed anywhere, it is a good idea to keep them in one location for a clearer architecture.
/// </summary>
public class ConfigurationManager {
	#region Networking/API
	
#if LOCAL_ENV
	public static string BASE_URL = "http://localhost:8080/"; // Only used for local development.
#else
	public static string BASE_URL = "https://app.instant-ar.com/"; // Live server.
#endif

// Auth Urls
	public static string AUTH_URL = BASE_URL + "auth/"; // Base Auth url used to construct other auth urls
	public static string LOGIN_URL = AUTH_URL + "o/token"; // Login inolves fetching auth token. This also includes refreshing token.
	public static string LOGOUT_URL = AUTH_URL + "o/revoke_token"; // Logout involes invalidating current token
	public static string REGISTER_URL = AUTH_URL + "register_app"; 
	public static string RECOVERY_URL = AUTH_URL + "forgot_password";

	// Use the following 3 endpoints when you want to test using the SecureRestfulApi's
	public static string RESTFUL_TEST_URL = BASE_URL + "rest";
	public static string RESTFUL_JSON_TEST_URL = BASE_URL + "json_rest";
	public static string RESTFUL_ERROR_TEST_URL = BASE_URL + "error_rest";
	
	// Use this url to test endpoints with the non-secure RestfulApi
	public static string RESTFUL_TEST_URL_INSECURE = BASE_URL + "insecure_rest_test";

	#endregion
	
	#region Credentials and Identifiers
	// Both of these are set to the live server.
	public static string OAUTH_CLIENT_ID = "yEbRZ5qJEtRkrMN7lplsBcFU0jY4DCctKVsj8a3d"; //https://app.instant-ar.com/admin/oauth2_provider/application/1/change/
    public static string BASIC_AUTH_CREDENTIALS = "YmFzaWM6Wz5iTk42NGBeNlltJj5G";
	#endregion

}
