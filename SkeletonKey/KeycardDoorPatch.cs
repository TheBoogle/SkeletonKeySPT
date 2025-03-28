﻿using System.Reflection;
using SPT.Reflection.Patching;
using EFT.Interactive;
using EFT.InventoryLogic;
using Diz.LanguageExtensions;
using EFT;

namespace Boogle
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
            ref GStruct457<GClass3424> __result,
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
            bool isAuthorized = key.Template.KeyId == __instance.KeyId || key.Template.KeyId == "673e213fc6be39d06423d6b7";
            if (!isAuthorized)
            {
                __result = new GClass3424(key, null, false);
                return false;
            }

            // Simulate usage logic
            key.NumberOfUsages++;
            if (key.NumberOfUsages >= key.Template.MaximumNumberOfUsage && key.Template.MaximumNumberOfUsage > 0)
            {
                var discardResult = InteractionsHandlerClass.Discard(
                    key.Item,
                    (TraderControllerClass)key.Item.Parent.GetOwner(),
                    false
                );

                if (discardResult.Failed)
                {
                    __result = discardResult.Error;
                    return false;
                }
                __result = new GClass3424(key, discardResult.Value, true);
                return false;
            }
            __result = new GClass3424(key, null, true);
            return false;
        }
    }
}
