using System.Reflection;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;

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
            ref GStruct457<GClass3424> __result,
            KeyComponent key,
            Player player,
            WorldInteractiveObject __instance)
        {
            var canInteract = player.MovementContext.CanInteract;
            
            if (canInteract != null)
            {
                __result = canInteract;
            }
            if (!(key.Template.KeyId == __instance.KeyId || key.Template.KeyId == "673e1f10aaf0fe810c488218"))
            {
                __result = new GClass3854("Key doesn't match");
            }
            
            var gstruct = default(GStruct455<GClass3200>);
            
            key.NumberOfUsages++;
            if (key.NumberOfUsages >= key.Template.MaximumNumberOfUsage && key.Template.MaximumNumberOfUsage > 0)
            {
                gstruct = InteractionsHandlerClass.Discard(key.Item, (TraderControllerClass)key.Item.Parent.GetOwner(), false);
                if (gstruct.Failed)
                {
                    __result = gstruct.Error;
                }
            }
            __result = new GClass3424(key, gstruct.Value, true);

            return false;
        }
    }
}
