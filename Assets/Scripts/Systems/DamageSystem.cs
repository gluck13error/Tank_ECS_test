using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(DamageSystem))]
public sealed class DamageSystem : UpdateSystem
{
    private Filter filter;
  //  private Filter filterImmortality;
    public override void OnAwake()
    {
        this.filter = this.World.Filter.With<DamageСausedComponent>();
       // this.filterImmortality = this.World.Filter.With<ImmortalityComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        DamageСaused(deltaTime);
        ////ImmortalityTime(deltaTime);
    }
    void DamageСaused(float deltaTime)
    {
        foreach (var entity in filter)
        {
            ref var damage = ref entity.GetComponent<DamageСausedComponent>();

            if (entity.Has<HealthPointComponent>() && !entity.Has<ImmortalityComponent>())
            {
                ref var hp = ref entity.GetComponent<HealthPointComponent>();
                hp.HealthPoint -= damage.Damage;
                if (hp.HealthPoint <= 0)
                    entity.AddComponent<DestroyMarker>();
            }
            entity.RemoveComponent<DamageСausedComponent>();
        }
    }
    //void ImmortalityTime(float deltaTime)
    //{
    //    foreach (var entity in filterImmortality)
    //    {
    //        ref var Immortality = ref entity.GetComponent<ImmortalityComponent>();
    //        Immortality.Time -= deltaTime;

    //        if (Immortality.Time < 0)
    //        {
    //            entity.RemoveComponent<ImmortalityComponent>();
    //            entity.AddComponent<UpdateTankVIewMarker>();
    //        }
    //    }
    //}
}