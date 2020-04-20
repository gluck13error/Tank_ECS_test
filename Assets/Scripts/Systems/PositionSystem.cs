using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PositionSystem))]
public sealed class PositionSystem : UpdateSystem
{
    private Filter filter;
    public override void OnAwake()
    {
        this.filter = this.World.Filter.With<PositionComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        //foreach (var entity in this.filter)

        //{
        //    ref var position = ref entity.GetComponent<PositionComponent>();
        //    position.Position += Vector3.right * deltaTime;


        //}
    }
}