using Netcode.Transports;
using Steamworks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SteamLobby : MonoBehaviour
{
    //���r�[�쐬�R�[���o�b�N
    private CallResult<LobbyCreated_t> m_crLobbyCreated;
    //ロビー入室コールバック
    private Callback<LobbyEnter_t> m_lobbyEnter;

    //���r�[�f�[�^�ݒ�p�L�[
    private const string s_HostAddressKey = "HostAddress";

    public ulong LobbyID { get; private set; }

    private int playernum = 0;

    public void Start()
    {
        //SteamManager�̏��������������Ă�����
        if (SteamManager.Initialized)
        {
            m_crLobbyCreated = CallResult<LobbyCreated_t>.Create(OnCreateLobby);
            m_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }
    }

    void Update()
    {
        if (SteamManager.Initialized)
        {
            SteamAPI.RunCallbacks();
        }
    }

    /// <summary>
    /// ���r�[�쐬�i�Q�[�����z�X�g�j
    /// </summary>
    /// <param name="lobbyType"></param>
    /// <param name="cMaxMembers"></param>
    public void CreateLobby()
    {
        if (!SteamManager.Initialized)
        {
            Title.ChangeDebugLog("Steam is not initialized");
            Debug.LogError("Steam is not initialized.");
            return;
        }

        Title.ChangeDebugLog("Steam is initialized");

        SteamAPICall_t hCreateLobby = SteamMatchmaking.CreateLobby(
            ELobbyType.k_ELobbyTypePublic,//ロビーの公開・非公開 
            4);//ロビーの最大人数
        m_crLobbyCreated.Set(hCreateLobby);
    }

    //���r�[�쐬�����R�[���o�b�N
    private void OnCreateLobby(LobbyCreated_t pCallback, bool bIOFailure)
    {
        //ロビー作成成功していなかった場合
        if (pCallback.m_eResult != EResult.k_EResultOK || bIOFailure)
        {
            Title.ChangeDebugLog("Faild to Create Lobby");
            Debug.Log("ロビーが作成されていません");
            return;
        }

        Title.ChangeDebugLog("Succses to create Lobby");
        //ホストのアドレス（SteamID）を登録
        SteamMatchmaking.SetLobbyData(
            new CSteamID(pCallback.m_ulSteamIDLobby),
            s_HostAddressKey,
            SteamUser.GetSteamID().ToString());

        //コールバックの作成と関数の登録
        m_crLobbyCreated = CallResult<LobbyCreated_t>.Create(OnCreateLobby);

        //ロビーID保存(画面にロビーIDを表示させる際に使用するので変数に入れておく)
        LobbyID = pCallback.m_ulSteamIDLobby;

        //サーバー開始コールバック
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        //ホスト開始
        NetworkManager.Singleton.StartHost();
        //シーンを切り替え
        NetworkManager.Singleton.SceneManager.LoadScene("Host", LoadSceneMode.Single);
    }

    /// <summary>
    /// ���r�[���o
    /// </summary>
    public void JoinLobby(CSteamID lobbyID)
    {
        Title.ChangeDebugLog("Client Start");
        Debug.Log("JoinLobby");
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    /// <summary>
    /// �Q�[���̏��҂��󂯂����̃R�[���o�b�N
    /// </summary>
    /// <param name="callback"></param>
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Title.ChangeDebugLog("OnGameJoinLobbyRequested");
        Debug.Log("OnGameJoinLobbyRequested");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    /// <summary>
    /// ���r�[�����R�[���o�b�N
    /// </summary>
    /// <param name="callback"></param>
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        Title.ChangeDebugLog("OnLobbyEntered");
        Debug.Log("OnLobbyEntered");
        //入室失敗時
        if ((EChatRoomEnterResponse)callback.m_EChatRoomEnterResponse != EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
        {
            Title.ChangeDebugLog("Faild to Get in Lobby");
            Debug.Log("入室に失敗しました");
            return;
        }

        //ホストのSteamIDを取得
        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),//ロビーID
            s_HostAddressKey);//設定したキー

        if (string.IsNullOrEmpty(hostAddress))
        {
            Title.ChangeDebugLog("Can not Get Host's Address");
            Debug.LogError("ホストのアドレスを取得出来ませんでした。");
            return;
        }
        else
        {
            Debug.Log(hostAddress);
        }
        //ホスト（CreateLobbyした本人）もここを通るのでクライアント接続しないようにリターン
        //これじゃね?
        if (hostAddress == SteamUser.GetSteamID().ToString())
        {
            Title.ChangeDebugLog("fuckyou");
            Debug.Log("fuckyou");
            return;
        }
        Title.ChangeDebugLog("Sucsess to join Lobby");

        //ロビーID保存
        LobbyID = callback.m_ulSteamIDLobby;

        //Netcodeでクライアント接続
        var stp = (SteamNetworkingSocketsTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        stp.ConnectToSteamID = ulong.Parse(hostAddress);

        playernum++;

        //ホストに接続
        bool result = NetworkManager.Singleton.StartClient();
        Title.ChangeDebugLog("Is Sucsess to connect host"+result);
        Debug.Log("ホストに接続できたか:" + result);
        //切断時
        if (hostAddress != SteamUser.GetSteamID().ToString()) NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        Title.ChangeDebugLog($"Connect to SteamID{hostAddress}.");
        Debug.Log($"SteamID{hostAddress}の部屋に接続");
    }

    /// <summary>
    /// �ڑ����F
    /// </summary>
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("ApprovalCheck");
        // �ǉ��̏��F�菇���K�v�ȏꍇ�́A�ǉ��̎菇����������܂ł���� true �ɐݒ肵�܂�
        // true ���� false �ɑJ�ڂ���ƁA�ڑ����F��������������܂��B
        response.Pending = true;

        //�ő�
        if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
        {
            response.Approved = false;
            response.Pending = false;
            return;
        }

        //��������͐ڑ������N���C�A���g�Ɍ���������
        response.Approved = true;//�ڑ�������

        //PlayerObject�𐶐����邩�ǂ���
        response.CreatePlayerObject = false;
        //��������PlayerObject��Prefab�n�b�V���l�Bnull�̏ꍇNetworkManager�ɓo�^�����v���n�u���g�p�����
        response.PlayerPrefabHash = null;

        //PlayerObject���X�|�[������ʒu(null�̏ꍇVector3.zero)
        response.Position = Vector3.zero;
        //PlayerObject���X�|�[�����̉�] (null�̏ꍇQuaternion.identity)
        response.Rotation = Quaternion.identity;

        response.Pending = false;
    }

    /// <summary>
    /// �N���C�A���g���ؒf�����Ƃ�
    /// </summary>
    private void OnClientDisconnect(ulong clientId)
    {
        //�N���C�A���g�ؒf�R�[���o�b�N
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        //�l�b�g���[�N�}�l�[�W���[��j���i����ŐV����NetworkManager�����i�g���j���Ƃ��ł���j
        NetworkManager.Singleton.Shutdown();
        //���C���V�[���ɖ߂�
        SceneManager.LoadScene("Title");
    }

    //�ȈՓI�ȃV���O���g��
    private static SteamLobby instance;
    public static SteamLobby Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (SteamLobby)FindObjectOfType(typeof(SteamLobby));
            }

            return instance;
        }
    }
}