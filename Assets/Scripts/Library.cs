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

    private Dictionary<string, GameObject> loadedModels = new Dictionary<string,GameObject>();
    public Dictionary<string, GameObject> ModelPath = new Dictionary<string, GameObject>();
    //Dictionaryをもう一つPath管理用に作れば良いのでは？
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
        GameObject[] models = Resources.LoadAll<GameObject>("ObjectFBX");
        //GameObject[] models = Resources.LoadAll<GameObject>("");

        foreach (var model in models)
        {
            string modelName = model.name;
            string editorpath = "ObjectFBX/" + model.name;
            string hostpath = "ObjectLibrary/" + model.name;
            Debug.Log($"Loading model:{modelName}");
            Debug.Log($"Loading model Path:{editorpath}");

            //同じ名前のモデルが登録されていないかをチェック
            if (!loadedModels.ContainsKey(modelName))
            {
                loadedModels.Add(modelName, model);
                ModelPath.Add(editorpath, model);
                CreateButton(modelName,editorpath,hostpath);
            }
        }
    }

    void CreateButton(string modelName,string editorpath,string hostpath)
    {
        Button newButton = Instantiate(ObjectPrefab);
        newButton.transform.parent = panel.transform;
        newButton.GetComponentInChildren<TextMeshProUGUI>().text = modelName;
        newButton.onClick.AddListener(() => SpawnModel(modelName,editorpath,hostpath));
    }

    void SpawnModel(string modelName, string editorpath,string hostpath)
    {
        Scenes scenes = GetComponent<Scenes>();
        if (loadedModels.TryGetValue(modelName,out GameObject modelPrefab))
        {
            GameObject newInstance = Instantiate(modelPrefab);
            newInstance.transform.position = Vector3.zero;
            Debug.Log(scenes.ActiveSceneName);
            newInstance.transform.parent = GameObject.Find(scenes.ActiveSceneName).transform;
            newInstance.AddComponent<GetPath>();
            newInstance.GetComponent<GetPath>().EditorPath = editorpath;
            newInstance.GetComponent<GetPath>().HostPath = hostpath;
        }
    }
}
