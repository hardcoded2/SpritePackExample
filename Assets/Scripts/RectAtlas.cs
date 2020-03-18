using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class RectAtlas
    {
        //public List<Texture2D> Tex; 
        public Texture2D Tex;
        public List<RectSprite> RectSprites;
        //TODO: use .userdata to eliminate hard references to this
        //public List<Sprite> SpriteRefs;
        public void Generate(List<Sprite> toPack)
        {
            //sprites are unique, rects can collide, don't want to worry about semantics
            
            List<Sprite> spritesToSort = new List<Sprite>(toPack);
            
            //spriteBoundsDictionary
            
            spritesToSort.Sort(new SpriteAreaComparer());
            foreach (var sprite in spritesToSort)
            {
                Debug.Log($" sprite name  {sprite.name} area {sprite.rect.width * sprite.rect.height}");
            }

            GenerateOnNewTexture(spritesToSort);
        }
        const int TEX_SIZE = 2048;
        private bool CanPotentiallyPlaceBasedOnTotalArea(List<Sprite> sprites)
        {
            double TexArea = TEX_SIZE * TEX_SIZE;
            double spriteAggregateArea = 0;
            foreach (var sprite in sprites)
            {
                //Is there floating point drifting from floats? 
                spriteAggregateArea += sprite.rect.width * sprite.rect.height;
            }

            return TexArea >= spriteAggregateArea;
        }
//mutates a ton of state, pretty dirty
        private void GenerateOnNewTexture(List<Sprite> sortedSprites)
        {
            if(!CanPotentiallyPlaceBasedOnTotalArea(sortedSprites)) throw new ArgumentException("sprites rect is too big for tex");
            Tex = new Texture2D(TEX_SIZE,TEX_SIZE);
            var openRects = new List<Rect>();
            openRects.Add(new Rect(0,0,TEX_SIZE,TEX_SIZE));
            foreach (var sprite in sortedSprites)
            {
                var placementResult = TryPlace(sprite, openRects);
                if(!placementResult.Success) throw new ArgumentException("Could not place sprites");  //fail fast for now
                var newRectSprite = new RectSprite()
                {
                    Offset = placementResult.NowFilled.min,
                    //FIXME: Tiling is almost definitely not correct
                    Tiling = new Vector2(TEX_SIZE/  placementResult.NowFilled.min.x,1-(TEX_SIZE/placementResult.NowFilled.min.y))
                };
                
            }
        }

        //WARNING: modifies openrects
        public PlacmentResult TryPlace(Sprite sprite, List<Rect> OpenRects)
        {
            OpenRects.Sort(new RectAreaComparer());
            for (int i = OpenRects.Count - 1; i >= 0; i--)
            {
                var openRect = OpenRects[i];

                if (CanFitInside(sprite.rect, openRect))
                {
                    //no flipping can put inside
                    //look for 2 rects potentially opened - above and to the right, since we're placing on lower left by convention
                    var nowFilled = new Rect(openRect.xMin, openRect.yMin, sprite.rect.width, sprite.rect.width);
                    Rect above = new Rect(nowFilled.x,nowFilled.y+nowFilled.height,openRect.width,openRect.height-nowFilled.height);
                    if (RectHasArea(above))
                    {
                        OpenRects.Add(above);
                        Rect right = new Rect(openRect.x+nowFilled.width,openRect.y,openRect.width-nowFilled.width,openRect.y-above.height);
                        if (RectHasArea(right))
                        {
                            OpenRects.Add(right);
                        }
                    }
                    else
                    {
                        Rect rightNoAbove = new Rect(openRect.x+nowFilled.x,openRect.y,openRect.width-nowFilled.width,openRect.y);
                        OpenRects.Add(rightNoAbove);
                    }

                    OpenRects.Remove(openRect);
                    
                    return new PlacmentResult()
                    {
                        NowFilled = nowFilled,
                        Success = true
                    };   
                }
            }
            return new PlacmentResult(){Success = false};
        }
        private bool RectHasArea(Rect rect)
        {
            return rect.height > Mathf.Epsilon && rect.width > Mathf.Epsilon; //is this the correct epsilon?
        }
        private float RectArea(Rect rect)
        {
            return rect.width * rect.height;
        }

        public bool CanFitInside(Rect toPlace, Rect inThisArea)
        {
            //enforces "no flipping" rule
            if (toPlace.width > inThisArea.width || toPlace.height > inThisArea.height) return false;
            return true;
        }

        public class PlacmentResult
        {
            public bool Success;
            public Rect NowFilled;
        }
        
        public class RectAreaComparer : IComparer<Rect>
        {
            public int Compare(Rect x, Rect y)
            {
                if (x.Equals(y)) return -1; //FIXME:non-deterministic resolution of equal size rects
                var xArea = x.width*x.height;
                var yArea = y.width*y.height;

                if (Mathf.Approximately(xArea, yArea)) return -1; //FIXME:non-deterministic resolution of equal size rects
                
                return xArea < yArea ? -1 : 1;
            }

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
        //removed currently due to circular ref causing serialzation depth to blow up. 
        //public RectAtlasPaged Atlas; //reference atlas - design choice unity makes, cribbing off that  Useful if you want a hidden editor asset controlling concrete sprites
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
