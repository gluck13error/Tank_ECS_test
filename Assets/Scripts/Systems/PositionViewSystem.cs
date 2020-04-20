using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;


[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PositionViewSystem))]
public sealed class PositionViewSystem : UpdateSystem {
    private Filter filter;
    private Filter filterInit;
    private Filter filterDestroy;
    public override void OnAwake() {
        this.filter = this.World.Filter.With<PositionComponent>().With<PositionViewComponent>();
        this.filterInit = this.filter.With<InitPositionViewComponent>();
        this.filterDestroy = this.filter.With<DestroyMarker>();
    }

    public override void OnUpdate(float deltaTime) {

        Init(deltaTime);
        
        var views = this.filter.Select<PositionViewComponent>();
        var positions = this.filter.Select<PositionComponent>();
        for (int i = 0; i < this.filter.Length; i++)
        {
            ref var postion = ref positions.GetComponent(i);
            ref var view = ref views.GetComponent(i);
            view.Transform.position = postion.Position; 
        }
        Destroy(deltaTime);
    }

    private void Init(float deltaTime)
    {
        foreach (var entity in this.filterInit)
        {
            ref var view = ref entity.GetComponent<PositionViewComponent>();
            ref var postion = ref entity.GetComponent<PositionComponent>();
            postion.Position = view.Transform.position;
            entity.RemoveComponent<InitPositionViewComponent>();
        }
    }

    private  void Destroy(float deltaTime)
    {
        foreach (var entity in this.filterDestroy)
        {
            ref var view = ref entity.GetComponent<PositionViewComponent>();
            Destroy(view.Transform.gameObject);
            World.RemoveEntity(entity);
        }
    }
}