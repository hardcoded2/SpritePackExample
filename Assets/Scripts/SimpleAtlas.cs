using System;
using UnityEngine;
using UnityEngine.Assertions;

public class SimpleAtlas : MonoBehaviour
{
    //evenly packed assumed
    [Serializable]
    public class AtlasInfo
    {
        public Vector2 SpriteSizeInPixels = Vector2.one * 50f;
        public Texture2D Tex; //duplicate info for now

        public TextureSettings SettingsForImage(Vector2Int texNumber)
        {
            Assert.AreEqual(Tex.height, Tex.width); //square only supported
            var upDownUnit = Tex.height / SpriteSizeInPixels.y; // 400/50 = 8
            //1 / 8 (where 8 is number of things) = .125 - uvspace increment
            var leftRightUnit = Tex.width / SpriteSizeInPixels.x;
            var upDownUnitUV = 1/upDownUnit; // 1/8 = 0.125
            var leftRightUnitUV = 1/leftRightUnit;
            var maxVal = new Vector2Int(Mathf.RoundToInt(leftRightUnit), Mathf.RoundToInt(upDownUnit)); // should be 8,8
            Assert.IsTrue(texNumber.x >= 0 && texNumber.x < maxVal.x);
            Assert.IsTrue(texNumber.y >= 0 && texNumber.y < maxVal.y);

            return new TextureSettings
            {
                Tiling = new Vector2(upDownUnitUV, leftRightUnitUV),
                Offset = new Vector2(leftRightUnitUV * texNumber.x, 1- (upDownUnitUV * (texNumber.y+1)))
            };
        }
    }

    public class TextureSettings
    {
        public Vector2 Offset;
        public Vector2 Tiling;
    }

    [SerializeField] private AtlasInfo m_AtlasInfo;
}
