#if NONSENSICALKIT_IOCC
using System;
using System.Reflection;

namespace TemperatureVisualization
{
    /// <summary>
    /// 通过反射注册 IOCC，避免对 NonsensicalKit.Core 的程序集硬依赖。
    /// </summary>
    internal static class TemperatureVisualizationIocc
    {
        private static MethodInfo s_SetMethod;

        public static void Register(string id, object target)
        {
            if (string.IsNullOrEmpty(id) || target == null) return;
            if (!TryGetSetMethod(out MethodInfo set)) return;
            set.Invoke(null, new object[] { id, target });
        }

        private static bool TryGetSetMethod(out MethodInfo set)
        {
            set = s_SetMethod;
            if (set != null) return true;

            Type type = Type.GetType("NonsensicalKit.Core.IOCC, NonsensicalKit.Core.Runtime")
                ?? Type.GetType("NonsensicalKit.Core.IOCC, NonsensicalKit.Core");
            if (type == null) return false;

            set = type.GetMethod(
                "Set",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(string), typeof(object) },
                null);
            if (set != null) s_SetMethod = set;
            return set != null;
        }
    }
}
#endif
