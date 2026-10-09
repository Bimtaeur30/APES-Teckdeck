namespace _Shared.Systems.RegistrySystem.Runtime
{
    //RegistryRuntime이 SO 원본을 복사해 런타임 사본을 만든 직후 한 번 호출된다. 부모와 자식 중 어디에 붙여도 된다.
    public interface IRegistryCreatedReceiver
    {
        public void OnRuntimeCreated();
    }
}