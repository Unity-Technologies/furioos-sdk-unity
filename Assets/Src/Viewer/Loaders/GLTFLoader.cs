using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Rise.Core;

using UnityGLTF;

namespace Rise.Viewer.Loaders {
    public class GLTFLoader : MonoBehaviour {
        public bool Multithreaded = true;
        public bool UseStream = false;

        public int MaximumLod = 300;

        public Shader GLTFStandard;
        public Shader GLTFConstant;

        public void Load(string url) {
            StartCoroutine(AsyncLoad(url));
        }

        private IEnumerator AsyncLoad(string url) {
            GLTFSceneImporter loader = null;
            FileStream gltfStream = null;

            if(UseStream) {
                var fullPath = Application.streamingAssetsPath + url;
                gltfStream = File.OpenRead(fullPath);
                loader = new GLTFSceneImporter(
                    fullPath,
                    gltfStream,
                    gameObject.transform
                );
            }
            else {
                loader = new GLTFSceneImporter(
                    url,
                    gameObject.transform
                );
            }

            loader.SetShaderForMaterialType(GLTFSceneImporter.MaterialType.PbrMetallicRoughness, GLTFStandard);
            loader.SetShaderForMaterialType(GLTFSceneImporter.MaterialType.CommonConstant, GLTFConstant);
            loader.MaximumLod = MaximumLod;

            yield return loader.Load(-1, Multithreaded);

            if(gltfStream != null) {
                #if WINDOWS_UWP
			        gltfStream.Dispose();
                #else
                    gltfStream.Close();
                #endif
            }
        }
    }
}