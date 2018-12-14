using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CyberV
{
    public class LipDataPlayer : MonoBehaviour
    {
        [SerializeField] KeyCode playStartKey = KeyCode.S;
        [SerializeField] KeyCode playStopKey = KeyCode.T;

        [SerializeField] LipDataSeaquence recordingLipData;

        [SerializeField] MeshBlendShapeIndex[] LipMeshBlendShapes;

        System.Action onPlayFinish;

        [SerializeField]
        [Tooltip("再生開始フレームを指定します。0だとファイル先頭から開始です")]
        int startFrame = 0;

        [SerializeField] bool playing = false;
        [SerializeField] int frameIndex = 0;

        float playingTime = 0;

        public void PlayLip()
        {
            if (playing == false)
            {
                if (recordingLipData == null)
                {
                    Debug.LogError("録画済みモーションデータが指定されていません。再生を行いません。");
                }
                else
                {
                    playingTime = startFrame * (Time.deltaTime / 1f);
                    frameIndex = startFrame;
                    playing = true;
                }
            }
        }
        public void StopLip()
        {
            if (playing)
            {
                playingTime = 0;
                frameIndex = startFrame;
                playing = false;
            }
        }
        
        void SetLip()
        {
            var lipData = recordingLipData.serializeLipDatas[frameIndex].lipBlendShape;

            for (int i = 0; i < LipMeshBlendShapes.Length;i++){
                for (int j = 0; j < 5;j++){
                    LipMeshBlendShapes[i].skinnedMeshRenderer.SetBlendShapeWeight(recordingLipData.blendShapesIndex[i].AIUEOIndex[j],lipData.blendShapes[j]);
                }
            }

            //処理落ちしたモーションデータの再生速度調整
            if (playingTime > recordingLipData.serializeLipDatas[frameIndex].Time)
            {
                frameIndex++;
            }

            if (frameIndex == recordingLipData.serializeLipDatas.Count - 1)
            {
                if (onPlayFinish != null)
                {
                    onPlayFinish();
                }
            }
        }

        // Use this for initialization
        void Awake()
        {
            onPlayFinish += StopLip;

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(playStartKey))
            {
                PlayLip();
            }

            if (Input.GetKeyDown(playStopKey))
            {
                StopLip();
            }
        }
        private void LateUpdate()
        {
            if (!playing) return;
            playingTime += Time.deltaTime;
            SetLip();
        }
    }
}
