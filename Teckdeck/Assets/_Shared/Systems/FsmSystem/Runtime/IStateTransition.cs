public interface IStateTransition
{
    void ChangeState(int index, float duration = 0.1f);
    void ChangeState<TData>(int index, TData data, float duration = 0.1f);
}

public interface IStateEnter<TData>
{
    void EnterWith(TData data, float duration);
}
