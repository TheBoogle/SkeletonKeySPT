using BepInEx;
using System;

namespace YourNamespace
{
    [BepInPlugin("com.yourname.removekeyid", "Remove KeyID Requirement", "1.0.0")]
    public class RemoveKeyIDPlugin : BaseUnityPlugin
    {
        public void Awake()
        {
            Logger.LogInfo("Loading: KeycardDoor Patch");

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

            Logger.LogInfo("Completed: KeycardDoor Patch");
        }

    }
}
