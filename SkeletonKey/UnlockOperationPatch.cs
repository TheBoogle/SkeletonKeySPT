using System.Reflection;
using SPT.Reflection.Patching;
using EFT.Interactive;
using EFT;
using EFT.InventoryLogic;
using Diz.LanguageExtensions;

namespace Boogle
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
            ref GStruct448<GClass3344> __result,
            KeyComponent key,
            Player player,
            WorldInteractiveObject __instance)
        {
            Error canInteract = player.MovementContext.CanInteract;
            if (canInteract != null)
            {
                __result = canInteract;
            }
            if (!(key.Template.KeyId == __instance.KeyId || key.Template.KeyId == "673e1f10aaf0fe810c488218"))
            {
                __result = new GClass3757("Key doesn't match");
            }
            GStruct446<GClass3129> gstruct = default(GStruct446<GClass3129>);
            key.NumberOfUsages++;
            if (key.NumberOfUsages >= key.Template.MaximumNumberOfUsage && key.Template.MaximumNumberOfUsage > 0)
            {
                gstruct = InteractionsHandlerClass.Discard(key.Item, (TraderControllerClass)key.Item.Parent.GetOwner(), false);
                if (gstruct.Failed)
                {
                    __result = gstruct.Error;
                }
            }
            __result = new GClass3344(key, gstruct.Value, true);

            return false;
        }
    }
}
