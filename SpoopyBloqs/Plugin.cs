using IPA;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;
using Harmony;
using System.Reflection;

namespace SpoopyBloqs
{
    public class Plugin : IBeatSaberPlugin
    {
        public static bool IsEnabled = false;
        internal static HarmonyInstance harmony;

        public void Init(IPALogger logger)
        {
            Logger.logger = logger;
        }

        public void OnApplicationStart()
        {
            harmony = HarmonyInstance.Create("com.steven.happy.halloween");
        }

        public void OnApplicationQuit() { }

        public void OnFixedUpdate() { }

        public void OnUpdate() { }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "GameCore" && IsEnabled)
            {
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Spooper.Init();
            }
            else
            {
                if (harmony.HasAnyPatches("com.steven.happy.halloween"))
                    harmony.UnpatchAll("com.steven.happy.halloween");
            }
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (scene.name == "MenuCore")
                SpoopyUI.CreateUI();
        }

        public void OnSceneUnloaded(Scene scene) { }
    }
}
