using EFT.Interactive;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Boogle
{
    public class KeycardDoorUnlockPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            // Target the UnlockOperation method in KeycardDoor
            return AccessTools.Method(typeof(KeycardDoor), "UnlockOperation");
        }

        [PatchTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // See "KeyTemplateClass" class to update GInterface.
            FieldInfo templateField = AccessTools.Field(typeof(KeyComponent), "Template");
            MethodInfo getKeyIdMethod = AccessTools.Method(typeof(GInterface368), "get_KeyId");
            FieldInfo keyIdField = AccessTools.Field(typeof(WorldInteractiveObject), "KeyId");
            MethodInfo opEqualityMethod = AccessTools.Method(typeof(string), "op_Equality", new[] { typeof(string), typeof(string) });

            if (templateField == null || getKeyIdMethod == null || keyIdField == null || opEqualityMethod == null)
            {
                Logger.LogError("One or more required fields/methods not found!");
                return instructions;
            }

            // This will try match the code "key.Template.KeyId == this.KeyId".
            CodeMatcher matcher = new CodeMatcher(instructions).MatchForward(true,
                new CodeMatch(OpCodes.Ldarg_1),
                new CodeMatch(OpCodes.Ldfld, templateField), // Getting "key.Template".
                new CodeMatch(OpCodes.Callvirt, getKeyIdMethod), // Calling "key.Template.get_KeyId()".
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, keyIdField), // Getting "this.KeyId".
                new CodeMatch(OpCodes.Call, opEqualityMethod), // Comparing "key.Template.KeyId" and "this.KeyId".
                new CodeMatch(OpCodes.Brtrue));

            if (matcher.IsValid)
            {
                object successLabel = matcher.Operand; // Get last IL code (Brtrue) operand, this will pass the if check.
                if (successLabel is Label)
                {
                    matcher.Advance(1); // Move 1 step to insert our IL code. Use -6 to check skeleton key first, then the original key.

                    // This will insert code "key.Template.KeyId == Skeletonkey.keycardID".
                    matcher.Insert(
                        new CodeInstruction(OpCodes.Ldarg_1), // Push "key" paramater
                        new CodeInstruction(OpCodes.Ldfld, templateField), // Read "key.Template"
                        new CodeInstruction(OpCodes.Callvirt, getKeyIdMethod), // Call "key.Template.get_KeyId()"
                        new CodeInstruction(OpCodes.Ldstr, Skeletonkey.keycardID), // Push the skeleton keycard ID
                        new CodeInstruction(OpCodes.Call, opEqualityMethod), // Compare "key.Template.KeyId" and "Skeletonkey.keycardID"
                        new CodeInstruction(OpCodes.Brtrue, successLabel)); // If true, branch to the original success label
                }
                else
                {
                    Logger.LogError("Getting the success label failed in KeycardDoor.UnlockOperation()!");
                    return instructions;
                }
            }
            else
            {
                Logger.LogError("Failed to find target IL instructions in KeycardDoor.UnlockOperation()!");
                return instructions;
            }

            // Final code (3.11.x - 35392)
            // bool flag = key.Template.KeyId == this.KeyId
            // || key.Template.KeyId == "673e213fc6be39d06423d6b7"
            // || (this._additionalKeys != null && this._additionalKeys.Contains(key.Template.KeyId));
            return matcher.InstructionEnumeration();
        }
    }
}
