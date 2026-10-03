using UnityEngine;

public interface IViewInteraction
{
    public void Enter();
    public void Exit();
    public void HandleInput(); // 조작
}
