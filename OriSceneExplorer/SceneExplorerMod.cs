using HarmonyLib;
using OriDeModLoader;

namespace OriSceneExplorer
{
    public class SceneExplorerMod : IMod
    {
        public string Name => "Scene Explorer";

        public void Init()
        {
            var harmony = new Harmony("com.ori.sceneexplorer");
            harmony.PatchAll();
        }

        public void Unload() { }
    }
}