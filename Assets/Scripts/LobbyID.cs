using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
//using GUIUtility;

public class LobbyID : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI m_lobbyIdText;
    [SerializeField] private Button button;
    

    // Start is called before the first frame update
    void Start()
    {
        //ƒƒr[ì¬or“üŽºŽž‚É‹L‰¯‚µ‚Ä‚¨‚¢‚½LobbyID‚ðÝ’è
        m_lobbyIdText.text = SteamLobby.Instance.LobbyID.ToString();
        button.onClick.AddListener(()=> Copy(m_lobbyIdText.text));
    }

    void Copy(string Text)
    {
        GUIUtility.systemCopyBuffer = Text;
        Debug.Log("Copyed Text:"+Text);
        return;
    }
}
