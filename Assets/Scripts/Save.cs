using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SceneSaveManager : MonoBehaviour
{
    public Transform sceneManager; // SceneManager �� Transform

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

        foreach (Transform scene in sceneManager) // �e�V�[�����Ƃɏ���
        {
            SceneData sceneData = new SceneData { sceneName = scene.name };

            foreach (Transform obj in scene) // �V�[�����̃I�u�W�F�N�g��ۑ�
            {
                // Resources �t�H���_���̃p�X���擾�i��: "Models/MyModel"�j
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

        // �����̃V�[�����폜
        foreach (Transform scene in sceneManager)
        {
            Destroy(scene.gameObject);
        }

        // �V�[���ƃI�u�W�F�N�g�𕜌�
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
        // Resources �ȉ��̃p�X���擾�iAssets/Resources/Models/MyModel.prefab �� "Models/MyModel"�j
        string path = UnityEditor.AssetDatabase.GetAssetPath(obj);
        if (string.IsNullOrEmpty(path) || !path.StartsWith("Assets/Resources/"))
        {
            Debug.LogWarning($"Object {obj.name} is not in Resources folder, skipping.");
            return "";
        }
        path = path.Substring("Assets/Resources/".Length); // "Models/MyModel.prefab"
        path = Path.ChangeExtension(path, null); // �g���q���폜���� "Models/MyModel"
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
    public string fbxPath; // Resources ���̃p�X (��: "Models/MyModel")
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}
