﻿// -----------------------------------------------------------------------
// <copyright file="DisableLightSourceToyUpdatePatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

namespace Mistaken.Toy.API.Patch
{
    [UsedImplicitly]
    [HarmonyPatch(typeof(AdminToys.LightSourceToy), nameof(AdminToys.LightSourceToy.Update))]
    internal static class DisableLightSourceToyUpdatePatch
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
