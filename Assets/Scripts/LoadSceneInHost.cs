using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LoadSceneInHost : NetworkBehaviour
{
    public Transform sceneManager; // SceneManager の Transform
    private string savePath => Path.Combine(Application.persistentDataPath, "sceneData.json");
    [SerializeField] Button Load;
    // Start is called before the first frame update
    void Start()
    {
        if (IsHost)
        {
            Load.onClick.AddListener(LoadScene);
        }
        
    }
    public void LoadScene()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogError("Save file not found!");
            return;
        }

        string json = File.ReadAllText(savePath);
        SceneManagerData sceneManagerData = JsonUtility.FromJson<SceneManagerData>(json);

        // 既存のシーンを削除
        foreach (Transform scene in sceneManager)
        {
            Destroy(scene.gameObject);
        }

        // シーンとオブジェクトを復元
        foreach (SceneData sceneData in sceneManagerData.scenes)
        {
            GameObject sceneObj = new GameObject(sceneData.sceneName);
            sceneObj.transform.SetParent(sceneManager);
            sceneObj.AddComponent<NetworkObject>();

            foreach (ObjectData objData in sceneData.objects)
            {
                GameObject prefab = Resources.Load<GameObject>(objData.fbxPath);
                if (prefab == null)
                {
                    Debug.LogError($"Failed to load prefab at {objData.fbxPath}");
                    return;
                }

                GameObject obj = Instantiate(prefab, objData.position, objData.rotation);
                obj.transform.SetParent(sceneObj.transform);
                obj.transform.localScale = objData.scale;
                obj.name = objData.name;
                obj.AddComponent<NetworkObject>();
            }
        }

        Debug.Log("Scene loaded from: " + savePath);
    }
}

/*[System.Serializable]
public class SceneManagerData
{
    public List<SceneData> scenes = new List<SceneData>();
}

[System.Serializable]
public class SceneData
{
    public string sceneName;
    public List<ObjectData> objects = new List<ObjectData>();
}

[System.Serializable]
public class ObjectData
{
    public string name;
    public string fbxPath; // Resources 内のパス (例: "Models/MyModel")
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}*/