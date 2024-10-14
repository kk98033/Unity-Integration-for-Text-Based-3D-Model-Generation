import json
from urllib import request, parse
import random 
import string
import time
from datetime import datetime
import requests

#This is the ComfyUI api prompt format.

#If you want it for a specific workflow you can "enable dev mode options"
#in the settings of the UI (gear beside the "Queue Size: ") this will enable
#a button on the UI to save workflows in api format.

#keep in mind ComfyUI is pre alpha software so this format will change a bit.

#this is the one for the default workflow
prompt_text = """
{
  "5": {
    "inputs": {
      "width": 1024,
      "height": 1024,
      "batch_size": 1
    },
    "class_type": "EmptyLatentImage",
    "_meta": {
      "title": "Empty Latent Image"
    }
  },
  "6": {
    "inputs": {
      "text": "A modern, minimalist bed with a light wood frame and headboard, without any background. The bed is neatly made with light blue bedding and a white textured blanket draped across the middle. Three pillows are arranged at the head of the bed: two in dark grey and one in white with a woven pattern. The design is simple, clean, and evokes a sense of calm and comfort, isolated on a plain background.",
      "clip": [
        "11",
        0
      ]
    },
    "class_type": "CLIPTextEncode",
    "_meta": {
      "title": "CLIP Text Encode (Prompt)"
    }
  },
  "8": {
    "inputs": {
      "samples": [
        "13",
        0
      ],
      "vae": [
        "10",
        0
      ]
    },
    "class_type": "VAEDecode",
    "_meta": {
      "title": "VAE Decode"
    }
  },
  "9": {
    "inputs": {
      "filename_prefix": "ComfyUI",
      "images": [
        "8",
        0
      ]
    },
    "class_type": "SaveImage",
    "_meta": {
      "title": "Save Image"
    }
  },
  "10": {
    "inputs": {
      "vae_name": "ae.safetensors"
    },
    "class_type": "VAELoader",
    "_meta": {
      "title": "Load VAE"
    }
  },
  "11": {
    "inputs": {
      "clip_name1": "t5xxl_fp8_e4m3fn.safetensors",
      "clip_name2": "clip_l.safetensors",
      "type": "flux"
    },
    "class_type": "DualCLIPLoader",
    "_meta": {
      "title": "DualCLIPLoader"
    }
  },
  "12": {
    "inputs": {
      "unet_name": "flux1-schnell.safetensors",
      "weight_dtype": "fp8_e4m3fn"
    },
    "class_type": "UNETLoader",
    "_meta": {
      "title": "Load Diffusion Model"
    }
  },
  "13": {
    "inputs": {
      "noise": [
        "25",
        0
      ],
      "guider": [
        "22",
        0
      ],
      "sampler": [
        "16",
        0
      ],
      "sigmas": [
        "17",
        0
      ],
      "latent_image": [
        "5",
        0
      ]
    },
    "class_type": "SamplerCustomAdvanced",
    "_meta": {
      "title": "SamplerCustomAdvanced"
    }
  },
  "16": {
    "inputs": {
      "sampler_name": "euler"
    },
    "class_type": "KSamplerSelect",
    "_meta": {
      "title": "KSamplerSelect"
    }
  },
  "17": {
    "inputs": {
      "scheduler": "simple",
      "steps": 1,
      "denoise": 1,
      "model": [
        "12",
        0
      ]
    },
    "class_type": "BasicScheduler",
    "_meta": {
      "title": "BasicScheduler"
    }
  },
  "22": {
    "inputs": {
      "model": [
        "12",
        0
      ],
      "conditioning": [
        "6",
        0
      ]
    },
    "class_type": "BasicGuider",
    "_meta": {
      "title": "BasicGuider"
    }
  },
  "25": {
    "inputs": {
      "noise_seed": 320704391881978
    },
    "class_type": "RandomNoise",
    "_meta": {
      "title": "RandomNoise"
    }
  },
  "26": {
    "inputs": {
      "model_name": "model.safetensors"
    },
    "class_type": "[Comfy3D] Load SF3D Model",
    "_meta": {
      "title": "Load SF3D Model"
    }
  },
  "27": {
    "inputs": {
      "texture_resolution": 1024,
      "remesh_option": "None",
      "sf3d_model": [
        "26",
        0
      ],
      "reference_image": [
        "30",
        0
      ],
      "reference_mask": [
        "30",
        1
      ]
    },
    "class_type": "[Comfy3D] StableFast3D",
    "_meta": {
      "title": "StableFast3D"
    }
  },
  "28": {
    "inputs": {
      "mesh_file_path": [
        "29",
        0
      ],
      "preview3d": null
    },
    "class_type": "[Comfy3D] Preview 3DMesh",
    "_meta": {
      "title": "Preview 3DMesh"
    }
  },
  "29": {
    "inputs": {
      "save_path": "SF3D_Test\\\\mesh.glb",
      "mesh": [
        "31",
        0
      ]
    },
    "class_type": "[Comfy3D] Save 3D Mesh",
    "_meta": {
      "title": "Save 3D Mesh"
    }
  },
  "30": {
    "inputs": {
      "foreground_ratio": 0.9,
      "images": [
        "32",
        0
      ],
      "masks": [
        "32",
        1
      ]
    },
    "class_type": "[Comfy3D] Resize Image Foreground",
    "_meta": {
      "title": "Resize Image Foreground"
    }
  },
  "31": {
    "inputs": {
      "axis_x_to": "-x",
      "axis_y_to": "+y",
      "axis_z_to": "-z",
      "flip_normal": false,
      "scale": 1,
      "mesh": [
        "27",
        0
      ]
    },
    "class_type": "[Comfy3D] Switch Mesh Axis",
    "_meta": {
      "title": "Switch Mesh Axis"
    }
  },
  "32": {
    "inputs": {
      "rembg_session": [
        "33",
        0
      ],
      "image": [
        "8",
        0
      ]
    },
    "class_type": "ImageRemoveBackground+",
    "_meta": {
      "title": "🔧 Image Remove Background"
    }
  },
  "33": {
    "inputs": {
      "model": "u2net: general purpose",
      "providers": "CPU"
    },
    "class_type": "RemBGSession+",
    "_meta": {
      "title": "🔧 RemBG Session"
    }
  },
  "34": {
    "inputs": {
      "images": [
        "32",
        0
      ]
    },
    "class_type": "PreviewImage",
    "_meta": {
      "title": "Preview Image"
    }
  },
  "35": {
    "inputs": {
      "image": [
        "32",
        0
      ],
      "alpha": [
        "37",
        0
      ]
    },
    "class_type": "JoinImageWithAlpha",
    "_meta": {
      "title": "Join Image with Alpha"
    }
  },
  "36": {
    "inputs": {
      "images": [
        "35",
        0
      ]
    },
    "class_type": "PreviewImage",
    "_meta": {
      "title": "Preview Image"
    }
  },
  "37": {
    "inputs": {
      "mask": [
        "32",
        1
      ]
    },
    "class_type": "InvertMask",
    "_meta": {
      "title": "InvertMask"
    }
  },
  "39": {
    "inputs": {
      "images": [
        "8",
        0
      ]
    },
    "class_type": "PreviewImage",
    "_meta": {
      "title": "Preview Image"
    }
  }
}
"""

def queue_prompt(prompt, filename):
    prompt["29"]["inputs"]["save_path"] = f"SF3D_Test\\\\for_API\\\\{filename}.glb"
    p = {"prompt": prompt}
    data = json.dumps(p).encode('utf-8')
    req =  request.Request("http://127.0.0.1:8188/prompt", data=data)
    request.urlopen(req)

    # 檢查模型是否生成
    wait_for_model_to_generate(filename)
    download_model_file(filename)

def wait_for_model_to_generate(filename, timeout=300, interval=5):
    """
    等待指定時間 (timeout 秒) 內，檢查伺服器上是否生成了目標文件
    """
    url = f"http://210.240.160.27:8189/download/{filename}"
    elapsed_time = 0
    
    while elapsed_time < timeout:
        try:
            # 嘗試檢查文件是否可用
            response = requests.head(url)
            if response.status_code == 200:
                print(f"Model {filename}.glb is ready for download!")
                return True
        except:
            # 文件還沒生成或伺服器連接失敗
            print('not yet')
            print(url)
            pass
        
        # 等待 interval 秒再檢查一次
        time.sleep(interval)
        elapsed_time += interval
    
    raise TimeoutError(f"Model {filename}.glb was not generated within {timeout} seconds.")


def generate_random_code():
    # Generate a random 15-digit number
    random_code = ''.join([str(random.randint(0, 9)) for _ in range(15)])
    return random_code

def generate_timestamped_code(length=5):
    current_time = datetime.now().strftime("%Y%m%d_%H%M%S")
    random_string = ''.join(random.choices(string.ascii_uppercase, k=length))
    return f"{current_time}_{random_string}"

def download_model_file(filename):
    url = f"http://210.240.160.27:8189/download/{filename}"
    response = requests.get(url)
    
    if response.status_code == 200:
        with open(f"{filename}.glb", 'wb') as f:
            f.write(response.content)
        print(f"Model {filename}.glb downloaded successfully!")
    else:
        print(f"Failed to download the file. Status code: {response.status_code}")

prompt = json.loads(prompt_text)
#set the text prompt for our positive CLIPTextEncode
prompt["6"]["inputs"]["text"] = "A detailed 3D rendering of SpongeBob SquarePants' iconic pineapple house, isolated on a plain white background. The house should feature the familiar pineapple texture with a vibrant orange-yellow hue, green leafy top, and the signature blue circular windows, wooden door, and chimney. No other background elements or surroundings, just the pineapple house itself in all its quirky, cartoonish glory."

#set the seed for our KSampler node
prompt["25"]["inputs"]["noise_seed"] = generate_random_code()

filename = generate_timestamped_code()

queue_prompt(prompt, filename)

download_model_file(filename)