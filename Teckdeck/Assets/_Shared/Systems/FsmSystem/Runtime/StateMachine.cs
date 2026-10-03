using System;
using System.Collections.Generic;
using ModuleSystem;
using UnityEngine;

namespace _Shared.Systems.FsmSystem.Runtime
{
    public class StateMachine : IStateTransition
    {
        public AbstractState CurrentState { get; private set; }
        public int CurrentStateIdx { get; private set; }

        private Dictionary<int, AbstractState> _stateDict;

        public StateMachine(ModuleOwner owner, StateSO[] stateList)
        {
            _stateDict = new Dictionary<int, AbstractState>();
            foreach (StateSO stateData in stateList)
            {
                Type type = Type.GetType(stateData.className); //해당 이름의 클래스 타입을 가져온다.
                Debug.Assert(type != null, $"타입을 찾는데 실패했습니다. : {stateData.className}");

                int paramHash = stateData.stateParam == null ? 0 : stateData.stateParam.HashValue;
                
                AbstractState abstractState = (AbstractState)Activator.CreateInstance(type, owner, paramHash);
                abstractState.BindTransition(this);
                _stateDict.Add(stateData.assetIndex, abstractState);
            }
        }

        public void ChangeState(int newStateIndex, float transitionDuration = 0.1f)
        {
            CurrentState?.Exit();
            AbstractState newState = _stateDict.GetValueOrDefault(newStateIndex);
            Debug.Assert(newState != null, $"찾고자하는 인덱스의 상태가 없습니다. : {newStateIndex}");

            CurrentState = newState;
            CurrentStateIdx = newStateIndex;
            CurrentState.Enter(transitionDuration);
            
            Debug.Log($"현재 상태: {CurrentState}");
        }

        public void ChangeState<TData>(int index, TData data, float duration = 0.1f)
        {
            if (!_stateDict.TryGetValue(index, out AbstractState next))
                throw new KeyNotFoundException($"상태 {index}가 없습니다.");

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
