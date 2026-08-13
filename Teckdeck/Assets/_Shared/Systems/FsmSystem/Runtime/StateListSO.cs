using UnityEngine;

namespace _Shared.Systems.FsmSystem.Runtime
{
    [CreateAssetMenu(fileName = "State list data", menuName = "Agent/State list", order = 21)]
    public class StateListSO : ScriptableObject
    {
        [HideInInspector] public string generatePath;
        public string enumName;
        public StateSO[] states;
    }
}