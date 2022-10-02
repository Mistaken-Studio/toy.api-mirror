// -----------------------------------------------------------------------
// <copyright file="ToysPositionSyncOptimizationPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Mistaken.Toy.API.Patch
{
    // ReSharper disable UnusedMember.Global
    [HarmonyPatch(typeof(AdminToys.AdminToyBase), nameof(AdminToys.AdminToyBase.LateUpdate))]
    internal static class ToysPositionSyncOptimizationPatch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var label = generator.DefineLabel();

            yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(ToyHandler), nameof(ToyHandler.ManagedToys)));
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(HashSet<AdminToys.AdminToyBase>), nameof(HashSet<AdminToys.AdminToyBase>.Contains), parameters: new[] { typeof(AdminToys.AdminToyBase) }));
            yield return new CodeInstruction(OpCodes.Brfalse_S, label);
            yield return new CodeInstruction(OpCodes.Ret);

            // Return first with .WithLabels(label) and return rest normally
            var added = false;
            foreach (var t in instructions)
            {
                if (added)
                    yield return t;
                else
                {
                    added = true;
                    yield return t.WithLabels(label);
                }
            }
        }
    }
}
