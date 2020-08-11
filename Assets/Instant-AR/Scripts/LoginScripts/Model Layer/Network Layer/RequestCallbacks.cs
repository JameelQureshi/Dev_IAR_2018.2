using System.Collections.Generic;
using UnityEngine.Networking;

/**
 * This file is used to store any callback delegates used for network requests. These are essentially "blueprints"
 * for functions used to handle the responses for the network calls.
 */

/// <summary>
/// Delegate for returning data from each request. Assumes that the response returns a JSON string.
/// </summary>
/// <remarks>
/// JsonRequestCallback is the main callback used to retrieve JSON data from the server. The JSON will automatically be
/// parsed and populated within the "jsonData" variable.
/// </remarks>
/// <param name="jsonData">Dictionary containing the data that will be serialized into a JSON string</param>
/// <param name="requestObj">The UnityWebRequest object containing all information about the request/response</param>
/// <param name="isError">Whether there was an error with the request</param>
public delegate void JsonRequestCallback(Dictionary<string, object> jsonData, UnityWebRequest requestObj, bool isError, bool loginNeeded = false);

/// <summary>
/// Delegate for returning simple string data for a network request.
/// </summary>
/// <remarks>
/// SimpleRequestCallback is used when the server returns a simple message. These are usually used for when you
/// really don't care about the response and just want to pass info to the server with completion code within
/// the callback.
/// </remarks>
/// <param name="response">The response string returned from the server.</param>
/// <param name="requestObj">The UnityWebRequest object containing all information about the request/response</param>
/// <param name="isError">Whether there was an error with the request</param>
public delegate void SimpleRequestCallback(string response, UnityWebRequest requestObj, bool isError, bool loginNeeded = false);

/// <summary>
/// An empty callback to pass generic functionality around with.
/// </summary>
public delegate void EmptyCallback();