using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class PlayerNum : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clientCountText;

    private void Start()
    {
        clientCountText.text = "0";
        if (NetworkManager.Singleton == null) return;

        // クライアント接続時のコールバック
        NetworkManager.Singleton.OnClientConnectedCallback += UpdateClientCount;
        // クライアント切断時のコールバック
        NetworkManager.Singleton.OnClientDisconnectCallback += UpdateClientCount;

        // 初期表示
        UpdateClientCount(0);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;

        // イベント登録解除
        NetworkManager.Singleton.OnClientConnectedCallback -= UpdateClientCount;
        NetworkManager.Singleton.OnClientDisconnectCallback -= UpdateClientCount;
    }

    private void UpdateClientCount(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        // ホストを含めた現在の接続クライアント数を取得
        int clientCount = NetworkManager.Singleton.ConnectedClients.Count;
        clientCountText.text = $"Client Number: {clientCount}";
    }
}