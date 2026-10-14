using System;
using _Shared.Systems.RegistrySystem.Runtime;
using ModuleSystem;
using UnityEngine;

namespace FsmSystem
{
    public class StateMachine : IStateTransition
    {
        public AbstractState CurrentState { get; private set; }
        public int CurrentStateIdx { get; private set; }

        private RegistryRuntime _stateRegistry;

        public StateMachine(ModuleOwner owner, RegistryRuntime stateRegistry)
        {
            _stateRegistry = stateRegistry;

            foreach (AbstractState state in _stateRegistry.GetItemList<AbstractState>())
            {
                state.InitializeState(owner);
                state.BindTransition(this);
            }
        }
        
        public void ChangeState(int newStateIndex, float transitionDuration = 0.1f)
        {
            if (_stateRegistry.TryGetItem(newStateIndex, out AbstractState newState) == false)
            {
                Debug.LogError($"찾고자하는 인덱스의 상태가 없습니다. : {newStateIndex}");
                return;
            }

            CurrentState?.Exit();
            CurrentState = newState;
            CurrentStateIdx = newStateIndex;
            CurrentState.Enter(transitionDuration);
            
            Debug.Log($"현재 상태: {CurrentState}");
        }

        public void ChangeState<TData>(int index, TData data, float duration = 0.1f)
        {
            if (_stateRegistry.TryGetItem(index, out AbstractState next) == false)
            {
                Debug.LogError($"찾고자하는 인덱스의 상태가 없습니다. : {index}");
                return;
            }

            if (next is not IStateEnter<TData> receiver)
                throw new InvalidOperationException($"상태 {index}는 {typeof(TData).Name} 데이터를 받지 않습니다.");

            CurrentState?.Exit();
            CurrentState = next;
            CurrentStateIdx = index;
            receiver.EnterWith(data, duration);
            
            Debug.Log($"현재 상태: {CurrentState}");
        }

        public void UpdateMachine() => CurrentState?.Update();
        
    }
}
