using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkConnection_UI : MonoBehaviour
{
    [SerializeField] private Button _StartHostButton;
    [SerializeField] private Button _StartClientButton;

    private void Start()
    {
        _StartHostButton.onClick.AddListener(() =>{
            print("HOST");
            NetworkManager.Singleton.StartHost();
            Hide();
        });
        _StartClientButton.onClick.AddListener(() =>{
            print("CLIENT");
            NetworkManager.Singleton.StartClient();
            Hide();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);    
    }
}
