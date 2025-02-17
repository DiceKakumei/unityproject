/*using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using Assimp;
using UnityEngine;
using Unity.Logging;
using Cysharp.Threading.Tasks;
using AssimpMaterial = Assimp.Material;
using AssimpMesh = Assimp.Mesh;
using UnityEngineMaterial = UnityEngine.Material;
using UnityEngineMesh = UnityEngine.Mesh;
using TMPro;

public class Import : MonoBehaviour
{
    [SerializeField] Button Im;
    private string PATH;
    public Scenes Scenes;

    // Start is called before the first frame update
    void Start()
    {
        if (Scenes.ActiveSceneName != null) Debug.Log("FuckYOU");
        Im.onClick.AddListener(() => LoadObj());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadObj()
    {
        if (Scenes.ActiveSceneName == null) return;
        PATH = UnityEditor.EditorUtility.OpenFilePanel("Select FBX File", "", "fbx");

    }

    /*namespace uDesktopMascot
{
    /// <summary>
    /// FBXファイルを読み込むクラス
    /// </summary>
    public static class LoadFBX
    {
        /// <summary>
        /// 指定されたパスのFBXモデルを非同期的に読み込み、GameObjectを返します。
        /// </summary>
        /// <param name="modelPath">モデルファイルのパス（StreamingAssetsからの相対パス）</param>
        /// <param name="cancellationToken"></param>
        /// <returns>読み込まれたモデルのGameObjectを返すUniTask</returns>
        public static async UniTask<GameObject> LoadModelAsync(
            string modelPath,
            CancellationToken cancellationToken)
        {
            // モデルファイルのフルパスを作成
            string fullPath = Path.Combine(Application.streamingAssetsPath, modelPath);

            // モデルファイルが存在するか確認
            if (!File.Exists(fullPath))
            {
                Log.Error("[LoadFBX] モデルファイルが見つかりません: {0}", fullPath);
                return null;
            }

            // FBXファイルのディレクトリを取得
            string fbxDirectory = Path.GetDirectoryName(fullPath);

            // 重い処理をバックグラウンドスレッドで実行
            ModelData modelData = await UniTask.RunOnThreadPool(async () =>
            {
                // Assimpのインポーターを作成
                AssimpContext importer = new AssimpContext();

                // シーンをインポート
                Scene scene;
                try
                {
                    scene = importer.ImportFile(fullPath, PostProcessPreset.TargetRealTimeMaximumQuality);
                }
                catch (Exception ex)
                {
                    Log.Error("[LoadFBX] モデルのインポート中にエラーが発生しました: {0}", ex.Message);
                    return null;
                }

                if (scene == null || !scene.HasMeshes)
                {
                    Log.Error("[LoadFBX] モデルのインポートに失敗しました。");
                    return null;
                }

                Log.Info("[LoadFBX] モデルのインポートに成功しました。");

                // モデルデータを保持するクラスを作成
                ModelData data = new ModelData
                {
                    Name = Path.GetFileNameWithoutExtension(modelPath),
                    Meshes = new List<MeshData>(),
                    Materials = new List<MaterialData>()
                };

                // マテリアルを処理
                foreach (var assimpMaterial in scene.Materials)
                {
                    var materialData = await ProcessMaterialAsync(assimpMaterial, fbxDirectory);
                    data.Materials.Add(materialData);
                }

                // ノードを処理
                ProcessNode(scene.RootNode, scene, data);

                return data;
            }, cancellationToken: cancellationToken);

            // バックグラウンド処理でエラーがあった場合
            if (modelData == null)
            {
                return null;
            }

            // メインスレッドでGameObjectを生成
            GameObject modelRoot = new GameObject(modelData.Name);

            // メッシュとマテリアルを設定
            foreach (var meshData in modelData.Meshes)
            {
                GameObject meshObject = new GameObject(meshData.Name);
                meshObject.transform.SetParent(modelRoot.transform, false);

                MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();

                // UnityEngine.Meshを作成
                UnityEngineMesh unityMesh = new UnityEngineMesh
                {
                    name = meshData.Name,
                    vertices = meshData.Vertices,
                    normals = meshData.Normals,
                    uv = meshData.UVs,
                    colors = meshData.Colors,
                    triangles = meshData.Indices
                };
                unityMesh.RecalculateBounds();

                meshFilter.mesh = unityMesh;

                Shader lilToon = Shader.Find("Hidden/lilToonCutout");
                if (lilToon == null)
                {
                    Log.Error("[LoadFBX] MToon10 シェーダーが見つかりません。"
                        + "UniVRM パッケージが正しくインポートされているか確認してください。");

                    // フォールバックとして Standard シェーダーを使用
                    lilToon = Shader.Find("Universal Render Pipeline/Lit");
                }

                UnityEngineMaterial material = new UnityEngineMaterial(lilToon);

                // カラーを設定
                material.SetColor("_Color", meshData.Material.Color);

                if (meshData.Material.TextureData != null)
                {
                    Texture2D texture = new Texture2D(2, 2);
                    if (texture.LoadImage(meshData.Material.TextureData))
                    {
                        // テクスチャを設定
                        material.SetTexture("_MainTex", texture);
                    }
                    else
                    {
                        Log.Warning("[LoadFBX] テクスチャのロードに失敗しました。");
                    }
                }

                meshRenderer.material = material;
            }

            Log.Info("[LoadFBX] モデルの読み込みとGameObjectの作成が完了しました。");

            return modelRoot;
        }

        /// <summary>
        /// ノードを処理してメッシュデータを収集
        /// </summary>
        private static void ProcessNode(Node node, Scene scene, ModelData modelData)
        {
            foreach (int meshIndex in node.MeshIndices)
            {
                AssimpMesh mesh = scene.Meshes[meshIndex];
                MeshData meshData = ConvertAssimpMesh(mesh,
                    modelData.Materials[mesh.MaterialIndex]);
                modelData.Meshes.Add(meshData);
            }

            foreach (Node childNode in node.Children)
            {
                ProcessNode(childNode, scene, modelData);
            }
        }

        /// <summary>
        /// AssimpのMeshをMeshDataに変換する
        /// </summary>
        private static MeshData ConvertAssimpMesh(AssimpMesh mesh, MaterialData materialData)
        {
            MeshData meshData = new MeshData
            {
                Name = mesh.Name,
                Material = materialData
            };

            // 頂点座標
            meshData.Vertices = new Vector3[mesh.VertexCount];
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vector3D vertex = mesh.Vertices[i];
                meshData.Vertices[i] = new Vector3(vertex.X, vertex.Y, vertex.Z);
            }

            // 法線
            if (mesh.HasNormals)
            {
            }
}*/
