using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System.Collections.Generic;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(CollisionSystem))]
public sealed class CollisionSystem : UpdateSystem
{
    public LayerMask groud;
    private Filter filter;
    private Filter filterMove;
    public override void OnAwake()
    {
        this.filter = this.World.Filter.With<PositionComponent>().With<UnitComponent>();
        this.filterMove = this.filter.With<BulletComponent>().With<MoveComponent>();
    }


    public override void OnUpdate(float deltaTime)
    {

        foreach (var entity in this.filterMove)
        {
            ref var unit = ref entity.GetComponent<UnitComponent>();
            ref var position = ref entity.GetComponent<PositionComponent>();
            ref var Bullet = ref entity.GetComponent<BulletComponent>();

            List<IEntity> collisions = new List<IEntity>();
            foreach (var entityCollision in this.filter)
            {
                ref var unitCollision = ref entityCollision.GetComponent<UnitComponent>();
                ref var positionCollision = ref entityCollision.GetComponent<PositionComponent>();
                if ((position.Position.x - (unit.Size.x / 2f) > positionCollision.Position.x + (unitCollision.Size.x / 2)) ||
                    (position.Position.x + (unit.Size.x / 2f) < positionCollision.Position.x - (unitCollision.Size.x / 2)) ||
                    (position.Position.y - (unit.Size.y / 2f) > positionCollision.Position.y + (unitCollision.Size.y / 2)) ||
                    (position.Position.y + (unit.Size.y / 2f) < positionCollision.Position.y - (unitCollision.Size.y / 2)))
                {
                }
                else if (entity != entityCollision)
                {
                    collisions.Add(entityCollision);
                }
            }
            if (collisions.Count > 0)
            {
                foreach (var collision in collisions)
                {
                    collision.AddComponent<DamageСausedComponent>().Damage = Bullet.TargetDamage;
                }
                entity.AddComponent<DestroyMarker>();
            }

        }
    }
}