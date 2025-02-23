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
        //接続承認コールバック
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        //ロビー作成
        SteamLobby.Instance.CreateLobby();
        //フルスクリーン解除
        Screen.fullScreen = false;
    }

    public void StartClient()
    {
        //ロビー入室
        SteamLobby.Instance.JoinLobby((CSteamID)ulong.Parse(m_joinLobbyID.text));
        Debug.Log(m_joinLobbyID.text);
        //フルスクリーン解除
        Screen.fullScreen = false;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // 追加の承認手順が必要な場合は、追加の手順が完了するまでこれを true に設定します
        // true から false に遷移すると、接続承認応答が処理されます。
        response.Pending = true;

        //最大人数をチェック(この場合は4人まで)
        if (NetworkManager.Singleton.ConnectedClients.Count >= 4)
        {
            response.Approved = false;//接続を許可しない
            response.Pending = false;
            return;
        }

        //ここからは接続成功クライアントに向けた処理
        response.Approved = true;//接続を許可

        //PlayerObjectを生成するかどうか
        response.CreatePlayerObject = false;

        //生成するPrefabハッシュ値。nullの場合NetworkManagerに登録したプレハブが使用される
        response.PlayerPrefabHash = null;

        //PlayerObjectをスポーンする位置(nullの場合Vector3.zero)
        var position = new Vector3(0, 1, -8);
        position.x = -5 + 5 * (NetworkManager.Singleton.ConnectedClients.Count % 3);
        response.Position = Vector3.zero;

        //PlayerObjectをスポーン時の回転 (nullの場合Quaternion.identity)
        response.Rotation = Quaternion.identity;

        response.Pending = false;
    }

}
