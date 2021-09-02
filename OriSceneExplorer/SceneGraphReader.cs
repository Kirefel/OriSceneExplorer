using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OriSceneExplorer
{
    public class SceneGraphReader
    {
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

        private ViewerGORef BuildGORef(GameObject go, int depth)
        {
            var children = new List<ViewerGORef>();
            foreach (var tf in go.transform)
                children.Add(BuildGORef(((Transform)tf).gameObject, depth + 1));

            var goref = new ViewerGORef()
            {
                Name = go.name,
                Children = children,
                Depth = depth,
                Reference = go
            };

            return goref;
        }
    }
}