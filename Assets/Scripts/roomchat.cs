using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class roomchat : MonoBehaviour
{
    [SerializeField] Button button;
    //Button button;
    void Start()
    {
        button.onClick.AddListener(() => Debug.Log("test"));
    }
}
