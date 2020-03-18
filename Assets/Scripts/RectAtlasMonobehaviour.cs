using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class RectAtlasMonobehaviour : UnityEngine.MonoBehaviour
    {
        //TODO: make sure that these sprites are only in editor, ie use .userdata assetimporter (gets saved in .meta). or odin serialization
        [SerializeField] private List<Sprite> m_TestSpritesToAtlas;
        [SerializeField] private RectAtlasPaged m_RectAtlas;

        private Material GetMaterialSafe()
        {
            var rend = GetComponent<Renderer>();
            Material mat = !Application.isPlaying ? rend.sharedMaterial : rend.material;
            return mat;
        }
        [SerializeField] private string m_SpriteNameToSet;

        [ContextMenu("Test Generate")]
        public void TestGen()
        {
            m_RectAtlas.Generate(m_TestSpritesToAtlas);
        }

        [ContextMenu("TestApply")]
        public void TestApply()
        {
            m_RectAtlas.Generate(m_TestSpritesToAtlas);
            var settings = m_RectAtlas.SettingsForImage(m_SpriteNameToSet);
            var renderer = GetComponent<Renderer>();
            Material mat = null;
            mat = !Application.isPlaying ? renderer.sharedMaterial : renderer.material;
            settings.Apply(mat);
        }
    }
}
