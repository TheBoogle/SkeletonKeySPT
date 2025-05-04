using EFT.Interactive;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Boogle
{
    public class WorldInteractiveObjectUnlockPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            // Target the UnlockOperation method in WorldInteractiveObject
            return AccessTools.Method(typeof(WorldInteractiveObject), "UnlockOperation");
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

                    // This will insert code "key.Template.KeyId == Skeletonkey.keyID".
                    matcher.Insert(
                        new CodeInstruction(OpCodes.Ldarg_1), // Push "key" paramater
                        new CodeInstruction(OpCodes.Ldfld, templateField), // Read "key.Template"
                        new CodeInstruction(OpCodes.Callvirt, getKeyIdMethod), // Call "key.Template.get_KeyId()"
                        new CodeInstruction(OpCodes.Ldstr, Skeletonkey.keyID), // Push the skeleton key ID
                        new CodeInstruction(OpCodes.Call, opEqualityMethod), // Compare "key.Template.KeyId" and "Skeletonkey.keyID"
                        new CodeInstruction(OpCodes.Brtrue, successLabel)); // If true, branch to the original success label
                }
                else
                {
                    Logger.LogError("Getting the success label failed in WorldInteractiveObject.UnlockOperation()!");
                    return instructions;
                }
            }
            else
            {
                Logger.LogError("Failed to find target IL instructions in WorldInteractiveObject.UnlockOperation()!");
                return instructions;
            }

            // Final code (3.11.x - 35392)
            // if (!(key.Template.KeyId == this.KeyId || key.Template.KeyId == "673e1f10aaf0fe810c488218"))
            return matcher.InstructionEnumeration();
        }
    }
}
