using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(SpawnSystem))]
public sealed class SpawnSystem : UpdateSystem {
    private Filter filter;
    public float ImmortalitySpawnTime = 4;
    public override void OnAwake()
    {
        this.filter = this.World.Filter.With<SpawnComponent>().With<SpawnRunMarker>().With<PositionComponent>().With<PlayerComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        foreach (var entity in this.filter)
        {
            ref var spawn = ref entity.GetComponent<SpawnComponent>();
            ref var position = ref entity.GetComponent<PositionComponent>();
            ref var playerSpawn = ref entity.GetComponent<PlayerComponent>();
            ref var spawnRun = ref entity.GetComponent<SpawnRunMarker>();

            var go = GameObject.Instantiate(spawn.Prefab);
            go.transform.position = position.Position;
            var playerProvider = go.AddComponent<PlayerProvider>();
            ref var player = ref playerProvider.GetData();
            player.IsMasterClient = playerSpawn.IsMasterClient;
            player.NetID = spawnRun.NetID;

            playerProvider.Entity.AddComponent<ImmortalityComponent>().Time = ImmortalitySpawnTime;
            playerProvider.Entity.AddComponent<UpdateTankVIewMarker>();

            entity.RemoveComponent<SpawnRunMarker>();
        }
    }
}