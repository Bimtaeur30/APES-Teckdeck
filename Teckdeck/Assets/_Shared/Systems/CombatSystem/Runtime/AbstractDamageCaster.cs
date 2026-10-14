using ModuleSystem;
using UnityEngine;

namespace CombatSystem
{
    public abstract class AbstractDamageCaster : MonoBehaviour
    {
        [SerializeField] protected LayerMask whatIsTarget;
        
        
        public ModuleOwner CasterOwner { get; private set; }
        public Vector3 LastHitPoint { get; protected set; }
        public Vector3 LastHitNormal { get; protected set; }
        public bool LastHitCritical { get; protected set; }
        
        //public IStatModule StatModule { get; protected set; }

        public virtual void InitCaster(ModuleOwner owner)
        {
            CasterOwner = owner;
            //StatModule = CasterOwner.GetModule<IStatModule>();
        }
        
        public abstract bool CastDamage(Vector3 position, Vector3 direction, SkillDataSO skillData);
    }
}