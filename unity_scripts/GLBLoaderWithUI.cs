using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;  // 使用 TextMeshPro
using UnityGLTF;
using System;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GLBPromptLoader : MonoBehaviour
{
    public TMP_InputField promptInputField;  // TextMeshPro 的 Input Field，用來接收用戶的 prompt
    public TMP_Text statusText;              // TextMeshPro 用來顯示當前狀態
    public Button generateButton;            // 生成按鈕
    public Toggle physicsToggle;             // Checkbox，用於選擇是否加上物理碰撞

    private string localPath;
    private string apiUrl = "http://127.0.0.1:8188/prompt";  
    private string downloadUrl = "http://210.240.160.27:8189/download/";

    void Start()
    {
        generateButton.onClick.AddListener(OnGenerateButtonClicked);  // 設置按鈕點擊事件
    }

    void OnGenerateButtonClicked()
    {
        string userPrompt = promptInputField.text;  // 獲取用戶輸入的 prompt
        if (!string.IsNullOrEmpty(userPrompt))
        {
            StartCoroutine(SendPromptToServer(userPrompt));
        }
        else
        {
            statusText.text = "Prompt cannot be empty!";
        }
    }

    IEnumerator SendPromptToServer(string promptText)
    {
        // 創建 JSON 並設置 prompt
        string filename = GenerateTimestampedCode();
        string jsonPayload = CreatePromptJson(promptText, filename);

        // 發送 API 請求
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log(jsonPayload);
        Debug.Log(filename);

        // 等待回應
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            statusText.text = "Prompt sent successfully! Checking model generation...";
            StartCoroutine(WaitForModelAndDownload(filename));  // 開始檢查並下載生成的模型
        }
        else
        {
            statusText.text = "Error sending prompt: " + request.error;
            Debug.Log(statusText.text);
        }
    }

    IEnumerator WaitForModelAndDownload(string filename)
    {
        string downloadPath = downloadUrl + filename;
        int elapsedTime = 0;
        int timeout = 300;  // 5 分鐘超時

        while (elapsedTime < timeout)
        {
            // 檢查文件是否已生成
            UnityWebRequest request = UnityWebRequest.Head(downloadPath);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                statusText.text = "Model is ready! Downloading...";
                StartCoroutine(DownloadModelFile(filename));  // 開始下載模型
                yield break;
            }

            // 等待 5 秒後再次檢查
            yield return new WaitForSeconds(5);
            elapsedTime += 5;
        }

        statusText.text = "Model generation timed out.";
    }

    IEnumerator DownloadModelFile(string filename)
    {
        string downloadPath = downloadUrl + filename;
        UnityWebRequest request = UnityWebRequest.Get(downloadPath);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            localPath = Path.Combine(Application.persistentDataPath, filename);
            File.WriteAllBytes(localPath, request.downloadHandler.data);
            statusText.text = "Model downloaded successfully! Loading...";

            LoadGLBFile(localPath);  // 加載下載的模型到場景中
        }
        else
        {
            statusText.text = "Failed to download the model: " + request.error;
        }
    }

    void LoadGLBFile(string path)
    {
        // 生成一個新的空 GameObject
        GameObject newObject = new GameObject("GLTFModel");

        // 設定新物件的初始位置為 (0, 0, 0)，並稍微抬高
        newObject.transform.position = new Vector3(0, 0.5f, 0); // 把模型抬高 0.5f
        newObject.transform.rotation = Quaternion.Euler(0, -180f, 0);

        // 檢查是否加上物理碰撞
        if (physicsToggle != null && physicsToggle.isOn)
        {
            // 為模型添加物理碰撞和剛體
            newObject.AddComponent<BoxCollider>();  // 添加簡單的 BoxCollider
            newObject.AddComponent<Rigidbody>();    // 添加剛體，使物件受物理影響
        }

        // 使用 UnityGLTF 來加載 .glb 文件
        GLTFComponent gltfComponent = newObject.AddComponent<GLTFComponent>();
        gltfComponent.GLTFUri = path;
        gltfComponent.Load();

        // 更新狀態文字，通知使用者模型已經加載
        statusText.text = "The model has been loaded into the scene, positioned at (0, 0.5, 0)!";
    }

    // 生成帶有時間戳的文件名
    string GenerateTimestampedCode()
    {
        return DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + UnityEngine.Random.Range(10000, 99999).ToString();
    }

    // 創建 JSON Payload，將用戶的 prompt 插入
    string CreatePromptJson(string userPrompt, string filename)
    {
        // 這裡根據 comfyUI prompt 結構生成對應的 JSON
        string basePromptJson = @"
        {
  ""5"": {
    ""inputs"": {
      ""width"": 1024,
      ""height"": 1024,
      ""batch_size"": 1
    },
    ""class_type"": ""EmptyLatentImage"",
    ""_meta"": {
      ""title"": ""Empty Latent Image""
    }
  },
  ""6"": {
    ""inputs"": {
      ""text"": ""A modern, minimalist bed with a light wood frame and headboard, without any background. The bed is neatly made with light blue bedding and a white textured blanket draped across the middle. Three pillows are arranged at the head of the bed: two in dark grey and one in white with a woven pattern. The design is simple, clean, and evokes a sense of calm and comfort, isolated on a plain background."",
      ""clip"": [
        ""11"",
        0
      ]
    },
    ""class_type"": ""CLIPTextEncode"",
    ""_meta"": {
      ""title"": ""CLIP Text Encode (Prompt)""
    }
  },
  ""8"": {
    ""inputs"": {
      ""samples"": [
        ""13"",
        0
      ],
      ""vae"": [
        ""10"",
        0
      ]
    },
    ""class_type"": ""VAEDecode"",
    ""_meta"": {
      ""title"": ""VAE Decode""
    }
  },
  ""9"": {
    ""inputs"": {
      ""filename_prefix"": ""ComfyUI"",
      ""images"": [
        ""8"",
        0
      ]
    },
    ""class_type"": ""SaveImage"",
    ""_meta"": {
      ""title"": ""Save Image""
    }
  },
  ""10"": {
    ""inputs"": {
      ""vae_name"": ""ae.safetensors""
    },
    ""class_type"": ""VAELoader"",
    ""_meta"": {
      ""title"": ""Load VAE""
    }
  },
  ""11"": {
    ""inputs"": {
      ""clip_name1"": ""t5xxl_fp8_e4m3fn.safetensors"",
      ""clip_name2"": ""clip_l.safetensors"",
      ""type"": ""flux""
    },
    ""class_type"": ""DualCLIPLoader"",
    ""_meta"": {
      ""title"": ""DualCLIPLoader""
    }
  },
  ""12"": {
    ""inputs"": {
      ""unet_name"": ""flux1-schnell.safetensors"",
      ""weight_dtype"": ""fp8_e4m3fn""
    },
    ""class_type"": ""UNETLoader"",
    ""_meta"": {
      ""title"": ""Load Diffusion Model""
    }
  },
  ""13"": {
    ""inputs"": {
      ""noise"": [
        ""25"",
        0
      ],
      ""guider"": [
        ""22"",
        0
      ],
      ""sampler"": [
        ""16"",
        0
      ],
      ""sigmas"": [
        ""17"",
        0
      ],
      ""latent_image"": [
        ""5"",
        0
      ]
    },
    ""class_type"": ""SamplerCustomAdvanced"",
    ""_meta"": {
      ""title"": ""SamplerCustomAdvanced""
    }
  },
  ""16"": {
    ""inputs"": {
      ""sampler_name"": ""euler""
    },
    ""class_type"": ""KSamplerSelect"",
    ""_meta"": {
      ""title"": ""KSamplerSelect""
    }
  },
  ""17"": {
    ""inputs"": {
      ""scheduler"": ""simple"",
      ""steps"": 1,
      ""denoise"": 1,
      ""model"": [
        ""12"",
        0
      ]
    },
    ""class_type"": ""BasicScheduler"",
    ""_meta"": {
      ""title"": ""BasicScheduler""
    }
  },
  ""22"": {
    ""inputs"": {
      ""model"": [
        ""12"",
        0
      ],
      ""conditioning"": [
        ""6"",
        0
      ]
    },
    ""class_type"": ""BasicGuider"",
    ""_meta"": {
      ""title"": ""BasicGuider""
    }
  },
  ""25"": {
    ""inputs"": {
      ""noise_seed"": 320704391881978
    },
    ""class_type"": ""RandomNoise"",
    ""_meta"": {
      ""title"": ""RandomNoise""
    }
  },
  ""26"": {
    ""inputs"": {
      ""model_name"": ""model.safetensors""
    },
    ""class_type"": ""[Comfy3D] Load SF3D Model"",
    ""_meta"": {
      ""title"": ""Load SF3D Model""
    }
  },
  ""27"": {
    ""inputs"": {
      ""texture_resolution"": 1024,
      ""remesh_option"": ""None"",
      ""sf3d_model"": [
        ""26"",
        0
      ],
      ""reference_image"": [
        ""30"",
        0
      ],
      ""reference_mask"": [
        ""30"",
        1
      ]
    },
    ""class_type"": ""[Comfy3D] StableFast3D"",
    ""_meta"": {
      ""title"": ""StableFast3D""
    }
  },
  ""28"": {
    ""inputs"": {
      ""mesh_file_path"": [
        ""29"",
        0
      ],
      ""preview3d"": null
    },
    ""class_type"": ""[Comfy3D] Preview 3DMesh"",
    ""_meta"": {
      ""title"": ""Preview 3DMesh""
    }
  },
  ""29"": {
    ""inputs"": {
      ""save_path"": ""SF3D_Test\\\\mesh.glb"",
      ""mesh"": [
        ""31"",
        0
      ]
    },
    ""class_type"": ""[Comfy3D] Save 3D Mesh"",
    ""_meta"": {
      ""title"": ""Save 3D Mesh""
    }
  },
  ""30"": {
    ""inputs"": {
      ""foreground_ratio"": 0.9,
      ""images"": [
        ""32"",
        0
      ],
      ""masks"": [
        ""32"",
        1
      ]
    },
    ""class_type"": ""[Comfy3D] Resize Image Foreground"",
    ""_meta"": {
      ""title"": ""Resize Image Foreground""
    }
  },
  ""31"": {
    ""inputs"": {
      ""axis_x_to"": ""-x"",
      ""axis_y_to"": ""+y"",
      ""axis_z_to"": ""-z"",
      ""flip_normal"": false,
      ""scale"": 1,
      ""mesh"": [
        ""27"",
        0
      ]
    },
    ""class_type"": ""[Comfy3D] Switch Mesh Axis"",
    ""_meta"": {
      ""title"": ""Switch Mesh Axis""
    }
  },
  ""32"": {
    ""inputs"": {
      ""rembg_session"": [
        ""33"",
        0
      ],
      ""image"": [
        ""8"",
        0
      ]
    },
    ""class_type"": ""ImageRemoveBackground+"",
    ""_meta"": {
      ""title"": ""🔧 Image Remove Background""
    }
  },
  ""33"": {
    ""inputs"": {
      ""model"": ""u2net: general purpose"",
      ""providers"": ""CPU""
    },
    ""class_type"": ""RemBGSession+"",
    ""_meta"": {
      ""title"": ""🔧 RemBG Session""
    }
  },
  ""34"": {
    ""inputs"": {
      ""images"": [
        ""32"",
        0
      ]
    },
    ""class_type"": ""PreviewImage"",
    ""_meta"": {
      ""title"": ""Preview Image""
    }
  },
  ""35"": {
    ""inputs"": {
      ""image"": [
        ""32"",
        0
      ],
      ""alpha"": [
        ""37"",
        0
      ]
    },
    ""class_type"": ""JoinImageWithAlpha"",
    ""_meta"": {
      ""title"": ""Join Image with Alpha""
    }
  },
  ""36"": {
    ""inputs"": {
      ""images"": [
        ""35"",
        0
      ]
    },
    ""class_type"": ""PreviewImage"",
    ""_meta"": {
      ""title"": ""Preview Image""
    }
  },
  ""37"": {
    ""inputs"": {
      ""mask"": [
        ""32"",
        1
      ]
    },
    ""class_type"": ""InvertMask"",
    ""_meta"": {
      ""title"": ""InvertMask""
    }
  },
  ""39"": {
    ""inputs"": {
      ""images"": [
        ""8"",
        0
      ]
    },
    ""class_type"": ""PreviewImage"",
    ""_meta"": {
      ""title"": ""Preview Image""
    }
  }
}";

        // Parse the base JSON
        JObject promptObject = JObject.Parse(basePromptJson);

        // 加上用戶的 prompt
        promptObject["6"]["inputs"]["text"] = userPrompt;

        // 自訂檔案名稱
        promptObject["29"]["inputs"]["save_path"] = $"SF3D_Test\\for_API\\{filename}.glb";

        // Wrap the prompt inside a top-level "prompt" key
        JObject finalPayload = new JObject
        {
            ["prompt"] = promptObject
        };

        return finalPayload.ToString(Formatting.None);
    }
}
