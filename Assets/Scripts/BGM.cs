using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class BGM : MonoBehaviour
{
    [SerializeField] Button OpenUI;//UIを呼び出す
    [SerializeField] Button AddBGM;
    [SerializeField] GameObject AddBGMPanel;
    [SerializeField] GameObject ButtonPrefabPanel;
    [SerializeField] Button musicItemPrefab;//BGMリストのボタンプレファブ
    [SerializeField] Button CloseTab;
    private AudioSource audioSource;
    private List<string> bgmPaths = new List<string>();//読み込んだBGMのパスリスト
    private string PATH;
    private int i = 0;
    private GameObject Canvas;

    void Start()
    {
        Canvas = GameObject.Find("Canvas");
        AddBGMPanel.SetActive(false);
        ButtonPrefabPanel.SetActive(false);
        //sicItemPrefab.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        OpenUI.onClick.AddListener(() => AddBGMPanel.SetActive(true));
        AddBGM.onClick.AddListener(LoadMusic);
        CloseTab.onClick.AddListener(() => AddBGMPanel.SetActive(false));
    }

    public void LoadMusic()
    {
        PATH = UnityEditor.EditorUtility.OpenFilePanel("Select Music File", "", "mp3,wav,ogg");
        Debug.Log("FilePath = " + PATH);
        if (PATH != null && PATH != "")
        {
            bgmPaths.Add(PATH);
            CreateMusicListItem(bgmPaths[i],i);
            i++;
        }
    }

    void CreateMusicListItem(string filePath, int i)
    {
        string var = Path.GetFileName(filePath);
        var = var.Substring(0, 6);
        //なんやかんやしてキャンバス上にインスタンスが生成されるようにする
        Button newItem = Instantiate(musicItemPrefab);
        newItem.GetComponentInChildren<TextMeshProUGUI>().text = var;
        newItem.GetComponent<Button>().onClick.AddListener(() => PlayMusic(filePath));
        newItem.transform.parent = Canvas.transform;
        newItem.transform.parent = AddBGMPanel.transform;
        newItem.transform.position = new Vector3(530, 740-i*100, 0);
        newItem.transform.localScale = new Vector3(2,2,1);
    }

    void PlayMusic(string path)
    {
        StartCoroutine(LoadMusicCoroutine(path));
    }

    IEnumerator LoadMusicCoroutine(string path)
    {
        var www = new WWW("file://" + path);
        yield return www;

        audioSource.clip = www.GetAudioClip(false, false);
        audioSource.Play();
    }
}
