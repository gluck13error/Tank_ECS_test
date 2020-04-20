using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using DG.Tweening;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(TankViewSystem))]
public sealed class TankViewSystem : UpdateSystem
{
    private Filter filter;
    private Filter filterImmortality;

    public override void OnAwake()
    {
        this.filter = this.World.Filter.With<UpdateTankVIewMarker>().With<TankViewComponent>().With<TankComponent>();
        this.filterImmortality = this.World.Filter.With<ImmortalityComponent>().With<TankViewComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {

        foreach (var entity in this.filter)
        {
            ref var view = ref entity.GetComponent<TankViewComponent>();
            ref var tank = ref entity.GetComponent<TankComponent>();

            if (tank.dir.x != 0)
            {
                view.srTank.sprite = tank.dir.x == 1 ? view.SpriteMoveLevel1[1] : view.SpriteMoveLevel1[3];
            }
            else if (tank.dir.y != 0)
            {
                view.srTank.sprite = tank.dir.y == 1 ? view.SpriteMoveLevel1[0] : view.SpriteMoveLevel1[2];
            }


            if (entity.Has<ImmortalityComponent>())
            {
                if (view.srImmortality.gameObject.activeSelf == false)
                {
                    view.srImmortality.gameObject.SetActive(true);
                }
            }
            else
            {
                if (view.srImmortality.gameObject.activeSelf == true)
                {
                    view.srImmortality.gameObject.SetActive(false);
                }
            }

            entity.RemoveComponent<UpdateTankVIewMarker>();
        }
        ImmortalityTime(deltaTime);
    }

    void ImmortalityTime(float deltaTime)
    {
        foreach (var entity in filterImmortality)
        {
            ref var Immortality = ref entity.GetComponent<ImmortalityComponent>();
            ref var view = ref entity.GetComponent<TankViewComponent>();
            Immortality.Time -= deltaTime;
            view.TimeAnimImmortality -= deltaTime;
            view.srImmortality.sprite = view.TimeAnimImmortality > 0 ? view.SpritesImmortality[0] : view.SpritesImmortality[1];

            if (view.TimeAnimImmortality < -0.1f)
                view.TimeAnimImmortality = 0.1f;

            if (Immortality.Time < 0)
            {
                entity.RemoveComponent<ImmortalityComponent>();
                entity.AddComponent<UpdateTankVIewMarker>();
            }
        }
    }
}