using AnimatorSystem;
using UnityEngine;

namespace _Shared.Systems.FsmSystem.Runtime
{
    [CreateAssetMenu(fileName = "State data", menuName = "Agent/State data", order = 0)]
    public class StateSO : ScriptableObject
    {
        public string stateName;
        public string className;
        public int assetIndex;
        public HashDataSO stateParam;
    }
}