using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class Edit
    {
        // A Test behaves as an ordinary method
        [Test]
        public void EditSimplePasses()
        {
            // Use the Assert class to test conditions
            var atlasInfo = new SimpleAtlas.AtlasInfo()
            {
                Tex = new Texture2D(400,400),SpriteSizeInPixels = 50f * Vector2.one
            };
            //Assert.AreApproximatelyEqual(atlasInfo.Tex.width,50f);
            //Assert.AreApproximatelyEqual(atlasInfo.Tex.height,50f);
            var settingsForImage = atlasInfo.SettingsForImage(new Vector2Int(0, 0));
            Assert.AreEqual(new Vector2(0.125f,0.125f ),settingsForImage.Tiling);
            Assert.AreEqual(new Vector2(0,0.875f), settingsForImage.Offset);
                
            settingsForImage = atlasInfo.SettingsForImage(new Vector2Int(1, 0));
            Assert.AreEqual(new Vector2(0.125f,0.125f ),settingsForImage.Tiling);
            Assert.AreEqual(new Vector2(0.125f,0.875f), settingsForImage.Offset);
            
            settingsForImage = atlasInfo.SettingsForImage(new Vector2Int(0, 1));
            Assert.AreEqual(new Vector2(0.125f,0.125f ),settingsForImage.Tiling);
            Assert.AreEqual(new Vector2(0f,0.75f), settingsForImage.Offset);
            
            settingsForImage = atlasInfo.SettingsForImage(new Vector2Int(0, 7));
            Assert.AreEqual(new Vector2(0f,0f), settingsForImage.Offset);
            
            Assert.AreEqual(new Vector2(0.875f,0f), atlasInfo.SettingsForImage(new Vector2Int(7, 7)).Offset);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator EditWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
