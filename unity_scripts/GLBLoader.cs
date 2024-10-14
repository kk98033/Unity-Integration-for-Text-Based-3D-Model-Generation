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
        // 祇癳更叫―
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        // 浪琩琌Θ更
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error downloading GLB file: {request.error}");
            yield break;
        }

        // 盢更 .glb ゅン糶セ纗
        File.WriteAllBytes(savePath, request.downloadHandler.data);
        Debug.Log($"Downloaded GLB file saved to: {savePath}");

        // 更更 .glb ゅンミ初春
        LoadGLBFile(savePath);
    }

    void LoadGLBFile(string path)
    {
        // ㄏノ UnityGLTF ㄓ更 .glb ゅン
        GLTFComponent gltfComponent = gameObject.AddComponent<GLTFComponent>();
        gltfComponent.GLTFUri = path;
        gltfComponent.Load();  // 更初春
        Debug.Log("GLB file loaded into the scene.");
    }
}
