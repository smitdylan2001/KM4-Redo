using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedPlayer : MonoBehaviour
{
    public bool IsReady { get; private set; }
    public bool Name { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        IsReady = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetName(string name)
    {
        Name = Name;
    }
}
