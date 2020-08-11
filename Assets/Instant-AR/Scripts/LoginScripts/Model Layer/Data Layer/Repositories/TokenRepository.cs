using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TokenRepository {

    private static string tokenFile = FileManager.TOKEN_FILE;
    
    /// <summary>
    /// Fetches the current TokenData from disk if it exists.
    /// </summary>
    /// <returns>The saved TokenData or null if none exists</returns>
    public TokenData GetCurrentToken() {
        if (FileManager.FileExists(tokenFile)) {
            return FileManager.LoadData<TokenData>(tokenFile);
        }
        
        return null;
    }

    /// <summary>
    /// Saves TokenData object to disk. This will overwrite any previously saved TokenData.
    /// </summary>
    /// <param name="token">Token data to save to disk.</param>
    public void SaveToken(TokenData token) {
        FileManager.SaveData(token, tokenFile);
    }

    /// <summary>
    /// Deletes the token data from disk.
    /// </summary>
    public void DeleteTokenData() {
        FileManager.DeleteData(tokenFile);
    }

}
