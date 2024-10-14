from flask import Flask, send_file, abort
import os

app = Flask(__name__)

''' 記得改成放你生成的GLB資料夾路徑 '''
# 文件存放路徑
output_directory = r"D:\stable_diffusion\comfyui_3d\ComfyUI_windows_portable_nvidia\ComfyUI_windows_portable\ComfyUI\output\SF3D_Test\for_API"

@app.route('/download/<filename>')
def download_file(filename):
    # 組合完整路徑
    file_path = os.path.join(output_directory, f"{filename}.glb")
    print(file_path)
    # 檢查文件是否存在
    if os.path.exists(file_path):
        return send_file(file_path, as_attachment=True)
    else:
        abort(404, description="File not found")

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8189)