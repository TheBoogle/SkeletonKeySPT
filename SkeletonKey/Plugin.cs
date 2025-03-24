using BepInEx;
using BepInEx.Logging;
using System;

namespace Boogle
{
    [BepInPlugin("com.boogle.skeletonkey", "Adds the Skeleton Key Item", "1.0.5")]
    public class skeletonkey : BaseUnityPlugin
    {
        public static ManualLogSource Log {  get; private set; }
        public void Awake()
        {
            Log = Logger;

            try
            {
                new DoorActionMenuPatch().Enable();
                new KeycardDoorUnlockPatch().Enable();
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
