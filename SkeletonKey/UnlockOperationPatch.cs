using System.Reflection;
using SPT.Reflection.Patching;
using EFT.Interactive;
using EFT;
using EFT.InventoryLogic;
using Diz.LanguageExtensions;

namespace YourNamespace
{
    public class UnlockOperationPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            // Target the UnlockOperation method in WorldInteractiveObject
            return typeof(WorldInteractiveObject)
                .GetMethod("UnlockOperation", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPrefix]
        private static bool PatchPrefix(
            ref GStruct416<KeyInteractionResultClass> __result,
            KeyComponent key,
            Player player,
            WorldInteractiveObject __instance)
        {
            Error canInteract = player.MovementContext.CanInteract;
            if (canInteract != null)
            {
                __result = canInteract;
            }
            if (!(key.Template.KeyId == __instance.KeyId || key.Template.KeyId == "BoogleSkeletonKey"))
            {
                __result = new GClass3370("Key doesn't match");
            }
            GStruct414<GClass2799> gstruct = default(GStruct414<GClass2799>);
            key.NumberOfUsages++;
            if (key.NumberOfUsages >= key.Template.MaximumNumberOfUsage && key.Template.MaximumNumberOfUsage > 0)
            {
                gstruct = InteractionsHandlerClass.Discard(key.Item, (TraderControllerClass)key.Item.Parent.GetOwner(), false, false);
                if (gstruct.Failed)
                {
                    __result = gstruct.Error;
                }
            }
            __result = new KeyInteractionResultClass(key, gstruct.Value, true);

            return false;
        }
    }
}
