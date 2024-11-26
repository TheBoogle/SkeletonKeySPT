using BepInEx;
using System;

namespace Boogle
{
    [BepInPlugin("com.boogle.skeletonkey", "Adds the Skeleton Key Item", "1.0.3")]
    public class skeletonkey : BaseUnityPlugin
    {
        public void Awake()
        {
            try
            {
                new UnlockOperationPatch().Enable();
                new Method0Patch().Enable();
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
