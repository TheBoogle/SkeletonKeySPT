using BepInEx;
using BepInEx.Logging;
using System;

namespace Boogle
{
    [BepInPlugin("com.boogle.skeletonkey", "Adds the Skeleton Key Item", "1.0.6")]
    public class Skeletonkey : BaseUnityPlugin
    {
        public static string keyID = "673e1f10aaf0fe810c488218";
        public static string keycardID = "673e213fc6be39d06423d6b7";

        public static ManualLogSource Log { get; private set; }
        public void Awake()
        {
            Log = Logger;

            try
            {
                new KeycardDoorUnlockPatch().Enable();
                new PlayerOwnerGetKeyPatch().Enable();
                new WorldInteractiveObjectUnlockPatch().Enable();
            }
            catch (Exception ex)
            {
                Logger.LogError($"A PATCH IN {GetType().Name} FAILED. SUBSEQUENT PATCHES HAVE NOT LOADED");
                Logger.LogError($"{GetType().Name}: {ex}");
                throw;
            }
        }

    }
}
