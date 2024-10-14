# ComfyUI SF3D Example Project

這個專案包含一個簡單的 Unity 場景範例以及如何使用 ComfyUI 和 SF3D 生成 3D 模型並將其導入 Unity。

## 專案內容

- **ComfyUI_SF3D_ExampleProject.unitypackage**：這是一個簡單搭建的 Unity 場景範例。
- **flux_3d_api.json**：這是 ComfyUI 的 workflow 文件。
- **glb_file_server.py**：這個 Python 腳本用來傳送 GLB 檔案到 Unity。要開著它才能傳送 GLB 檔案。

### 使用說明

1. 確保你已經修改了 `glb_file_server.py` 裡的 GLB 路徑：
   ``` 
   output_directory = r"你的路徑"
   ```
   這裡的路徑要改成你 ComfyUI 生成 GLB 的資料夾位置。

2. 運行 `glb_file_server.py` 來啟動服務，這樣 Unity 才能接收 GLB 檔案。

## ComfyUI 設置

這裡我使用了 Flux 生圖功能和 SF3D 搭配 ComfyUI-3D-Pack （這個 3D Pack **超級無敵難裝** 💀，一定要照著 GitHub 的 README 一步一步來）。

- [Flux 生圖 GitHub 專案](https://github.com/Stability-AI/stable-fast-3d)
- [ComfyUI-3D-Pack GitHub 專案](https://github.com/MrForExample/ComfyUI-3D-Pack)

🚨 **注意**：第一次開啟 ComfyUI 時，建議先隨便在網頁上生成一個模型，因為第一次載入模型時會 **超級無敵久**!!!! 🚨

### 為什麼選用 Flux？

使用 Flux 生圖的原因是因為生成的圖品質更高。在生成圖時建議在 prompt 裡加上 `clean background, 3d XXX`，效果比較好。

### 參考影片

如果你在安裝 ComfyUI 或 3D Pack 時遇到問題，可以參考以下安裝影片：

1. [ComfyUI 安裝指南 1](https://youtu.be/vDPVuJW17kI?si=q6yL-mBYmD0HHQIH)
2. [ComfyUI 安裝指南 2](https://youtu.be/RjuoLMCpvbI?si=OQUO3SPAcxW4w-OR)
3. [ComfyUI 安裝指南 3](https://youtu.be/aPQ8gvTNCKM?si=fTPK8aOHepfywCoz)