using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using SFB;
using Unity.Netcode;

public class NetworkBGM : NetworkBehaviour
{
    [SerializeField] Button OpenUI;
    [SerializeField] Button AddBGM;
    [SerializeField] GameObject AddBGMPanel;
    [SerializeField] GameObject ButtonPrefabPanel;
    [SerializeField] Button musicItemPrefab;
    [SerializeField] Button CloseTab;
    [SerializeField] GameObject Camera;

    private AudioSource audioSource;
    private Dictionary<ulong, List<byte>> receivedChunks = new Dictionary<ulong, List<byte>>();

    private List<string> bgmPaths = new List<string>();//�ǂݍ���BGM�̃p�X���X�g
    private int i = 0;
    private GameObject Canvas;
    private string[] PATH;
    const int CHUNK_SIZE = 64000; // 64KB�iNetcode�̐������j

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Canvas = GameObject.Find("Canvas");
        AddBGMPanel.SetActive(false);
        ButtonPrefabPanel.SetActive(false);
        //sicItemPrefab.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        OpenUI.onClick.AddListener(() => {
            Camera.GetComponent<SceneViewCamera>().CanMoveCam = true;
            AddBGMPanel.SetActive(true);
            });
        AddBGM.onClick.AddListener(LoadMusic);
        CloseTab.onClick.AddListener(() => {
            Camera.GetComponent<SceneViewCamera>().CanMoveCam = false;
            AddBGMPanel.SetActive(false);
            });
    }

    public void LoadMusic()
    {
        //PATH = UnityEditor.EditorUtility.OpenFilePanel("Select Music File", "", "mp3,wav,ogg");
        var path = (StandaloneFileBrowser.OpenFilePanel("Select Music File", "", new[] { new ExtensionFilter("Audio Files", "mp3", "wav", "ogg") }, false));
        if (path.Length > 0 && !string.IsNullOrEmpty(path[0]))
        {
            //bgmPaths[i] = path[0];
            Debug.Log("FilePath = " + path[0]);
            CreateMusicListItem(path[0], i);
            i++;
        }
    }

    void CreateMusicListItem(string filePath, int i)
    {
        string var = Path.GetFileName(filePath);
        var = var.Substring(0, 6);
        //�Ȃ�₩��₵�ăL�����o�X��ɃC���X�^���X�����������悤�ɂ���
        Button newItem = Instantiate(musicItemPrefab);
        newItem.GetComponentInChildren<TextMeshProUGUI>().text = var;
        newItem.GetComponent<Button>().onClick.AddListener(() => SendMusicFile(filePath));
        newItem.transform.parent = Canvas.transform;
        newItem.transform.parent = AddBGMPanel.transform;
        newItem.transform.position = new Vector3(530, 740 - i * 100, 0);
        newItem.transform.localScale = new Vector3(2, 2, 1);
    }

    public void SendMusicFile(string path)
    {
        if (!IsHost) return; // �z�X�g�̂ݑ��M�\

        Debug.Log("Accessed to SendMusicFile. path:"+path);

        byte[] fileData = File.ReadAllBytes(path);
        int totalChunks = Mathf.CeilToInt((float)fileData.Length / CHUNK_SIZE);

        // �ŏ��̃`�����N���M���Ƀt�@�C���T�C�Y������
        for (int i = 0; i < totalChunks; i++)
        {
            int chunkSize = Mathf.Min(CHUNK_SIZE, fileData.Length - i * CHUNK_SIZE);
            byte[] chunk = new byte[chunkSize];
            System.Array.Copy(fileData, i * CHUNK_SIZE, chunk, 0, chunkSize);

            SendMusicChunkClientRpc(chunk, i, totalChunks, fileData.Length);
        }
    }

    [ClientRpc]
    private void SendMusicChunkClientRpc(byte[] chunk, int chunkIndex, int totalChunks, int originalFileSize, ClientRpcParams clientRpcParams = default)
    {
        ulong senderId = NetworkManager.ServerClientId; // ���M�҂�ID

        if (!receivedChunks.ContainsKey(senderId))
        {
            receivedChunks[senderId] = new List<byte>();
        }

        receivedChunks[senderId].AddRange(chunk);

        // **�C���_: ��M�f�[�^�̃T�C�Y�����̃t�@�C���T�C�Y�ƈ�v������Đ�**
        if (receivedChunks[senderId].Count >= originalFileSize)
        {
            Debug.Log("All chunks received, reconstructing audio...");
            byte[] completeFile = receivedChunks[senderId].ToArray();
            receivedChunks.Remove(senderId);
            StartCoroutine(PlayReceivedMusic(completeFile));
        }
    }

    private IEnumerator PlayReceivedMusic(byte[] fileData)
    {
        string path = Path.Combine(Application.persistentDataPath, "received_audio.mp3");
        File.WriteAllBytes(path, fileData);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load audio: " + www.error);
                yield break;
            }

            AudioClip newClip = DownloadHandlerAudioClip.GetContent(www);

            // �����̃N���b�v��j���i���������[�N�h�~�j
            if (audioSource.clip != null)
            {
                Destroy(audioSource.clip);
            }

            audioSource.clip = newClip;
            audioSource.Play();
        }
    }
}
