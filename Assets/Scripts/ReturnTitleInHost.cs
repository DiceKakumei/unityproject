using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;
using Steamworks;

public class ReturnTitleInHost : MonoBehaviour
{
    //SteamLobby steamlobby;
    private ulong ID;
    GameObject LobbyId;
    LobbyID lobbyid;

    [SerializeField] Button gotitle;
    // Start is called before the first frame update
    void Start()
    {
        gotitle.onClick.AddListener(delegate {
            LobbyId = GameObject.Find("LobbyID");
            lobbyid = LobbyId.GetComponent<LobbyID>();
            ulong.TryParse((lobbyid.m_lobbyIdText.text), out ID);
            LeaveLobby(ID);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
    void ChangeToTitle()
    {
        
        Debug.Log("GoTitle");
    }

    //ロビーから退出する
    public void LeaveLobby(ulong LobbyID)
    {
        Debug.Log("LobbyID:" + LobbyID);
        if (LobbyID != 0)
        {
            // Steamロビーから退出
            SteamMatchmaking.LeaveLobby(new CSteamID(LobbyID));
            Debug.Log("ロビーから退出しました");
        }

        // Netcodeの切断処理
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Netcodeの接続をシャットダウンしました");
        }

        // シーンをタイトルに戻す
        SceneManager.LoadScene("Title");
    }
}