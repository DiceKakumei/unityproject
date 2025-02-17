using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Scenes : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Button Scene;
    [SerializeField] Button AddScene;
    [SerializeField] Button DelScene;
    [SerializeField] Button CloseTab;
    [SerializeField] Button chooseScene;
    [SerializeField] GameObject ScenePrefab;
    [SerializeField] TMP_InputField SceneName;

    private GameObject Canvas;
    private int i = 0;
    private List<string> SceneNameList = new List<string>();
    private List<GameObject> AllScenes = new List<GameObject>();
    public string ActiveSceneName;
    //Button button;
    void Start()
    {
        Canvas = GameObject.Find("Canvas");
        panel.SetActive(false);
        Scene.onClick.AddListener(() => panel.SetActive(true));
        SceneName.onEndEdit.AddListener(delegate (string sceneName)
        {
            SceneNameList.Add(sceneName);
        });
        AddScene.onClick.AddListener((CreateNewScene));
        CloseTab.onClick.AddListener(() => panel.SetActive(false));
    }
    /*CreateNewSceneでやりたいことはワールド内の内装を変えることだから、
      Emptyかなんかを親にしてシーンにあるオブジェクトをそこに紐づけて
     それをシーン遷移のUIで行き来出来るようにしよう*/
    void CreateNewScene()
    {
        if (i<SceneNameList.Count && !string.IsNullOrEmpty(SceneNameList[i]))
        {
            string sceneName = SceneNameList[i];
            for (int a = 0; a < i; a++)
            {
                if (SceneNameList[i] == SceneNameList[a]) return;
            }
            GameObject newScene = Instantiate(ScenePrefab);
            AllScenes.Add(newScene);
            newScene.name = SceneNameList[i];
            Button newSc = Instantiate(chooseScene);
            newSc.GetComponentInChildren<TextMeshProUGUI>().text = SceneNameList[i];
            newSc.transform.parent = Canvas.transform;
            newSc.transform.parent = panel.transform;
            newSc.transform.position = new Vector3(530, 700 - i * 100, 0);
            newSc.transform.localScale = new Vector3(2, 2, 1);
            newSc.GetComponent<Button>().onClick.AddListener(() => ChangeScene(sceneName));
            if (i > 0) newScene.SetActive(false);
            i++;
        }
        //インスタンス化したインプットフィールドをなんやかんやして複製

        //SceneName.onEndEdit.AddListener(() => SceneNameList.Add(SceneName.txt));
        //GameObject newsecne = Instantiate(ScenePrefab);
        //newsecne.name = SceneNameList[i];
        //i++;

        //AddTextBoxToWriteSceneName
    }

    void ChangeScene(string scenename)
    {
        foreach (var allobject in AllScenes)
        {
            allobject.SetActive(true);
        }
        GameObject var = GameObject.Find(scenename);
        foreach (var allobject in AllScenes)
        {
            allobject.SetActive(false);
        }
        var.SetActive(true);
        ActiveSceneName = scenename;

    }


}
