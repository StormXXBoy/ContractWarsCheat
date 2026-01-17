using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ContractWarsAssist
{
    internal class Hacks : MonoBehaviour
    {
        bool espEnabled = false;

        [DllImport("user32.dll")]
        static extern void mouse_event(
            uint dwFlags,
            uint dx,
            uint dy,
            uint dwData,
            UIntPtr dwExtraInfo
        );

        public const uint MOUSEEVENTF_MOVE = 0x0001;
        public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        public void MoveMouseTo(Vector2 target)
        {
            Vector2 mousePos = new Vector2(Screen.width / 2, Screen.height / 2);
            double DistX = target.x - mousePos.x;
            double DistY = target.y - mousePos.y;

            // Smooth factor
            DistX /= 4;
            DistY /= 4;

            // Optional clamp
            DistX = Mathf.Clamp((float)DistX, -20f, 20f);
            DistY = Mathf.Clamp((float)DistY, -20f, 20f);

            // Relative mouse move
            mouse_event(MOUSEEVENTF_MOVE, (uint)DistX, (uint)DistY, 0, UIntPtr.Zero);
        }

        private bool showMenu = true;
        private Rect menuRect = new Rect(20, 20, 250, 300);

        public static HashSet<GameObject> players = new HashSet<GameObject>();
        public static GameObject player = null;

        Vector2 closestPoint = Vector2.zero;

        void OnGUI()
        {
            float minDistance = float.MaxValue;

            if (showMenu) menuRect = GUI.Window(0, menuRect, new GUI.WindowFunction(DrawMenu), "Testing");

            try
            {
                closestPoint = Vector2.zero;
                Vector2 mousePoint = Event.current.mousePosition;

                foreach (GameObject gameObj in players)
                {
                    //if (getTeam(gameObj) == getTeam(player)) { continue; }
                    if (gameObj == player) continue;
                    Transform[] allChildren = gameObj.transform.GetComponentsInChildren<Transform>();
                    foreach (Transform child in allChildren)
                    {
                        if (child.name == "NPC_Head") //NPC_Spine1 "NPC_Neck"
                        {
                            Vector3 feetPoint = Camera.main.WorldToScreenPoint(gameObj.transform.position);
                            Vector3 headPoint = Camera.main.WorldToScreenPoint(child.position);
                            var distance = Vector3.Distance(Camera.main.transform.position, gameObj.transform.position);

                            if (feetPoint.z > -1)
                            {
                                float distBetweenHeadAndFeet = Mathf.Abs(feetPoint.y - headPoint.y);

                                Vector2 screenPos = new Vector2(headPoint.x, Screen.height - headPoint.y); // flip Y

                                float currentDistance = Vector2.Distance(screenPos, new Vector2(Screen.width/2, Screen.height/2));
                                if (currentDistance < minDistance)
                                {
                                    minDistance = currentDistance;
                                    closestPoint = screenPos;
                                }

                                if (!espEnabled) continue;
                                GuiRenderer.DrawLine(new Vector2(Screen.width / 2, Screen.height), new Vector2(headPoint.x, Screen.height - headPoint.y), Color.red, 2f, true);
                                GuiRenderer.Box(feetPoint.x, feetPoint.y, distBetweenHeadAndFeet / 1.8f, distBetweenHeadAndFeet, Textures.White);
                                GUI.Label(new Rect(feetPoint.x - 50, Screen.height - feetPoint.y + 20, 100, 20), getTeam(gameObj) + " " + distance.ToString("F1") + "m");
                            }
                        }
                    }
                }
                if (closestPoint != Vector2.zero & espEnabled)
                {
                    GuiRenderer.DrawLine(new Vector2(Screen.width / 2, Screen.height), closestPoint, Color.green, 2f, true);
                }
            }
            catch { }
        }

        public string holdingE = "No";

        void Update()
        {
            UpdatePlayerList();

            if (Input.GetKeyDown(KeyCode.Insert))
                showMenu = !showMenu;

            try
            {
                if (Input.GetKey(KeyCode.E))
                {
                    MoveMouseTo(closestPoint);
                    holdingE = "Yes";
                }
                else { holdingE = "No"; }
            } catch {}
        }

        void UpdatePlayerList()
        {
            try
            {
                foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
                {
                    if ((go.name.Contains("client_bear") || go.name.Contains("client_usec")) && !players.Contains(go))
                    {
                        var distance = Vector3.Distance(Camera.main.transform.position, go.transform.position);
                        if (IsFakePlayer(go))
                        {
                            GameObject.Destroy(go);
                        }
                        else
                        {
                            if (distance < 10)
                            {
                                player = go;
                            }
                            else
                            {
                                players.Add(go);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        bool IsFakePlayer(GameObject playerObj)
        {
            // Check all renderers in the object and its children
            Renderer[] renderers = playerObj.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                foreach (Material material in renderer.sharedMaterials)
                {
                    if (material.name.Contains("client_fake"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        string getTeam(GameObject gameObj)
        {
            if (gameObj.name.Contains("client_bear"))
                return "Bear";
            if (gameObj.name.Contains("client_usec"))
                return "Usec";
            return "Unknown";
        }

        enum MenuAction
        {
            EnableESP,
            FlushPLayers,
        }

        void DrawMenu(int id)
        {
            float elementHeight = 20f;
            float elementCount = 0f;

            void AddButton(string label, MenuAction action)
            {
                float x = 0f;
                float y = elementCount * elementHeight;
                float width = menuRect.width;
                float height = elementHeight;

                if (GUI.Button(new Rect(x, 20f + y, width, height), label))
                    HandleMenuAction(action);

                elementCount++;
            }

            void AddLabel(string label)
            {
                float x = 0f;
                float y = elementCount * elementHeight;
                float width = menuRect.width;
                float height = elementHeight;

                GUI.Label(new Rect(x, 20f + y, width, height), label);

                elementCount++;
            }

            void HandleMenuAction(MenuAction action)
            {
                switch (action)
                {
                    case MenuAction.EnableESP:
                        espEnabled = !espEnabled;
                        break;
                    case MenuAction.FlushPLayers:
                        players.Clear();
                        break;
                }
            }

            //Example buttons
            AddButton("Toggle ESP: " + espEnabled.ToString(), MenuAction.EnableESP);
            AddButton("Flush Players", MenuAction.FlushPLayers);
            AddLabel("PLRS: " + players.Count);
            AddLabel("E for aimbot: " + Input.GetKeyDown(KeyCode.E).ToString());

            GUI.DragWindow();
        }
    }
}