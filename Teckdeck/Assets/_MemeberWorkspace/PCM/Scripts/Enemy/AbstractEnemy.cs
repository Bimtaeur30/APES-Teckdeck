using CombatSystem;
using Enemy.BT;
using Enemy.Interface;
using Systems.AgentSystem;
using Unity.Behavior;
using UnityEngine;

namespace Enemy
{
    public class AbstractEnemy : Agent
    {
        [field: SerializeField] public EnemyDataSO EnemyData { get; private set; }

        public int CurrentWayPointIndex { get; set; } = -1;
        public INavMovement NavMovement { get; private set; }
        public BehaviorGraphAgent BTAgent { get; private set; }
        public IRenderer Renderer { get; private set; }
        public ISensor Sensor { get; private set; }
        public ISkillModule SkillModule { get; private set; }
        public AgentTrigger Trigger { get; private set; }

        public CommandChange StateChannel { get; private set; }

        [SerializeField] private bool isDebugMode;

        protected override void InitializeModules()
        {
            base.InitializeModules();
            NavMovement = GetModule<INavMovement>();
            BTAgent = GetComponent<BehaviorGraphAgent>();
            Renderer = GetModule<IRenderer>();
            Sensor = GetModule<ISensor>();
            SkillModule = GetModule<ISkillModule>();
            Trigger = GetModule<AgentTrigger>();
        }

        protected override void AfterInitializeModules()
        {
            base.AfterInitializeModules();
            OnHit.AddListener(HandleHitEvent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnHit.RemoveListener(HandleHitEvent);
        }

        private void HandleHitEvent()
        {
            if (IsDead) return; //�Ʊ� ������ �����ؼ� ���� �۵��Ѵ�.

            StateChannel.SendEventMessage(StateCommands.HIT);
        }

        protected override void HandleDeath()
        {
            base.HandleDeath();

            Collider[] bodyColliders = GetComponents<Collider>();
            foreach (var c in bodyColliders)
                c.enabled = false;

            StateChannel.SendEventMessage(StateCommands.DIE); //������� ��ȯ.
        }

        protected virtual void Start()
        {
            if (GetVariable(BtVar.StateChannel, out BlackboardVariable<CommandChange> channel))
            {
                StateChannel = channel.Value;
            }
            SetVariableValue(BtVar.Enemy, this);
        }

        public void SetVariableValue<T>(string variableName, T value)
        {
            Debug.Assert(!string.IsNullOrEmpty(variableName), "���� �̸��� ��������� �ȵ˴ϴ�.");

            if (BTAgent.GetVariable(variableName, out BlackboardVariable<T> variable))
            {
                variable.Value = value;
            }
        }

        public bool GetVariable<T>(string variableName, out BlackboardVariable<T> variable)
        {
            Debug.Assert(!string.IsNullOrEmpty(variableName), "���� �̸��� ��������� �ȵ˴ϴ�.");
            return BTAgent.GetVariable(variableName, out variable);
        }

        private void OnDrawGizmos()
        {
            if (!isDebugMode) return;
            if (EnemyData == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, EnemyData.DetectRadius);
        }
    }
}