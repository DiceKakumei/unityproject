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
        if (!doinit)  // doinit �� false �̏ꍇ
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
        doinit = true;  // ��� doinit �� true �ɂ���
        SceneManager.LoadScene("Title");
    }
}
