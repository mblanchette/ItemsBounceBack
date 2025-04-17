using HarmonyLib;
using System.Collections.Generic;
using System;
using UnityEngine;
using static PlayerHealth;
using System.Reflection.Emit;
using System.ComponentModel;

namespace ItemsBounceBack
{
    [HarmonyPatch(typeof(StatsManager))]
    internal static class ItemBouncePatch_StatsManager
    {
        [HarmonyPostfix, HarmonyPatch(nameof(StatsManager.Start))]
        public static void Start_Postfix(StatsManager __instance)
        {
            ItemsBounceBack.PopulateDictionary();
        }
    }

    [HarmonyPatch(typeof(HurtCollider))]
    internal static class ItemBouncePatch_HurtCollider
    {

        [HarmonyTranspiler, HarmonyPatch(nameof(HurtCollider.ColliderCheck), MethodType.Enumerator)]
        static IEnumerable<CodeInstruction> ColliderCheck_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions /*, ILGenerator generator*/);

            //
            // Expected Behavior: Add check to ColliderCheck() Line 227
            // if (!componentInParent4.destroyDisable)
            // to
            // if (!componentInParent4.destroyDisable || !ItemsBounceBack.ShouldItemBounce(componentInParent4.gameObject))
            //

            // Expect IL_03CE
            codeMatcher.MatchForward(true, (CodeMatch[])(object)new CodeMatch[7]
            {
                new CodeMatch((OpCode?)OpCodes.Brtrue),
                new CodeMatch((OpCode?)OpCodes.Ldloc_1),
                new CodeMatch((OpCode?)OpCodes.Ldfld),
                new CodeMatch((OpCode?)OpCodes.Brfalse),
                new CodeMatch((OpCode?)OpCodes.Ldloc_S),
                new CodeMatch((OpCode?)OpCodes.Ldfld),
                new CodeMatch((OpCode?)OpCodes.Brtrue)
            })
            .ThrowIfInvalid("ColliderCheck(): Couldn't find matching code");

            // IL_03CE, label IL_03F5
            var exitOperand = codeMatcher.Operand;

            codeMatcher.Advance(-1);
            codeMatcher.Advance(-1);
            // IL_03C7, label V_17
            var variableOperand = codeMatcher.Operand;

            //Go back to above codematch+1
            codeMatcher.Advance(1);
            codeMatcher.Advance(1);
            codeMatcher.Advance(1);

            codeMatcher.Insert((CodeInstruction[])(object)new CodeInstruction[4]
            {
                // store V_17 (componentInParent4 or PhyGrabObjectImpactDetector) to stack
			    new CodeInstruction(OpCodes.Ldloc_S, variableOperand),
                // get gameObject
                new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(UnityEngine.Component), "get_gameObject")),
                // call ItemsBounceBack.ShouldItemBounce, which checks for ItemAttributes component, then check ItemAttributes.itemAssetName against a list,
                new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(ItemsBounceBack), nameof(ItemsBounceBack.ShouldItemBounce))),
                // move to IL_03F5 if above statement is true
			    new CodeInstruction(OpCodes.Brtrue, exitOperand)
            });
            return codeMatcher.InstructionEnumeration();
        }
    }
}