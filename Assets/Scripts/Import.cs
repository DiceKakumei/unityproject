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
    /// FBX�t�@�C����ǂݍ��ރN���X
    /// </summary>
    public static class LoadFBX
    {
        /// <summary>
        /// �w�肳�ꂽ�p�X��FBX���f����񓯊��I�ɓǂݍ��݁AGameObject��Ԃ��܂��B
        /// </summary>
        /// <param name="modelPath">���f���t�@�C���̃p�X�iStreamingAssets����̑��΃p�X�j</param>
        /// <param name="cancellationToken"></param>
        /// <returns>�ǂݍ��܂ꂽ���f����GameObject��Ԃ�UniTask</returns>
        public static async UniTask<GameObject> LoadModelAsync(
            string modelPath,
            CancellationToken cancellationToken)
        {
            // ���f���t�@�C���̃t���p�X���쐬
            string fullPath = Path.Combine(Application.streamingAssetsPath, modelPath);

            // ���f���t�@�C�������݂��邩�m�F
            if (!File.Exists(fullPath))
            {
                Log.Error("[LoadFBX] ���f���t�@�C����������܂���: {0}", fullPath);
                return null;
            }

            // FBX�t�@�C���̃f�B���N�g�����擾
            string fbxDirectory = Path.GetDirectoryName(fullPath);

            // �d���������o�b�N�O���E���h�X���b�h�Ŏ��s
            ModelData modelData = await UniTask.RunOnThreadPool(async () =>
            {
                // Assimp�̃C���|�[�^�[���쐬
                AssimpContext importer = new AssimpContext();

                // �V�[�����C���|�[�g
                Scene scene;
                try
                {
                    scene = importer.ImportFile(fullPath, PostProcessPreset.TargetRealTimeMaximumQuality);
                }
                catch (Exception ex)
                {
                    Log.Error("[LoadFBX] ���f���̃C���|�[�g���ɃG���[���������܂���: {0}", ex.Message);
                    return null;
                }

                if (scene == null || !scene.HasMeshes)
                {
                    Log.Error("[LoadFBX] ���f���̃C���|�[�g�Ɏ��s���܂����B");
                    return null;
                }

                Log.Info("[LoadFBX] ���f���̃C���|�[�g�ɐ������܂����B");

                // ���f���f�[�^��ێ�����N���X���쐬
                ModelData data = new ModelData
                {
                    Name = Path.GetFileNameWithoutExtension(modelPath),
                    Meshes = new List<MeshData>(),
                    Materials = new List<MaterialData>()
                };

                // �}�e���A��������
                foreach (var assimpMaterial in scene.Materials)
                {
                    var materialData = await ProcessMaterialAsync(assimpMaterial, fbxDirectory);
                    data.Materials.Add(materialData);
                }

                // �m�[�h������
                ProcessNode(scene.RootNode, scene, data);

                return data;
            }, cancellationToken: cancellationToken);

            // �o�b�N�O���E���h�����ŃG���[���������ꍇ
            if (modelData == null)
            {
                return null;
            }

            // ���C���X���b�h��GameObject�𐶐�
            GameObject modelRoot = new GameObject(modelData.Name);

            // ���b�V���ƃ}�e���A����ݒ�
            foreach (var meshData in modelData.Meshes)
            {
                GameObject meshObject = new GameObject(meshData.Name);
                meshObject.transform.SetParent(modelRoot.transform, false);

                MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();

                // UnityEngine.Mesh���쐬
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
                    Log.Error("[LoadFBX] MToon10 �V�F�[�_�[��������܂���B"
                        + "UniVRM �p�b�P�[�W���������C���|�[�g����Ă��邩�m�F���Ă��������B");

                    // �t�H�[���o�b�N�Ƃ��� Standard �V�F�[�_�[���g�p
                    lilToon = Shader.Find("Universal Render Pipeline/Lit");
                }

                UnityEngineMaterial material = new UnityEngineMaterial(lilToon);

                // �J���[��ݒ�
                material.SetColor("_Color", meshData.Material.Color);

                if (meshData.Material.TextureData != null)
                {
                    Texture2D texture = new Texture2D(2, 2);
                    if (texture.LoadImage(meshData.Material.TextureData))
                    {
                        // �e�N�X�`����ݒ�
                        material.SetTexture("_MainTex", texture);
                    }
                    else
                    {
                        Log.Warning("[LoadFBX] �e�N�X�`���̃��[�h�Ɏ��s���܂����B");
                    }
                }

                meshRenderer.material = material;
            }

            Log.Info("[LoadFBX] ���f���̓ǂݍ��݂�GameObject�̍쐬���������܂����B");

            return modelRoot;
        }

        /// <summary>
        /// �m�[�h���������ă��b�V���f�[�^�����W
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
        /// Assimp��Mesh��MeshData�ɕϊ�����
        /// </summary>
        private static MeshData ConvertAssimpMesh(AssimpMesh mesh, MaterialData materialData)
        {
            MeshData meshData = new MeshData
            {
                Name = mesh.Name,
                Material = materialData
            };

            // ���_���W
            meshData.Vertices = new Vector3[mesh.VertexCount];
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vector3D vertex = mesh.Vertices[i];
                meshData.Vertices[i] = new Vector3(vertex.X, vertex.Y, vertex.Z);
            }

            // �@��
            if (mesh.HasNormals)
            {
            }
}*/
