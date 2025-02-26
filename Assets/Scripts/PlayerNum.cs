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

        // �N���C�A���g�ڑ����̃R�[���o�b�N
        NetworkManager.Singleton.OnClientConnectedCallback += UpdateClientCount;
        // �N���C�A���g�ؒf���̃R�[���o�b�N
        NetworkManager.Singleton.OnClientDisconnectCallback += UpdateClientCount;

        // �����\��
        UpdateClientCount(0);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;

        // �C�x���g�o�^����
        NetworkManager.Singleton.OnClientConnectedCallback -= UpdateClientCount;
        NetworkManager.Singleton.OnClientDisconnectCallback -= UpdateClientCount;
    }

    private void UpdateClientCount(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        // �z�X�g���܂߂����݂̐ڑ��N���C�A���g�����擾
        int clientCount = NetworkManager.Singleton.ConnectedClients.Count;
        clientCountText.text = $"Client Number: {clientCount}";
    }
}