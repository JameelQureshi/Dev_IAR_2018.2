using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is a convenience class to automatically set necessary fields for any screens that subclass this.
///
/// Usually used to automatically set the "gameController" object that will handle switching of screens, handling specific
/// events, etc...
/// </summary>
public class AccountBaseScreen : MonoBehaviour {

    protected AccountController gameController;
    protected AuthManager authManager;
    
    public virtual void Awake() {
        SetGameController();
        authManager = AuthManager.Instance;
    }

    protected void SetGameController() {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        AccountController controllerComponent = gameControllerObject.GetComponent<AccountController>();

        if (controllerComponent != null) {
            gameController = controllerComponent;

            if (gameController == null) {
                throw new MissingComponentException("No GameController script on Game Controller object");
            } 

        } else {
            throw new MissingReferenceException("No Game Controller object in scene");
        }
    }
}
