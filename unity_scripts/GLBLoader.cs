using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityGLTF; 

public class GLBLoader : MonoBehaviour
{
    // API URL
    public string apiURL = "http://127.0.0.1:8189/download/testModel.glb";

    private string localPath;

    void Start()
    {
        localPath = Path.Combine(Application.persistentDataPath, "testModel.glb");
        StartCoroutine(DownloadAndLoadGLB(apiURL, localPath));
    }

    IEnumerator DownloadAndLoadGLB(string url, string savePath)
    {
        // �o�e�U���ШD
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        // �ˬd�O�_���\�U��
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error downloading GLB file: {request.error}");
            yield break;
        }

        // �N�U���� .glb ���g�J�쥻�a�s�x
        File.WriteAllBytes(savePath, request.downloadHandler.data);
        Debug.Log($"Downloaded GLB file saved to: {savePath}");

        // �[���U���� .glb ���A�ߧY��J����
        LoadGLBFile(savePath);
    }

    void LoadGLBFile(string path)
    {
        // �ϥ� UnityGLTF �ӥ[�� .glb ���
        GLTFComponent gltfComponent = gameObject.AddComponent<GLTFComponent>();
        gltfComponent.GLTFUri = path;
        gltfComponent.Load();  // �[���é�J����
        Debug.Log("GLB file loaded into the scene.");
    }
}
