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
    //Dictionary���������Path�Ǘ��p�ɍ��Ηǂ��̂ł́H
    //�ǂݍ��񂾃��f�����L���b�V��

    void Start()
    {
        panel.SetActive(false);
        LibraryButton.onClick.AddListener(() => panel.SetActive(true));
        AddObj.onClick.AddListener(() => LoadFBXList());
        CloseTab.onClick.AddListener(() => panel.SetActive(false));
    }

    void LoadFBXList()
    {
        //FBX���擾
        GameObject[] models = Resources.LoadAll<GameObject>("ObjectLibrary");
        //GameObject[] models = Resources.LoadAll<GameObject>("");

        foreach (var model in models)
        {
            string modelName = model.name;
            string path = "ObjectLibrary/" + model.name;
            Debug.Log($"Loading model:{modelName}");
            Debug.Log($"Loading model Path:{path}");

            //�������O�̃��f�����o�^����Ă��Ȃ������`�F�b�N
            if (!loadedModels.ContainsKey(modelName))
            {
                loadedModels.Add(modelName, model);
                ModelPath.Add(path, model);
                CreateButton(modelName,path);
            }
        }
    }

    void CreateButton(string modelName,string path)
    {
        Button newButton = Instantiate(ObjectPrefab);
        newButton.transform.parent = panel.transform;
        newButton.GetComponentInChildren<TextMeshProUGUI>().text = modelName;
        newButton.onClick.AddListener(() => SpawnModel(modelName,path));
    }

    void SpawnModel(string modelName, string path)
    {
        Scenes scenes = GetComponent<Scenes>();
        if (loadedModels.TryGetValue(modelName,out GameObject modelPrefab))
        {
            GameObject newInstance = Instantiate(modelPrefab);
            newInstance.transform.position = Vector3.zero;
            Debug.Log(scenes.ActiveSceneName);
            newInstance.transform.parent = GameObject.Find(scenes.ActiveSceneName).transform;
            newInstance.AddComponent<GetPath>();
            newInstance.GetComponent<GetPath>().path = path;
        }
    }
}
