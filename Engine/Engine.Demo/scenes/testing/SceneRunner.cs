using System;
using System.Collections.Generic;
using System.Reflection;
using Engine.Core;

namespace Engine.Demo.scenes.testing
{
    public class SceneRunner
    {
        public static SceneRunner Instance;
        private int CurrrentScene;
        private GameEngine Engine;
        private List<Type> Scenes = new List<Type>();
        private bool TerminateOnFinish = true;

        public SceneRunner(GameEngine engine, Assembly asm, string nameSpace)
        {
            Engine = engine;
            Instance = this;
            Scenes = GetScenesFromNamespace(asm, nameSpace);
        }

        public SceneRunner(GameEngine engine, Assembly asm, string[] nameSpaces)
        {
            Engine = engine;
            Instance = this;
            for (int i = 0; i < nameSpaces.Length; i++)
            {
                Scenes.AddRange(GetScenesFromNamespace(asm, nameSpaces[i]));
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
            if (CurrrentScene >= Scenes.Count)
            {
                CurrrentScene = 0;
                Finished = true;
                if (TerminateOnFinish)
                {
                    Engine.Exit();
                    return;
                }
            }

            Engine.InitializeScene(Scenes[CurrrentScene]);
            CurrrentScene++;
        }
    }
}