using _Shared.Systems.RegistrySystem.Runtime;
using UnityEngine;

namespace JTH.RegistryTest
{
    //RegistryRuntime 사본 생성·조회 확인용(PHASE-018). 확인이 끝나면 지워도 된다.
    //플레이 중 RegistryRuntime 인스펙터에서 사본 값을 바꾼 뒤 컨텍스트 메뉴로 다시 비교하면, 사본만 바뀌고 SO는 그대로인지 볼 수 있다.
    [RequireComponent(typeof(RegistryRuntime))]
    public class RegistryRuntimeTester : MonoBehaviour
    {
        private void Start() => LogCompareWithSO();

        [ContextMenu("Compare With SO")]
        private void LogCompareWithSO()
        {
            RegistryRuntime runtime = GetComponent<RegistryRuntime>();
            if (runtime.RegistrySO == null)
            {
                Debug.LogWarning("[RegistryTest] RegistrySO가 비어 있습니다", this);
                return;
            }

            foreach (RegistryEntry entry in runtime.RegistrySO.entries)
            {
                if (!runtime.TryGetItem(entry.enumValue, out IRegistryItem copy))
                {
                    Debug.Log($"[RegistryTest] {entry.enumKeyName}: 사본 없음(건너뜀)", this);
                    continue;
                }

                bool isSeparate = !ReferenceEquals(copy, entry.registryItem);
                bool isSameValue = JsonUtility.ToJson(copy) == JsonUtility.ToJson(entry.registryItem);
                Debug.Log($"[RegistryTest] {entry.enumKeyName}: {copy.GetType().Name}, 원본과 다른 객체 {isSeparate}, 값 같음 {isSameValue}\n" +
                          $"사본: {JsonUtility.ToJson(copy)}\nSO  : {JsonUtility.ToJson(entry.registryItem)}", this);
            }
        }
    }
}
