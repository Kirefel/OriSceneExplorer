using HarmonyLib;
using UnityEngine;

namespace OriSceneExplorer
{
    [HarmonyPatch(typeof(TitleScreenPressStartLogic), nameof(TitleScreenPressStartLogic.OnStartPressedCallback))]
    public class OnPressStart
    {
        public static bool Prefix()
        {
            if (!Editor.ready)
            {
                GameObject go = new GameObject("UnityViewer");
                go.AddComponent<Editor>();
                Editor.ready = true;
            }

            return true;
        }
    }
}