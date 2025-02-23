using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Chat : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Button CloseTab;
    [SerializeField] GameObject panel;
    [SerializeField] Image TextPrefab;
    [SerializeField] TMP_InputField chat;
    private GameObject Canvas;
    private int i = 0;
    //Button button;
    void Start()
    {
        panel.SetActive(false);
        Canvas = GameObject.Find("Canvas");
        button.onClick.AddListener(() => panel.SetActive(true));
        CloseTab.onClick.AddListener(() => panel.SetActive(false));
        chat.onEndEdit.AddListener(delegate (string chatlog)
        {
            Image newChat = Instantiate(TextPrefab);
            //newChat.SetActive(true);
            newChat.name = chatlog;
            newChat.GetComponentInChildren<TextMeshProUGUI>().text = chatlog;
            newChat.transform.parent = Canvas.transform;
            newChat.transform.parent = panel.transform;
            //前のボックスサイズ+10分下に下げる
            newChat.transform.position = new Vector3(1680, 800 - i * 110, 0);
            //テキストの長さに合わせてサイズ変更する昨日
            newChat.transform.localScale = new Vector3(4, 1, 1);
            i++;
        });
    }


}
