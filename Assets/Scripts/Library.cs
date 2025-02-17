using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class Library : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Button LibraryButton;
    [SerializeField] Button AddObj;
    [SerializeField] Button CloseTab;
    [SerializeField] Button ObjectPrefab;

    private Dictionary<string, GameObject> loadedModels = new Dictionary<string, GameObject>();
    //読み込んだモデルをキャッシュ

    void Start()
    {
        panel.SetActive(false);
        LibraryButton.onClick.AddListener(() => panel.SetActive(true));
        AddObj.onClick.AddListener(() => LoadFBXList());
        CloseTab.onClick.AddListener(() => panel.SetActive(false));
    }

    void LoadFBXList()
    {
        //FBXを取得
        GameObject[] models = Resources.LoadAll<GameObject>("ObjectLibrary");

        foreach(var model in models)
        {
            string modelName = model.name;
            Debug.Log($"Loading model:{modelName}");

            //同じ名前のモデルが登録されていないかをチェック
            if (!loadedModels.ContainsKey(modelName))
            {
                loadedModels.Add(modelName, model);
                CreateButton(modelName);
            }
        }
    }

    void CreateButton(string modelName)
    {
        Button newButton = Instantiate(ObjectPrefab);
        newButton.transform.parent = panel.transform;
        newButton.GetComponentInChildren<TextMeshProUGUI>().text = modelName;
        newButton.onClick.AddListener(() => SpawnModel(modelName));
    }

    void SpawnModel(string modelName)
    {
        Scenes scenes = GetComponent<Scenes>();
        if (loadedModels.TryGetValue(modelName,out GameObject modelPrefab))
        {
            GameObject newInstance = Instantiate(modelPrefab);
            newInstance.transform.position = Vector3.zero;
            Debug.Log(scenes.ActiveSceneName);
            newInstance.transform.parent = GameObject.Find(scenes.ActiveSceneName).transform;
        }
    }
}
