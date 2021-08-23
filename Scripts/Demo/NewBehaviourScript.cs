using Netwrok;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Connect("Main", "127.0.0.1", 8999);
    }

    // Update is called once per frame
    void Update()
    {
        NetworkManager.Update();
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Send", GUILayout.Width(100), GUILayout.Height(100)))
        {
            ChatProxy.Instance.SendInt(100);
        }
    }
}
