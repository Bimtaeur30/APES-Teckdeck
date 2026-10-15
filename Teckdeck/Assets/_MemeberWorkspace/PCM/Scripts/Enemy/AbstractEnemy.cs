using AnimatorSystem;
using Assets._MemeberWorkspace.PCM.Scripts.Spawn.Interface;
using CombatSystem;
using Enemy.BT;
using Enemy.Interface;
using Systems.AgentSystem;
using Unity.Behavior;
using UnityEngine;

namespace Enemy
{
    public abstract class AbstractEnemy : Agent
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
        private AnimationChannel _animationChannel;

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
            if (_animationChannel != null)
                _animationChannel.Event -= HandleAnimation;

            base.OnDestroy();
            OnHit.RemoveListener(HandleHitEvent);
        }

        private void HandleHitEvent()
        {
            if (IsDead || StateChannel == null)
                return;

            StateChannel.SendEventMessage(StateCommands.HIT);
        }

        protected override void HandleDeath()
        {
            base.HandleDeath();
            StateChannel?.SendEventMessage(StateCommands.DIE);
        }

        public void PrepareForSpawn()
        {
            CreateOwnChannels();
            IsDead = false;
            CurrentWayPointIndex = -1;

            if (Health != null)
                Health.SetHealth(Health.MaxHealth, Health.MaxHealth);

            NavMovement?.StopImmediately();
            SetVariableValue("TargetGameObject", (GameObject)null);
            SetVariableValue(BtVar.Enemy, this);
            BTAgent?.Restart();
        }

        protected virtual void Start()
        {
            CreateOwnChannels();
            SetVariableValue(BtVar.Enemy, this);
        }
        private void CreateOwnChannels()
        {
            CommandChange stateChannel = ScriptableObject.CreateInstance<CommandChange>();
            AnimationChannel animationChannel = ScriptableObject.CreateInstance<AnimationChannel>();
            SetVariableValue(BtVar.StateChannel, stateChannel);
            SetVariableValue(BtVar.AnimationChannel, animationChannel);
            StateChannel = stateChannel;
            if (_animationChannel != null)
                _animationChannel.Event -= HandleAnimation;
            _animationChannel = animationChannel;
            _animationChannel.Event += HandleAnimation;
        }

        private void HandleAnimation(HashDataSO hash)
        {
            if (hash == null)
                return;

            const float crossFadeDuration = 0.1f;
            if (Renderer != null)
            {
                Renderer.PlayClip(hash.HashValue, 0f, crossFadeDuration);
                return;
            }

            Animator animator = GetComponentInChildren<Animator>();
            animator?.CrossFadeInFixedTime(hash.HashValue, crossFadeDuration, 0, 0f);
        }

        public void SetVariableValue<T>(string variableName, T value)
        {
            Debug.Assert(!string.IsNullOrEmpty(variableName), "���� �̸��� ��������� �ȵ˴ϴ�."); //아 라이더 오류;;;

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