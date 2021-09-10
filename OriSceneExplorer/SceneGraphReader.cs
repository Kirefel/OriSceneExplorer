using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OriSceneExplorer
{
    public class SceneGraphReader
    {
        private GameObject target;
        public ViewerGORef TargetGoRef { get; private set; } = null;

        public SceneGraphReader(GameObject target = null)
        {
            this.target = target;
        }

        public List<ViewerGORef> GetAllGameObjectReferences()
        {
            var allRefs = new List<ViewerGORef>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var sref = new ViewerGORef()
                {
                    Depth = 0,
                    IsScene = true,
                    Name = scene.name
                };

                var gameObjects = scene.GetRootGameObjects();

                foreach (var go in gameObjects)
                    sref.Children.Add(BuildGORef(go, 1));

                sref.Expanded = sref.Children.Any(c => c.Expanded);

                allRefs.Add(sref);
            }

            return allRefs;
        }

        public List<ViewerGORef> GetAllActiveRendererComponents()
        {
            return GameObject.FindObjectsOfType<Renderer>()
                .Where(r => r.gameObject.activeInHierarchy)
                .Select(r => new ViewerGORef()
                {
                    Reference = r.gameObject,
                    Name = r.name
                })
                .ToList();
        }

        Regex actionSequenceRegex = new Regex("^\\d\\d\\.");

        private ViewerGORef BuildGORef(GameObject go, int depth)
        {
            var children = new List<ViewerGORef>();
            foreach (var tf in go.transform)
                children.Add(BuildGORef(((Transform)tf).gameObject, depth + 1));

            if (children.Any(c => actionSequenceRegex.IsMatch(c.Name)))
                children.Sort((a, b) => a.Name.CompareTo(b.Name));

            var goref = new ViewerGORef()
            {
                Name = go.name,
                Children = children,
                Depth = depth,
                Reference = go,
                Expanded = go == target || children.Any(c => c.Expanded)
            };

            if (go == target)
                TargetGoRef = goref;

            return goref;
        }
    }
}