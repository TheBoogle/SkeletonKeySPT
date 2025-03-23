using EFT.Interactive;
using EFT;
using SPT.Reflection.Patching;
using System;
using System.Linq;
using System.Reflection;
using EFT.InventoryLogic;

namespace Boogle
{
    internal class DoorActionMenuPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() => typeof(GetActionsClass).GetMethod(nameof(GetActionsClass.smethod_13));

        [PatchPostfix]
        public static void Postfix(ref ActionsReturnClass __result, GamePlayerOwner owner, Door door)
        {
            if (door.DoorState != EDoorState.Locked)
            {
                return;
            }

            GetActionsClass.Class1610 doorUnlockAction = new GetActionsClass.Class1610
            {
                owner = owner,
                worldInteractiveObject = door as WorldInteractiveObject
            };

            if (__result != null && __result.Actions != null)
            {
                // Reducing menu clutter by only showing the "Use Skeleton Key" option if you: Have a skeleton key, and Don't have the original key (Prevent accidental usage)
                if (!HasKey(owner.Player, door.KeyId) && HasKey(owner.Player, "673e1f10aaf0fe810c488218"))
                {
                    __result.Actions.Add(new ActionsTypesClass
                    {
                        Name = "Use Skeleton Key",

                        Action = new Action(() =>
                        {
                            // Backing up the original door's KeyId
                            var oldKeyId = door.KeyId;

                            // Setting the door's KeyId to the Skeleton Key, this way all future functions pass the checks
                            door.KeyId = "673e1f10aaf0fe810c488218";

                            // Populate this.GetActionsClass.Class610.key, needed when running method_0()
                            doorUnlockAction.key = owner.GetKey(door);

                            // Run the same vanilla method used when selecting "Unlock"
                            doorUnlockAction.method_0();

                            // Restore the old KeyId (Likely unnecessary, the GameObject instance for the door is destroyed after raid)
                            door.KeyId = oldKeyId;
                        }),
                        Disabled = !doorUnlockAction.worldInteractiveObject.Operatable
                    });
                }
            }
        }

        static internal bool HasKey(Player player, string KeyId)
        {
            try
            {
                var foundItem = player.Inventory.GetPlayerItems(EPlayerItems.Equipment).FirstOrDefault(x => x.TemplateId == KeyId);
                if (foundItem != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
