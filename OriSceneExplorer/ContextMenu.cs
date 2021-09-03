using System;
using UnityEngine;

namespace OriSceneExplorer
{
    public class ContextMenu
    {
        public delegate void ContextMenuCallback(int item);
        private static ContextMenuCallback callback;
        private static string[] contextItems;
        private static bool open;
        public static bool Open => open;
        private static Rect bounds;

        private const float width = 250;
        private const float itemHeight = 24;

        public static void OnGUI()
        {
            if (!open)
                return;

            GUI.depth = 1;

            for (int i = 0; i < contextItems.Length; i++)
            {
                if (GUI.Button(new Rect(bounds.x, bounds.y + i * itemHeight, width, itemHeight), contextItems[i]))
                {
                    callback(i);
                    Close();
                }
            }
        }

        private static void Close()
        {
            open = false;
            callback = null;
        }

        public static void Show(ContextMenuCallback callback, params string[] options)
        {
            if (options.Length == 0)
                throw new ArgumentException("options cannot be empty");

            contextItems = options;
            open = true;
            ContextMenu.callback = callback;

            bounds = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, width, itemHeight * options.Length);
            //if (bounds.xMax > Screen.width)
            //    bounds.x -= bounds.width; 
            //if (bounds.yMax > Screen.height)
            //    bounds.y -= bounds.height;
        }

        public static void Update()
        {
            if (!open)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                callback(-1);
                Close();
            }
        }
    }
}