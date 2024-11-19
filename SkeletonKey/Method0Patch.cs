using System.Reflection;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;
using static EFT.PlayerOwner;

namespace YourNamespace
{
    public class Method0Patch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            // Target the method_0 function in Class1544
            return typeof(Class1544)
                .GetMethod("method_0", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPrefix]
        private static bool PatchPrefix(KeyComponent x, Class1544 __instance, ref bool __result)
        {
            // Check if the key has the correct KeyId or is "BoogleSkeletonKey"
            if (x.Template.KeyId == __instance.worldInteractiveObject.KeyId || x.Template.KeyId == "BoogleSkeletonKey")
            {
                __result = true; // Allow unlock
                return false;    // Skip the original method
            }

            // Otherwise, let the original logic handle it
            return true;
        }
    }
}
