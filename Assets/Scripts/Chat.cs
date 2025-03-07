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
                // �T�[�o�[�Ȃ璼�ڏ���
                AddChatMessageClientRpc(chatlog);
            }
            else
            {
                Debug.Log("Call as Client");
                // �N���C�A���g�Ȃ�T�[�o�[�ɑ��M
                SendChatMessageServerRpc(chatlog);
            }
        });
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        Debug.Log("As Client Message:" + message);
        AddChatMessageClientRpc(message);
    }

    //�N���C�A���g�����烁�b�Z�[�W�𑗂��Ă����Ȃ��ƕ������
    [ClientRpc]
    private void AddChatMessageClientRpc(string message)
    {
        Debug.Log("As Server Message:" + message);
        Image newChat = Instantiate(TextPrefab);
        newChat.name = message;
        newChat.GetComponentInChildren<TextMeshProUGUI>().text = message;
        newChat.transform.SetParent(panel.transform);
        newChat.transform.localPosition = new Vector3(0, 800 - i * 110, 0);
        newChat.transform.localScale = new Vector3(4, 1, 1);
        i++;
    }
}
