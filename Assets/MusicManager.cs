using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager MMinstance;
    void Awake()
    {
        DontDestroyOnLoad(this);
        if (MMinstance == null) MMinstance = this;
        else Destroy(gameObject);
    }
}
