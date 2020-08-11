using System;
using UnityEngine;

public class DeactivatePopUp : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false);
    }
}
