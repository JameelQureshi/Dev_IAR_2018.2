using UnityEngine;
using System.Collections.Generic;
using Vuforia;

public class DontDestroy : MonoBehaviour
{

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
       // Destroy(this.gameObject);


    }
}