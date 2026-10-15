using _Shared.Systems.FsmSystem.Runtime;
using ModuleSystem;
using UnityEngine;

public class GrapplePullState : AbstractState, IStateEnter<GrappleEnterContainer>
{
    private const float FlightDuration = 3f;
    private const float InitialSpeed = 18f;
    private const float SpeedDecay = 0.8f;

    private Vector3 _enemyPosition;
    private Player _player;
    private float _elapsed;
    private Vector3 _lastMouseDirection;
    private bool _hasMouseDirection;
    
    public GrapplePullState(ModuleOwner owner, int stateClipHash = 0) : base(owner, stateClipHash)
    {
        _player = owner as Player;
    }

    public void EnterWith(GrappleEnterContainer data, float duration)
    {
        _enemyPosition = data.EnemyPosition;
        Enter(duration);
    }

    public override void Enter(float transitionDuration, int layerIndex = 0)
    {
        base.Enter(transitionDuration, layerIndex);
        _elapsed = 0f;
        _lastMouseDirection = Vector3.zero;
        _hasMouseDirection = false;
        _player.MovementModule.PauseJump();
    }

    public override void Update()
    {
        base.Update();

        float step = Mathf.Min(Time.deltaTime, FlightDuration - _elapsed);
        if (step > 0f && _player.TryGetMouseDirection(out Vector3 direction))
        {
            _lastMouseDirection = direction;
            _hasMouseDirection = true;
            float speed = InitialSpeed * Mathf.Exp(-SpeedDecay * _elapsed);
            if (_player.MovementModule.MoveFlightStep(direction, speed * step))
            {
                Transition.ChangeState((int)StateEnum.Jump);
                return;
            }
        }

        _elapsed += step;
        if (_elapsed >= FlightDuration)
        {
            if (_hasMouseDirection)
                _player.MovementModule.SetJumpDirection(_lastMouseDirection);
            Transition.ChangeState((int)StateEnum.Jump);
        }
    }
}
