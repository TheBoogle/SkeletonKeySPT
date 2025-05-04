using EFT.InventoryLogic;
using SPT.Reflection.Patching;
using System.Reflection;
using static EFT.PlayerOwner;

namespace Boogle
{
    public class PlayerOwnerGetKeyPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            // Target the compiler generated method_0 method in PlayerOwner.Class1674
            return typeof(Class1674).GetMethod("method_0", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPrefix]
        private static bool PatchPrefix(Class1674 __instance, KeyComponent x, ref bool __result)
        {
            __result = x.Template.KeyId == __instance.worldInteractiveObject.KeyId
                || x.Template.KeyId == Skeletonkey.keyID;
            return false; // Skip original method
        }
    }
}
