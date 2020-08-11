using System.Collections.Generic;
using System.Text.RegularExpressions;
using MiniJSON;

/// <summary>
/// A convenience class that will automatically parse a network JSON response into a Dictionary<string, object>
/// and put it into its "data" parameter
/// </summary>
public class JsonResponse {
    public Dictionary<string, object> data;

    public JsonResponse(string responseString) {
        string formattedResponse = responseString;

        /* We need to test if there is unecessary quotes and remove them if there are
         * 
         * NOTE: Having those quotes is actually the normal JSON specification and usually the system would
         * handle these for us but it seems that Unity doesn't do a good job at that.
         * This only occurs with web requests as the server will adhere to the JSON specification and add
         * These quotes on for us.
        */
        string test = responseString.Substring(0,1);
        if (test == "\"") {
            formattedResponse = responseString.Substring(1, responseString.Length - 2);
        }

        string jsonString = Regex.Unescape(formattedResponse);// Unescape the urlencoded string
        this.data = Json.Deserialize(jsonString) as Dictionary<string, object>;
    }
}