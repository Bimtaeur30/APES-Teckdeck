using System;
using ModuleSystem;
using UnityEngine;

public class PlayerMovementModule : MonoBehaviour, IModule, IPlayerMovementModule
{   
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float overlapSphereRadius = 5f;
    [SerializeField] private float jumpForce = 10f;
    
    [SerializeField] private bool IsWall = false;
    private readonly Collider[] colliders = new Collider[8];
    private Player _player;
    private Rigidbody _rigidbody;
    
    public void Initialize(ModuleOwner owner)
    {
        _player = owner as Player;
        _rigidbody = _player.Rigidbody;
    }

    private void Update()
    {
        CheckIsWall();
    }

    public void JumpTo(Vector3 dir)
    {
        if (!IsWall) return;
        _rigidbody.AddForce(dir * jumpForce, ForceMode.Impulse);
    }

    private void OnWallEnter()
    {
        _rigidbody.linearVelocity = Vector3.zero;
    }

    private void CheckIsWall()
    {
        if (Physics.OverlapSphereNonAlloc(transform.position, overlapSphereRadius, colliders , wallLayer) > 0)
        {
            if (!IsWall)
                OnWallEnter();
            IsWall = true;
        }
        else
        {
            IsWall = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (IsWall)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, overlapSphereRadius);
    }
}