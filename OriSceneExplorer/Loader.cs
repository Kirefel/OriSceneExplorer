using HarmonyLib;
using System;

namespace OriSceneExplorer
{
    public static class Loader
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (args.LoadedAssembly.GetName().Name == "Assembly-CSharp")
            {
                Patch();
            }
        }

        private static void Patch()
        {
            var harmony = new Harmony("com.ori.sceneexplorer");
            harmony.PatchAll();
        }
    }
}