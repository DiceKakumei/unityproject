using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnTitleInHost : MonoBehaviour
{

    [SerializeField] Button gotitle;
    // Start is called before the first frame update
    void Start()
    {
        gotitle.onClick.AddListener(ChangeToTitle);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void ChangeToTitle()
    {
        SceneManager.LoadScene("Title");
        Debug.Log("GoTitle");
    }
}