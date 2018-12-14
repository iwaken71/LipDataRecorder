using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CyberV
{
    public class LipDataSeaquence : ScriptableObject
    {
        [System.SerializableAttribute]
        public class SerializeLipData
        {
            public int FrameCount;

            public float Time;
            public LipBlendShape lipBlendShape;

            [System.Serializable]
            public class LipBlendShape
            {
                [Range(0f, 100f)]
                public float[] blendShapes = new float[5];

                public LipBlendShape(float[] weight)
                {
                    if (weight.Length != 5)
                    {
                        return;
                    }
                    for (int i = 0; i < weight.Length; i++)
                    {
                        blendShapes[i] = Mathf.Clamp(weight[i], 0, 100);
                    }
                }
            }
        }
        public BlendShapeIndex[] blendShapesIndex;
        public List<SerializeLipData> serializeLipDatas = new List<SerializeLipData>();
    }
    [System.Serializable]
    public class BlendShapeIndex
    {
        public string Name;
        public int[] AIUEOIndex = new int[5];
    }

    [System.Serializable]
    public class MeshBlendShapeIndex
    {
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public BlendShapeIndex blendShapeIndex;
    }
}
