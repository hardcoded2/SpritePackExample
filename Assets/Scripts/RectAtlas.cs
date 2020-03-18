using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class RectAtlasPaged
    {
        public List<Texture2D> Tex; 
        public List<RectSprite> RectSprites;
        //TODO: use .userdata to eliminate hard references to this
        //public List<Sprite> SpriteRefs;
        public void Generate(List<Sprite> toPack)
        {
            //sprites are unique, rects can collide, don't want to worry about semantics
            SortedDictionary<Sprite,Rect> spriteBoundsDictionary = new SortedDictionary<Sprite, Rect>(); //likely can be eliminated
            foreach (var sprite in toPack)
            {
                if (sprite == null) continue; //handle bad editor data
                spriteBoundsDictionary.Add(sprite,sprite.rect);
            }
            //spriteBoundsDictionary
        }

        public class SpriteAreaComparer : IComparer<Sprite>
        {
            public int Compare(Sprite x, Sprite y)
            {
                if (x.Equals(y)) return 0;
                if (x.rect.Equals(y.rect)) return SpriteRefCompare(x, y); // probably non-deterministic resolution for equla area objects

                var xArea = AreaOfSprite(x);
                var yArea = AreaOfSprite(y);
                //if areas are same, do ref compare using hashcodes
                if (Mathf.Approximately(xArea, yArea)) return SpriteRefCompare(x, y);
                return xArea < yArea ? -1 : 1;
            }

            private int SpriteRefCompare(Sprite x, Sprite y)
            {
                if (x.GetHashCode() == y.GetHashCode()) return 0;
                return x.GetHashCode() < y.GetHashCode() ? -1 : 1;
            }

            private float AreaOfSprite(Sprite sprite)
            {
                return sprite.textureRect.width * sprite.textureRect.height;
            }
        }
        public class SpriteWidthComparer : IComparer<Sprite>
        {
            public int Compare(Sprite x, Sprite y)
            {
                if (x.Equals(y)) return 0;
                if (x.rect.Equals(y.rect)) return SpriteRefCompare(x, y); // probably non-deterministic resolution for equla area objects

                var xWidth = x.rect.width;
                var yWidth = y.rect.width;
                //if areas are same, do ref compare using hashcodes
                if (Mathf.Approximately(xWidth, yWidth)) return SpriteRefCompare(x, y);
                return xWidth < yWidth ? -1 : 1;
            }

            private int SpriteRefCompare(Sprite x, Sprite y)
            {
                if (x.GetHashCode() == y.GetHashCode()) return 0;
                return x.GetHashCode() < y.GetHashCode() ? -1 : 1;
            }

            private float AreaOfSprite(Sprite sprite)
            {
                return sprite.textureRect.width * sprite.textureRect.height;
            }
        }

        public RectSprite SettingsForImage(string spriteNameToSet)
        {
            //TODO: use real dictionary and handle name collisions
            foreach (var sprite in RectSprites)
            {
                if (sprite.SpriteName == spriteNameToSet) return sprite;
            }
            throw new ArgumentException($"sprite {spriteNameToSet} not in atlas");
        }
    }
    [Serializable]
    public class RectSprite
    {
        public RectAtlasPaged Atlas; //reference atlas - design choice unity makes, cribbing off that
        public Texture2D Tex;
        public string SpriteName;
        public Vector2 Offset;
        public Vector2 Tiling;
        //Rect for tiling/offset here?
        
        public void Apply(Material material)
        {
            material.mainTexture = Tex; //make sure we are using correct material
            material.mainTextureOffset = Offset;
            material.mainTextureScale = Tiling;
        }
    }
}
