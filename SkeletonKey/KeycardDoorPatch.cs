using System.Reflection;
using SPT.Reflection.Patching;
using EFT.Interactive;
using EFT.InventoryLogic;
using Diz.LanguageExtensions;
using EFT;

namespace YourNamespace
{
    public class KeycardDoorUnlockPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            // Target the UnlockOperation method in KeycardDoor
            return typeof(KeycardDoor).GetMethod("UnlockOperation", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPrefix]
        private static bool PatchPrefix(
            ref GStruct416<KeyInteractionResultClass> __result,
            KeyComponent key,
            Player player,
            KeycardDoor __instance)
        {
            // Check if the player can interact
            Error canInteract = player.MovementContext.CanInteract;
            if (canInteract != null)
            {
                __result = canInteract;
                return false;
            }

            // Dynamically allow the "BoogleSkeletonKeycard"
            bool isAuthorized = key.Template.KeyId == __instance.KeyId || key.Template.KeyId == "BoogleSkeletonKeycard";

            if (!isAuthorized)
            {
                __result = new KeyInteractionResultClass(key, null, false);
                return false;
            }

            // Simulate usage logic
            key.NumberOfUsages++;
            if (key.NumberOfUsages >= key.Template.MaximumNumberOfUsage && key.Template.MaximumNumberOfUsage > 0)
            {
                var discardResult = InteractionsHandlerClass.Discard(
                    key.Item,
                    (TraderControllerClass)key.Item.Parent.GetOwner(),
                    false,
                    false
                );

                if (discardResult.Failed)
                {
                    __result = discardResult.Error;
                    return false;
                }
            }

            // Return successful interaction
            __result = new KeyInteractionResultClass(key, null, true);
            return false;
        }
    }
}
