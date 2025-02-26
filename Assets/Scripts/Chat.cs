using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class Chat : NetworkBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Button CloseTab;
    [SerializeField] private GameObject panel;
    [SerializeField] private Image TextPrefab;
    [SerializeField] private TMP_InputField chat;
    private GameObject Canvas;

    private List<string> chatMessages = new List<string>();
    private int i = 0;

    void Start()
    {
        panel.SetActive(false);
        Canvas = GameObject.Find("Canvas");
        button.onClick.AddListener(() => panel.SetActive(true));
        CloseTab.onClick.AddListener(() => panel.SetActive(false));

        chat.onEndEdit.AddListener(delegate (string chatlog)
        {
            if (IsServer)
            {
                // サーバーなら直接処理
                AddChatMessageClientRpc(chatlog);
            }
            else
            {
                // クライアントならサーバーに送信
                SendChatMessageServerRpc(chatlog);
            }
        });
    }

    [ServerRpc]
    private void SendChatMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        AddChatMessageClientRpc(message);
    }

    [ClientRpc]
    private void AddChatMessageClientRpc(string message)
    {
        Image newChat = Instantiate(TextPrefab);
        newChat.name = message;
        newChat.GetComponentInChildren<TextMeshProUGUI>().text = message;
        newChat.transform.SetParent(panel.transform);
        newChat.transform.localPosition = new Vector3(0, 800 - i * 110, 0);
        newChat.transform.localScale = new Vector3(4, 1, 1);
        i++;
    }
}
