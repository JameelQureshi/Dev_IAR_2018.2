using System;
using UnityEngine;

public class BringToFront : MonoBehaviour
{

    void OnEnable()
    {
        transform.SetAsLastSibling();
    }
}
