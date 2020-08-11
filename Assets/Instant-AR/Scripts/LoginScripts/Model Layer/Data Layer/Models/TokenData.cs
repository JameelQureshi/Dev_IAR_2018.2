using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to store login credentials.
/// </summary>
[Serializable]
public class TokenData {

    public const string TOKEN_KEY = "access_token";
    public const string EXPIRES_KEY = "expires_in";
    public const string TOKEN_TYPE_KEY = "token_type";
    public const string SCOPE_KEY = "scope";
    public const string REFRESH_TOKEN_KEY = "refresh_token";
    
    public string token;
    public string refreshToken;
    public DateTimeOffset expirationDate;
    public string scope;
    public string tokenType;
    public string username;

    public TokenData(string token, 
        string refreshToken, 
        double expiresIn,
        string scope,
        string tokenType,
        string username) {
        
        this.token = token;
        this.refreshToken = refreshToken;
        this.expirationDate = DateTimeOffset.UtcNow.AddSeconds(expiresIn);
        this.scope = scope;
        this.tokenType = tokenType;
        this.username = username;
    }

    /// <summary>
    /// Creates a TokenData object from JSON data received from the server.
    /// </summary>
    /// <param name="jsonData"></param>
    public TokenData(Dictionary<string, object> jsonData, string username) {
        if (jsonData.ContainsKey(TOKEN_KEY)
            && jsonData.ContainsKey(EXPIRES_KEY)
            && jsonData.ContainsKey(TOKEN_TYPE_KEY)
            && jsonData.ContainsKey(SCOPE_KEY)
            && jsonData.ContainsKey(REFRESH_TOKEN_KEY)) {

            this.token = jsonData[TOKEN_KEY] as string;
            this.refreshToken = jsonData[REFRESH_TOKEN_KEY] as string;
            this.scope = jsonData[SCOPE_KEY] as string;
            this.tokenType = jsonData[TOKEN_TYPE_KEY] as string;

            long expiresIn = (long) jsonData[EXPIRES_KEY];
            this.expirationDate = DateTimeOffset.UtcNow.AddSeconds(expiresIn);

            this.username = username;

        }
        else {
            throw new Exception("Could not create TokenData from json data. Missing data.");
        }
    }

    /// <summary>
    /// Checks if this token is expired. This will check against one day before the actual
    /// expiration date to avoid overlap in expiration.
    /// </summary>
    /// <returns>True if the token is expired, false otherwise.</returns>
    public bool IsTokenExpired() {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        // "Expire" the token 1 day before it actually expires so there is no chance of expiration overlap
        expirationDate.AddDays(-1);

        int status = now.CompareTo(expirationDate);
        // A value less than 0 (i.e. -1) means that the "now" DateTimeOffset is less than the "expirationDate" DateTimeOffset.
        if (status < 0) {
            // Now is before the expiration date.
            return false;
        }

        return true;
    }
}
