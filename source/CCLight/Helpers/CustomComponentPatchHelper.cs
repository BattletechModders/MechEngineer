using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using MechEngineer;

namespace CustomComponents
{
    public static class DataManagerPatchHelper
    {
        public static IEnumerable<CodeInstruction> ReplaceDefaultConstructor<T>(IEnumerable<CodeInstruction> instructions) where T : class, new()
        {
            return instructions.ReplaceConstructorWithStaticFactoryMethod(
                    AccessTools.Constructor(typeof(T)),
                    ((Func<T>)CreateCustomType<T>).Method
                );
        }

        internal static CustomComponentDescriptor ComponentTypeDescriptor;

        public static T CreateCustomType<T>() where T : class, new()
        {
            try
            {
                var instance = ComponentTypeDescriptor?.CreateNew();
                if (instance is T casted)
                {
                    return casted;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return new T();
        }

        public static IEnumerable<CodeInstruction> ReplaceConstructorWithStaticFactoryMethod(
            this IEnumerable<CodeInstruction> instructions,
            ConstructorInfo from, MethodInfo to)
        {
            foreach (var instruction in instructions)
            {
                var operand = instruction.operand as ConstructorInfo;
                if (operand == from)
                {
                    yield return new CodeInstruction(OpCodes.Call, to);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}