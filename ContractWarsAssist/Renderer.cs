using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ContractWarsAssist
{
    class GuiRenderer
    {
        private static Texture2D aaLineTex = null;
        private static Texture2D lineTex = null;
        private static Material blitMaterial = null;
        private static Material blendMaterial = null;
        private static Rect lineRect = new Rect(0f, 0f, 1f, 1f);

        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
        {
            if (lineTex == null)
            {
                lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                lineTex.SetPixel(0, 0, UnityEngine.Color.white);
                lineTex.Apply();
            }
            if (aaLineTex == null)
            {
                aaLineTex = new Texture2D(1, 3, TextureFormat.ARGB32, false);
                aaLineTex.SetPixel(0, 0, new UnityEngine.Color(1, 1, 1, 0));
                aaLineTex.SetPixel(0, 1, UnityEngine.Color.white);
                aaLineTex.SetPixel(0, 2, new UnityEngine.Color(1, 1, 1, 0));
                aaLineTex.Apply();
            }

            blitMaterial = (Material)typeof(GUI).GetMethod("get_blitMaterial", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
            blendMaterial = (Material)typeof(GUI).GetMethod("get_blendMaterial", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);

            float num = pointB.x - pointA.x;
            float num2 = pointB.y - pointA.y;
            float num3 = Mathf.Sqrt(num * num + num2 * num2);
            if (num3 < 0.001f)
            {
                return;
            }
            Texture2D texture2D;
            if (antiAlias)
            {
                width *= 3f;
                texture2D = aaLineTex;
            }
            else
            {
                texture2D = lineTex;
            }
            float num4 = width * num2 / num3;
            float num5 = width * num / num3;
            Matrix4x4 identity = Matrix4x4.identity;
            identity.m00 = num;
            identity.m01 = -num4;
            identity.m03 = pointA.x + 0.5f * num4;
            identity.m10 = num2;
            identity.m11 = num5;
            identity.m13 = pointA.y - 0.5f * num5;
            GL.PushMatrix();
            GL.MultMatrix(identity);
            GUI.color = color;
            GUI.DrawTexture(lineRect, texture2D);
            GL.PopMatrix();
        }

        public static void RectFilled(float x, float y, float width, float height, Texture2D text)
        {
            GUI.DrawTexture(new Rect(x, y, width, height), text);
        }

        public static void RectOutlined(float x, float y, float width, float height, Texture2D text, float thickness = 1f)
        {
            RectFilled(x, y, thickness, height, text);
            RectFilled(x + width - thickness, y, thickness, height, text);
            RectFilled(x + thickness, y, width - thickness * 2f, thickness, text);
            RectFilled(x + thickness, y + height - thickness, width - thickness * 2f, thickness, text);
        }

        public static void Box(float x, float y, float width, float height, Texture2D text, float thickness = 1f)
        {
            RectOutlined(x - width / 2f, Screen.height - y - height, width, height, text, thickness);
        }
    }
}
