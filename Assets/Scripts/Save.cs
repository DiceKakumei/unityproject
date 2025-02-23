using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SceneSaveManager : MonoBehaviour
{
    public Transform sceneManager; // SceneManager の Transform

    private string savePath => Path.Combine(Application.persistentDataPath, "sceneData.json");

    [SerializeField] Button Save;
    [SerializeField] Button Load;
    void Start()
    {
        Save.onClick.AddListener(SaveScene);
        Load.onClick.AddListener(LoadScene);
    }

    public void SaveScene()
    {
        SceneManagerData sceneManagerData = new SceneManagerData();

        foreach (Transform scene in sceneManager) // 各シーンごとに処理
        {
            SceneData sceneData = new SceneData { sceneName = scene.name };

            foreach (Transform obj in scene) // シーン内のオブジェクトを保存
            {
                // Resources フォルダ内のパスを取得（例: "Models/MyModel"）
                string resourcePath = obj.GetComponent<GetPath>().path;

                ObjectData objData = new ObjectData
                {
                    name = obj.name,
                    fbxPath = resourcePath,
                    position = obj.position,
                    rotation = obj.rotation,
                    scale = obj.localScale
                };
                sceneData.objects.Add(objData);
            }

            sceneManagerData.scenes.Add(sceneData);
        }

        string json = JsonUtility.ToJson(sceneManagerData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Scene saved to: " + savePath);
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
            }
        }

        Debug.Log("Scene loaded from: " + savePath);
    }

    /*private string GetResourcePath(GameObject obj)
    {
        // Resources 以下のパスを取得（Assets/Resources/Models/MyModel.prefab → "Models/MyModel"）
        string path = UnityEditor.AssetDatabase.GetAssetPath(obj);
        if (string.IsNullOrEmpty(path) || !path.StartsWith("Assets/Resources/"))
        {
            Debug.LogWarning($"Object {obj.name} is not in Resources folder, skipping.");
            return "";
        }
        path = path.Substring("Assets/Resources/".Length); // "Models/MyModel.prefab"
        path = Path.ChangeExtension(path, null); // 拡張子を削除して "Models/MyModel"
        return path;
    }*/
}

[System.Serializable]
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
}
