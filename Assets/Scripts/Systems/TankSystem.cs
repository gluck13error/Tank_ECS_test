using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Photon.Pun;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(TankSystem))]
public sealed class TankSystem : UpdateSystem
{
    public GameObject[] prefabBullets;
    private Filter filter;
    private Filter filterInit;
    private Filter filterShot;
    private Filter filterDestroy;
    private Filter filterSpawn;
    public override void OnAwake()
    {
        this.filter = this.World.Filter.With<TankComponent>().With<PositionComponent>().With<UnitComponent>();
        this.filterInit = this.filter.With<InitTankDataMarker>();
        this.filterShot = this.filter.With<TankShotMarker>();
        this.filterDestroy = this.filter.With<DestroyMarker>();
        this.filterSpawn = this.World.Filter.With<SpawnComponent>().With<PlayerComponent>();

    }
    public override void OnUpdate(float deltaTime)
    {
        InitData(deltaTime);
        Shot(deltaTime);
        foreach (var entity in this.filter)
        {
            ref var tank = ref entity.GetComponent<TankComponent>();
            tank.Reloading -= deltaTime;
        }
        Destroy(deltaTime);
    }

    private void InitData(float deltaTime)
    {
        foreach (var entity in this.filterInit)
        {
            ref var tank = ref entity.GetComponent<TankComponent>();
            ref var initTankData = ref entity.GetComponent<InitTankDataMarker>();
            tank.TimeReloaded = initTankData.TimeReloaded;
            entity.RemoveComponent<InitTankDataMarker>();
        }
    }
    private void Shot(float deltaTime)
    {

        foreach (var entity in this.filterShot)
        {
            ref var tank = ref entity.GetComponent<TankComponent>();
            ref var position = ref entity.GetComponent<PositionComponent>();
            ref var unit = ref entity.GetComponent<UnitComponent>();

            GameObject prefab = null;

            if (tank.dir.x != 0)
            {
                prefab = tank.dir.x == 1 ? prefabBullets[1] : prefabBullets[3];
            }
            else if (tank.dir.y != 0)
            {
                prefab = tank.dir.y == 1 ? prefabBullets[0] : prefabBullets[2];
            }
            var go = GameObject.Instantiate(prefab);
            Vector3 dop = tank.dir * (unit.Size / 2 + (Vector2.one * 0.08f));
            go.transform.position = position.Position + dop;

          

            entity.RemoveComponent<TankShotMarker>();
            //ref var tank = ref entity.GetComponent<TankComponent>();
            //ref var initTankData = ref entity.GetComponent<InitTankDataMarker>();
            //tank.TimeReloaded = initTankData.TimeReloaded;
            //entity.RemoveComponent<InitTankDataMarker>();
        }
    }
    private void Destroy(float deltaTime)
    {
        foreach (var entityTank in this.filterDestroy)
        {
        
            ref var playerTank = ref entityTank.GetComponent<PlayerComponent>();
            if (PhotonNetwork.IsMasterClient == playerTank.IsMasterClient)
            {
                foreach (var entity in this.filterSpawn)
                {
                    ref var player = ref entity.GetComponent<PlayerComponent>();
                  
                    if (PhotonNetwork.IsMasterClient == player.IsMasterClient)
                    {
                        var NetID = ++NetworkComponent.CountNetID;
                        entity.AddComponent<SpawnRunMarker>().NetID = NetID;

                        var newentity = World.CreateEntity();
                        newentity.SetComponent<NetworkComponent>(new NetworkComponent
                        {
                            Event = NetworkComponent.EventSpawn,
                            Data = new object[] { NetID }
                        });

                    }

                }

               

            }
        }
    }
}