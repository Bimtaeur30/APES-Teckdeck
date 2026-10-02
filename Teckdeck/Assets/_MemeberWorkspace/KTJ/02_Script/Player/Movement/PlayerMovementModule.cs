using System;
using ModuleSystem;
using UnityEngine;

public class PlayerMovementModule : MonoBehaviour, IModule, IPlayerMovementModule
{   
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float overlapSphereRadius = 5f;
    [SerializeField] private float jumpForce = 10f;
    
    private readonly Collider[] colliders = new Collider[8];
    private Player _player;
    private MovementVector _currentMovementVector;
    
    private bool isWall = false;
    private bool isJumping;
    private bool leftStartWall;
    public void Initialize(ModuleOwner owner)
    {
        _player = owner as Player;
    }

    private void Update()
    {
        CheckIsWall();
        Jump();
    }

    private void Jump()
    {
        if (!isJumping) return;

        if (!isWall)
            leftStartWall = true;

        if (leftStartWall && isWall)
        {
            isJumping = false;
            OnWallEnter();
            return;
        }

        _player.transform.position +=
            _currentMovementVector.Direction * (jumpForce * Time.deltaTime);
    }

    public void JumpStart(MovementVector vector)
    {
        if (!isWall || isJumping) return;

        _currentMovementVector = vector;
        leftStartWall = false;
        isJumping = true;
    }

    private void OnWallEnter(){}

    private void CheckIsWall()
    {
        if (Physics.OverlapSphereNonAlloc(transform.position, overlapSphereRadius, colliders , wallLayer) > 0)
        {
            if (!isWall)
                OnWallEnter();
            isWall = true;
        }
        else
        {
            isWall = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isWall ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, overlapSphereRadius);
    }
}