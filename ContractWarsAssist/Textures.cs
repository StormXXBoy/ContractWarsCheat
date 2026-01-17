using UnityEngine;

namespace ContractWarsAssist
{
    public static class Textures
    {
        private static Texture2D whiteTex;
        private static Texture2D blackTex;
        private static Texture2D backgroundTex;

        public static Texture2D White
        {
            get
            {
                if (whiteTex == null)
                {
                    whiteTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    whiteTex.SetPixel(0, 0, Color.white);
                    whiteTex.Apply();
                    whiteTex.hideFlags = HideFlags.HideAndDontSave;
                }
                return whiteTex;
            }
        }

        public static Texture2D Black
        {
            get
            {
                if (blackTex == null)
                {
                    blackTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    blackTex.SetPixel(0, 0, Color.black);
                    blackTex.Apply();
                    blackTex.hideFlags = HideFlags.HideAndDontSave;
                }
                return blackTex;
            }
        }

        public static Texture2D Background
        {
            get
            {
                if (backgroundTex == null)
                {
                    Color BackGroundColor = new Color32(13, 13, 13, 255);
                    backgroundTex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                    backgroundTex.SetPixel(0, 0, BackGroundColor);
                    backgroundTex.SetPixel(1, 0, BackGroundColor);
                    backgroundTex.SetPixel(0, 1, BackGroundColor);
                    backgroundTex.SetPixel(1, 1, BackGroundColor);
                    backgroundTex.Apply();
                }
                return backgroundTex;
            }
        }

        public static void Cleanup()
        {
            if (whiteTex != null)
            {
                Object.DestroyImmediate(whiteTex);
                whiteTex = null;
            }

            if (backgroundTex != null)
            {
                Object.DestroyImmediate(backgroundTex);
                backgroundTex = null;
            }
        }
    }
}