using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkConnection_UI : MonoBehaviour
{
    [SerializeField] private Button _StartHostButton;
    [SerializeField] private Button _StartClientButton;
    [SerializeField] private Button _PlayClientButton;
    [SerializeField] private TMP_InputField _Adress;

    private void Start()
    {
        _StartHostButton.onClick.AddListener(() =>{
            NetworkManager.Singleton.StartHost();
            _StartHostButton.interactable = false;
            _StartClientButton.interactable = false;
            _PlayClientButton.interactable = true;
        });
        _StartClientButton.onClick.AddListener(() =>{
           // NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData. = _Adress.text;
            NetworkManager.Singleton.StartClient();
            _StartHostButton.interactable = false;
            _StartClientButton.interactable = false;
        });
        _PlayClientButton.onClick.AddListener(() =>
        {
            MonoBehaviourExtension.Instance.OnSessionStarted();
            HideServerRpc();
            //Instantiate Tiles
            //TileSystem.Instance.
        });

    }

    private void Update()
    {
        if (NetworkManager.Singleton.IsListening && NetworkManager.Singleton.IsServer)
        {
            _PlayClientButton.gameObject.SetActive(true);
        }
    }

    [ServerRpc]
    private void HideServerRpc()
    {
        gameObject.SetActive(false);    
    }
}
