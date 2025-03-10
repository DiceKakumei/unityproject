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
    [SerializeField] private GameObject Camera;
    private Image[] ChatArray = new Image[1000];//配列を定義
    private GameObject Canvas;
    private float wh = 0;
    private float SumWh = 0;

    private List<string> chatMessages = new List<string>();
    private int i = 0;

    void Start()
    {
        panel.SetActive(false);
        Canvas = GameObject.Find("Canvas");
        button.onClick.AddListener(() => {
            Camera.GetComponent<SceneViewCamera>().CanMoveCam = false;
            panel.SetActive(true);
            });
        CloseTab.onClick.AddListener(() =>
        {
            Camera.GetComponent<SceneViewCamera>().CanMoveCam = true;
            panel.SetActive(false);
        });

        chat.onEndEdit.AddListener(delegate (string chatlog)
        {
            if (IsServer)
            {
                // サーバーなら直接処理
                AddChatMessageClientRpc(chatlog);
            }
            else
            {
                Debug.Log("Call as Client");
                // クライアントならサーバーに送信
                SendChatMessageServerRpc(chatlog);
            }
        });   
    }

    void Update()
    {
        // 中クリック（ホイールクリック）でスクロール
        wh = Input.GetAxis("Mouse ScrollWheel");
        SumWh += wh*1000;
        //Debug.Log("wh:" + wh);
        if (wh != 0)
        {
            for (int j = 0; j < i; j++)
            {
                ChatArray[j].transform.localPosition += new Vector3(0, wh*1000, 0);
                Vector3 pos = ChatArray[j].transform.localPosition;
                if (pos.y > 350 || pos.y < -310)
                {
                    ChatArray[j].gameObject.SetActive(false);
                }
                else
                {
                    ChatArray[j].gameObject.SetActive(true);
                }
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        Debug.Log("As Client Message:" + message);
        AddChatMessageClientRpc(message);
    }

    //クライアント側からメッセージを送ってもらわないと分からん
    [ClientRpc]
    private void AddChatMessageClientRpc(string message)
    {
        Debug.Log("As Server Message:" + message);
        Image newChat = Instantiate(TextPrefab);
        newChat.name = message;
        newChat.GetComponentInChildren<TextMeshProUGUI>().text = message;
        newChat.transform.SetParent(panel.transform);
        newChat.transform.localPosition = new Vector3(0, 350 - i * 110 + SumWh, 0);
        newChat.transform.localScale = new Vector3(4, 1, 1);
        ChatArray[i] = newChat;
        Vector3 pos = ChatArray[i].transform.localPosition;
        if (pos.y > 350 || pos.y < -310)
        {
            ChatArray[i].gameObject.SetActive(false);
        }
        i++;
        if (i > 1000) i = 0;
    }
}
