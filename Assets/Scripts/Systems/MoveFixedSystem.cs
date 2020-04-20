using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(MoveFixedSystem))]
public sealed class MoveFixedSystem : FixedUpdateSystem
{
    public float MoveLen = 0.08f;
    public LayerMask Ground;
    private Filter filter;
    private Filter filterStart;
    private Filter filterMove;
    public override void OnAwake()
    {
        this.filter = this.World.Filter.With<PositionComponent>().With<UnitComponent>();
        this.filterStart = this.filter.With<StartMoveComponent>().Without<MoveComponent>();
        this.filterMove = this.filter.With<MoveComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {

        foreach (var entity in filterStart)
        {
            ref var unit = ref entity.GetComponent<UnitComponent>();
            ref var start = ref entity.GetComponent<StartMoveComponent>();
            ref var position = ref entity.GetComponent<PositionComponent>();

            var newpos = position.Position + (start.Move * MoveLen);
            bool isMove = true;
            foreach (var entityCollision in this.filter)
            {
                ref var unitCollision = ref entityCollision.GetComponent<UnitComponent>();
                ref var positionCollision = ref entityCollision.GetComponent<PositionComponent>();
                if ((newpos.x - (unit.Size.x / 2f) > positionCollision.Position.x + (unitCollision.Size.x / 2)) ||
                    (newpos.x + (unit.Size.x / 2f) < positionCollision.Position.x - (unitCollision.Size.x / 2)) ||
                    (newpos.y - (unit.Size.y / 2f) > positionCollision.Position.y + (unitCollision.Size.y / 2)) ||
                    (newpos.y + (unit.Size.y / 2f) < positionCollision.Position.y - (unitCollision.Size.y / 2)))
                {
                }
                else if (entity != entityCollision)
                {

                    isMove = false;
                    break;
                }
            }
            if (isMove)
            {
                ref var move = ref entity.AddComponent<MoveComponent>();
                move.Speed = start.Move * MoveLen / start.Time;
                move.Time = start.Time;

            }
            entity.RemoveComponent<StartMoveComponent>();
        }
        foreach (var entity in this.filterMove)
        {

            ref var move = ref entity.GetComponent<MoveComponent>();
            ref var position = ref entity.GetComponent<PositionComponent>();

            position.Position += move.Speed * (move.Time < deltaTime ? move.Time : deltaTime);
            move.Time -= deltaTime;
            if (move.Time <= 0)
            {
                // Debug.Log("RemoveComponent MoveComponent");
                entity.RemoveComponent<MoveComponent>();
            }

        }
    }

}