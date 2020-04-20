using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Photon.Pun;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(InputSystem))]
public sealed class InputSystem : UpdateSystem
{
    private Filter filter;
    public override void OnAwake()
    {

        this.filter = this.World.Filter.With<PlayerComponent>().With<TankComponent>().Without<StartMoveComponent>().Without<MoveComponent>();
    }

    void SetMovePlayer(IEntity entity, float x, float y)
    {

        ref var move = ref entity.AddComponent<StartMoveComponent>(out _);
        move.Move.x = x;
        move.Move.y = y;
        move.Time = 0.15f;
        entity.GetComponent<TankComponent>().dir = move.Move;

        ref var player = ref entity.GetComponent<PlayerComponent>();
        ref var tank = ref entity.GetComponent<TankComponent>();
        var newentity = World.CreateEntity();
        newentity.SetComponent<NetworkComponent>(new NetworkComponent
        {
            Event = NetworkComponent.EventRefreshOrShot,
            Data = new object[] { player.NetID, tank.dir.x, tank.dir.y,false }
        });


        entity.AddComponent<UpdateTankVIewMarker>();

    }
    public override void OnUpdate(float deltaTime)
    {
        foreach (var entity in this.filter)
        {
            ref var player = ref entity.GetComponent<PlayerComponent>();
            if (PhotonNetwork.IsMasterClient != player.IsMasterClient)
                continue;

            if (Input.GetKey(KeyCode.A))
            {
                SetMovePlayer(entity, -1, 0);

            }
            else if (Input.GetKey(KeyCode.D))
            {
                SetMovePlayer(entity, 1, 0);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                SetMovePlayer(entity, 0, 1);
            }

            else if (Input.GetKey(KeyCode.S))
            {
                SetMovePlayer(entity, 0, -1);
            }


            if (Input.GetKey(KeyCode.Space))
            {
                ref var tank = ref entity.GetComponent<TankComponent>();
                if (tank.Reloading < 0)
                {
                    entity.AddComponent<TankShotMarker>();
                    tank.Reloading = tank.TimeReloaded;


                    var newentity = World.CreateEntity();
                    newentity.SetComponent<NetworkComponent>(new NetworkComponent
                    {
                        Event = NetworkComponent.EventRefreshOrShot ,
                        Data = new object[] { player.NetID, tank.dir.x, tank.dir.y,true }
                    });
                }
            }

            //else
            //{
            //    entity.RemoveComponent<MoveComponent>();
            //}
        }

    }
}