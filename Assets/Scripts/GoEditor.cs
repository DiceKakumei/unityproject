using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoEditor : MonoBehaviour
{
    [SerializeField] Button goeditor;
    static bool doinit = false;

    void Start()
    {
        Debug.Log("doinit: " + doinit);
        if (!doinit)  // doinit が false の場合
        {
            init();
        }
        else
        {
            goeditor.onClick.AddListener(ChangeToEditor);
        }
    }

    public void ChangeToEditor()
    {
        SceneManager.LoadScene("Editor");
        Debug.Log("GoEditor");
    }

    void init()
    {
        doinit = true;  // 先に doinit を true にする
        SceneManager.LoadScene("Title");
    }
}
