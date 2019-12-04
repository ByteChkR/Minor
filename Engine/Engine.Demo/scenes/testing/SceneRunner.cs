using System;
using System.Collections.Generic;
using System.Reflection;
using Engine.Core;

namespace Engine.Demo.scenes.testing
{
    public class SceneRunner
    {
        public static SceneRunner Instance;
        private int currrentScene;
        private GameEngine engine;
        private List<Type> scenes = new List<Type>();
        private bool terminateOnFinish = true;

        public SceneRunner(GameEngine engine, Assembly asm, string nameSpace)
        {
            this.engine = engine;
            Instance = this;
            scenes = GetScenesFromNamespace(asm, nameSpace);
        }

        public SceneRunner(GameEngine engine, Assembly asm, string[] nameSpaces)
        {
            this.engine = engine;
            Instance = this;
            for (int i = 0; i < nameSpaces.Length; i++)
            {
                scenes.AddRange(GetScenesFromNamespace(asm, nameSpaces[i]));
            }
        }

        public bool Finished { get; private set; }

        private List<Type> GetScenesFromNamespace(Assembly asm, string nameSpace)
        {
            List<Type> ret = new List<Type>();
            Type[] ts = asm.GetTypes();
            Type sceneType = typeof(AbstractScene);
            for (int i = 0; i < ts.Length; i++)
            {
                if (sceneType.IsAssignableFrom(ts[i]) && ts[i].Namespace.StartsWith(nameSpace))
                {
                    ret.Add(ts[i]);
                }
            }

            return ret;
        }

        public void NextScene()
        {
            if (currrentScene >= scenes.Count)
            {
                currrentScene = 0;
                Finished = true;
                if (terminateOnFinish)
                {
                    engine.Exit();
                    return;
                }
            }

            engine.InitializeScene(scenes[currrentScene]);

            currrentScene++;
        }
    }
}