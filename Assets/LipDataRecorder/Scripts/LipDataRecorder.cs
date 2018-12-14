using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace CyberV
{
    public class LipDataRecorder : MonoBehaviour
    {
        [SerializeField] string fileName = "Name";
        [SerializeField] KeyCode recordStartKey = KeyCode.R;
        [SerializeField] KeyCode recordStopKey = KeyCode.X;

        [SerializeField]
        MeshBlendShapeIndex[] meshBlendShapes;
        [SerializeField] bool recording = false;
        [SerializeField] int frameIndex = 0;

        Action onRecordEnd;

        float recordedTime = 0;

        LipDataSeaquence lipDataSeaquence;
        DateTime startDataTime;

        void Update()
        {
            if (Input.GetKeyDown(recordStartKey))
            {
                RecordStart();
            }

            if (Input.GetKeyDown(recordStopKey))
            {
                RecordEnd();
            }
        }

        public void RecordStart()
        {
            if (recording == false)
            {
                if(meshBlendShapes == null || meshBlendShapes.Length == 0){
                    return;
                }
                frameIndex = 0;
                recordedTime = 0;
                lipDataSeaquence = ScriptableObject.CreateInstance<LipDataSeaquence>();
                lipDataSeaquence.blendShapesIndex = new BlendShapeIndex[meshBlendShapes.Length];
                for (int i = 0; i < meshBlendShapes.Length;i++){
                    lipDataSeaquence.blendShapesIndex[i] = meshBlendShapes[i].blendShapeIndex;
                }
                onRecordEnd += WriteLipDataFile;
                recording = true;
                startDataTime = DateTime.Now;
            }
        }
        public void RecordEnd()
        {
            if (recording)
            {
                if (onRecordEnd != null)
                {
                    onRecordEnd();
                    onRecordEnd = null;
                }

                recording = false;
            }
        }
        void LateUpdate()
        {
            if (recording)
            {
                recordedTime += Time.deltaTime;

                var serializedLip = new LipDataSeaquence.SerializeLipData();
                serializedLip.lipBlendShape = new LipDataSeaquence.SerializeLipData.LipBlendShape(GetCurrentLipBlendShape());
                serializedLip.FrameCount = frameIndex;
                serializedLip.Time = recordedTime;

                lipDataSeaquence.serializeLipDatas.Add(serializedLip);
                frameIndex++;
            }
        }

        void WriteLipDataFile()
        {
            string filePath = "Assets/LipDataRecorder/Resources/RecordingData/" + startDataTime.ToString("yyyy_MMdd");
            SafeCreateDirectory(filePath);

            string path = AssetDatabase.GenerateUniqueAssetPath(
                filePath + "/" + fileName + "_" + startDataTime.ToString("HHmmss") +
                ".asset");
            AssetDatabase.CreateAsset(lipDataSeaquence, path);
            AssetDatabase.Refresh();
            frameIndex = 0;
            recordedTime = 0;
        }

        float[] GetCurrentLipBlendShape(){
            float[] output = new float[5];
            for (int i = 0; i < output.Length; i++){
                output[i] = meshBlendShapes[0].skinnedMeshRenderer.GetBlendShapeWeight(meshBlendShapes[0].blendShapeIndex.AIUEOIndex[i]) * 100;
            }
            return output;
        }

        public static DirectoryInfo SafeCreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return null;
            }

            return Directory.CreateDirectory(path);
        }
    }
}