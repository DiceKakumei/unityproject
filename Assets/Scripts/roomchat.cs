using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class roomchat : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Button CloseTab;
    [SerializeField] GameObject panel;
    [SerializeField] Button TextPrefab;
    [SerializeField] TMP_InputField Chat;
    private GameObject Canvas;
    private int i = 0;
    //Button button;
    void Start()
    {
        panel.SetActive(false);
        Canvas = GameObject.Find("Canvas");
        button.onClick.AddListener(() => panel.SetActive(true));
        CloseTab.onClick.AddListener(() => panel.SetActive(false));
        Chat.onEndEdit.AddListener(delegate (string chatlog)
        {
            Button newChat = Instantiate(TextPrefab);
            newChat.name = chatlog;
            newChat.GetComponentInChildren<TextMeshProUGUI>().text = chatlog;
            newChat.transform.parent = Canvas.transform;
            newChat.transform.parent = panel.transform;
            newChat.transform.position = new Vector3(1680, 900 - i * 60, 0);
            newChat.transform.localScale = new Vector3(8, 2, 2);
            i++;
        });
    }


}
