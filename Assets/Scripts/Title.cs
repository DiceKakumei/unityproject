using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;
using Steamworks;

public class Title : MonoBehaviour
{
    [SerializeField] TMP_InputField m_joinLobbyID;
    public void StartHost()
    {
        //�ڑ����F�R�[���o�b�N
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        //���r�[�쐬
        SteamLobby.Instance.CreateLobby();
        //�t���X�N���[������
        Screen.fullScreen = false;
    }

    public void StartClient()
    {
        //���r�[����
        SteamLobby.Instance.JoinLobby((CSteamID)ulong.Parse(m_joinLobbyID.text));
        Debug.Log(m_joinLobbyID.text);
        //�t���X�N���[������
        Screen.fullScreen = false;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // �ǉ��̏��F�菇���K�v�ȏꍇ�́A�ǉ��̎菇����������܂ł���� true �ɐݒ肵�܂�
        // true ���� false �ɑJ�ڂ���ƁA�ڑ����F��������������܂��B
        response.Pending = true;

        //�ő�l�����`�F�b�N(���̏ꍇ��4�l�܂�)
        if (NetworkManager.Singleton.ConnectedClients.Count >= 4)
        {
            response.Approved = false;//�ڑ��������Ȃ�
            response.Pending = false;
            return;
        }

        //��������͐ڑ������N���C�A���g�Ɍ���������
        response.Approved = true;//�ڑ�������

        //PlayerObject�𐶐����邩�ǂ���
        response.CreatePlayerObject = false;

        //��������Prefab�n�b�V���l�Bnull�̏ꍇNetworkManager�ɓo�^�����v���n�u���g�p�����
        response.PlayerPrefabHash = null;

        //PlayerObject���X�|�[������ʒu(null�̏ꍇVector3.zero)
        var position = new Vector3(0, 1, -8);
        position.x = -5 + 5 * (NetworkManager.Singleton.ConnectedClients.Count % 3);
        response.Position = Vector3.zero;

        //PlayerObject���X�|�[�����̉�] (null�̏ꍇQuaternion.identity)
        response.Rotation = Quaternion.identity;

        response.Pending = false;
    }

}
