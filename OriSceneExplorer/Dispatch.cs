using System;
using System.Collections.Generic;

namespace OriSceneExplorer
{
    // Hacky hack for https://answers.unity.com/questions/400454/argumentexception-getting-control-0s-position-in-a-1.html
    public static class Dispatch
    {
        private static readonly List<Action> actions = new List<Action>();
        public static void Queue(Action action) => actions.Add(action);
        public static void Execute()
        {
            foreach (var action in actions)
                action();
            actions.Clear();
        }
    }
}