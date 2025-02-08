using MelonLoader;
using UnityEngine;
using RumbleModdingAPI;
using Il2CppRUMBLE.Interactions.InteractionBase;
using Il2CppRUMBLE.Managers;
using System.Collections;
using Il2CppRUMBLE.Environment;
using RumbleModUI;

namespace Mirror
{
    public static class BuildInfo
    {
        public const string ModName = "Mirror";
        public const string ModVersion = "1.1.2";
        public const string Description = "Creates a Mirror for the Dressing Room and Parks";
        public const string Author = "UlvakSkillz";
        public const string Company = "";
    }

    public class MirrorClass : MelonMod
    {
        private string currentScene = "Loader";
        private bool mirrorToggled = false;
        private GameObject ddolMirror, mirror, camera;
        public static Mod Mirror = new Mod();
        private bool doFreeze = false;

        public GameObject LoadAssetBundle(string bundleName, string objectName)
        {
            using (Stream bundleStream = MelonAssembly.Assembly.GetManifestResourceStream(bundleName))
            {
                byte[] bundleBytes = new byte[bundleStream.Length];
                bundleStream.Read(bundleBytes, 0, bundleBytes.Length);
                Il2CppAssetBundle bundle = Il2CppAssetBundleManager.LoadFromMemory(bundleBytes);
                return UnityEngine.Object.Instantiate(bundle.LoadAsset<GameObject>(objectName));
            }
        }

        public override void OnLateInitializeMelon()
        {
            Calls.onMapInitialized += loadMirror;
            ddolMirror = LoadAssetBundle("Mirror.mirror", "Mirror");
            ddolMirror.SetActive(false);
            ddolMirror.name = "Mirror";
            GameObject.DontDestroyOnLoad(ddolMirror);
            Mirror.ModName = "Mirror";
            Mirror.ModVersion = BuildInfo.ModVersion;
            Mirror.SetFolder("Mirror");
            Mirror.AddToList("Freeze Model", false, 0, "Freezes the Player Model while Mirror is Toggled On", new Tags { });
            UI.instance.UI_Initialized += UIInit;
            Mirror.ModSaved += Save;
        }

        private void UIInit()
        {
            UI.instance.AddMod(Mirror);
        }

        private void Save()
        {
            doFreeze = (bool)Mirror.Settings[0].SavedValue;
            Mirror.Settings[0].SavedValue = false;
            Mirror.Settings[0].Value = false;
            if (!doFreeze && (currentScene == "Gym") && mirrorToggled)
            {
                MelonCoroutines.Start(poseModel());
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName;
            mirrorToggled = false;
        }

        private void loadMirror()
        {
            if ((currentScene == "Gym") || (currentScene == "Park"))
            {
                mirror = GameObject.Instantiate(ddolMirror);
                GameObject button = Calls.Create.NewButton();
                button.name = "Mirror Toggle";
                if (currentScene == "Gym")
                {
                    mirror.transform.position = new Vector3(-1.4f, 1f, 0.3f);
                    mirror.transform.rotation = Quaternion.Euler(0f, 223f, 0f);
                    button.transform.position = new Vector3(-1.69f, 1.3f, -2.09f);
                    button.transform.rotation = Quaternion.Euler(90f, 0.6424f, 0f);
                }
                else
                {
                    mirror.transform.position = new Vector3(-13.5155f, -5.1345f, -8.6418f);
                    mirror.transform.rotation = Quaternion.Euler(0f, 62.2366f, 0f);
                    button.transform.position = new Vector3(-12.0155f, -5.2345f, -9.0418f);
                    button.transform.rotation = Quaternion.Euler(90f, 334.3802f, 0f);
                }
                camera = mirror.transform.GetChild(1).gameObject;
                button.transform.GetChild(0).gameObject.GetComponent<InteractionButton>().onPressed.AddListener(new Action(() =>
                {
                    doFreeze = false;
                    mirrorToggled = !mirrorToggled;
                    mirror.SetActive(mirrorToggled);
                    if (currentScene == "Gym")
                    {
                        Calls.GameObjects.Gym.Scene.GymProduction.GetGameObject().transform.GetChild(5).GetChild(3).GetChild(0).GetComponent<Animator>().enabled = !mirrorToggled;
                        if (mirrorToggled)
                        {
                            MelonCoroutines.Start(poseModel());
                        }
                    }
                }));
            }
        }

        private IEnumerator poseModel()
        {
            GameObject modelGO = Calls.GameObjects.Gym.Scene.GymProduction.GetGameObject().transform.GetChild(5).GetChild(3).GetChild(0).GetChild(1).GetChild(0).gameObject;
            GameObject playerGO = PlayerManager.instance.localPlayer.Controller.gameObject.transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
            while (currentScene == "Gym" && mirror.active && !doFreeze)
            {
                modelGO.transform.localRotation = playerGO.transform.localRotation;
                for(int i = 0; i < modelGO.transform.childCount - 1; i++)
                {
                    PoseChildren(modelGO.transform.GetChild(i).gameObject, playerGO.transform.GetChild(i + 1).gameObject);
                }
                yield return new WaitForFixedUpdate();
            }
            doFreeze = false;
            yield break;
        }

        private void PoseChildren(GameObject modelGO, GameObject playerGO)
        {
            modelGO.transform.localRotation = playerGO.transform.localRotation;
            if (modelGO.name != "Bone_Head")
            {
                for (int i = 0; i < modelGO.transform.childCount; i++)
                {
                    try
                    {
                        if (modelGO.transform.GetChild(i).gameObject.name == playerGO.transform.GetChild(i).gameObject.name)
                        {
                            PoseChildren(modelGO.transform.GetChild(i).gameObject, playerGO.transform.GetChild(i).gameObject);
                        }
                        else if (modelGO.transform.GetChild(i).gameObject.name == "Shiftsocket_R")
                        {
                            PoseChildren(modelGO.transform.GetChild(i).gameObject, playerGO.transform.GetChild(i + 1).gameObject);
                        }
                    }
                    catch { }
                }
            }
        }

        public override void OnFixedUpdate()
        {
            if (mirrorToggled)
            {
                Vector3 localPlayer = mirror.transform.InverseTransformPoint(PlayerManager.instance.localPlayer.Controller.gameObject.transform.GetChild(1).GetChild(0).GetChild(0).position);
                camera.transform.position = mirror.transform.TransformPoint(new Vector3(localPlayer.x, localPlayer.y, -localPlayer.z));
                Vector3 lookAtMirror = mirror.transform.TransformPoint(new Vector3(-localPlayer.x, localPlayer.y, localPlayer.z));
                camera.transform.LookAt(lookAtMirror);
            }
        }
    }
}
