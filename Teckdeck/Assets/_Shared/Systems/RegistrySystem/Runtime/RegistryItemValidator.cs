using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace _Shared.Systems.RegistrySystem.Runtime
{
    //타입 정보는 다시 컴파일될 때만 바뀌고, 그때 도메인 리로드로 static 캐시도 비워지기 때문에 따로 무효화하지 않는다.
    public static class RegistryItemValidator
    {
        private static readonly Dictionary<string, Type> TypeCache = new();
        private static readonly Dictionary<Type, string> WarningCache = new();

        public static string GetTypeName(Type type) => $"{type.FullName}, {type.Assembly.GetName().Name}";

        public static Type ResolveType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;

            if (!TypeCache.TryGetValue(typeName, out Type type))
            {
                type = Type.GetType(typeName);
                TypeCache[typeName] = type;
            }
            return type;
        }

        //인스턴스를 만들 수 없는 타입(추상 클래스·인터페이스, 제네릭, UnityEngine.Object)은 후보에서 뺀다.
        public static bool IsCandidate(Type type)
        {
            if (type.IsAbstract)
                return false;

            if (type.ContainsGenericParameters)
                return false;

            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                return false;

            return true;
        }

        //추가는 막지 않고 무엇이 잘못됐는지 보여주기 위한 경고. 없으면 null
        public static string GetWarningMsg(Type type)
        {
            if (WarningCache.TryGetValue(type, out string warningMsg))
                return warningMsg;

            List<string> warnings = new List<string>();

            if (!type.IsSerializable)
                warnings.Add("[Serializable]이 없어 저장되지 않습니다");

            bool hasArgsCtor = type
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(c => c.GetParameters().Length > 0);
            if (hasArgsCtor)
                warnings.Add("매개변수가 있는 생성자는 호출되지 않습니다. 초기화는 OnRuntimeCreated에서 하세요");

            warningMsg = warnings.Count == 0 ? null : string.Join("\n", warnings);
            WarningCache[type] = warningMsg;
            return warningMsg;
        }
    }
}
