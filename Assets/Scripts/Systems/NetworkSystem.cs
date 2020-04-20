using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(NetworkSystem))]


public sealed class NetworkSystem : FixedUpdateSystem
{

    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
    RaiseEventOptions raiseOthersEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
    SendOptions sendOptions = new SendOptions { Reliability = true };
    private Filter filterSpawn;
    private Filter filter;
    private Filter filterTankPlayer;
    private Filter filterMove;
    private Filter filterNetMove;

    private int countNetID = 1;
    public override void OnAwake()
    {

        this.filter = this.World.Filter.With<NetworkComponent>();
        this.filterSpawn = this.World.Filter.With<SpawnComponent>().With<PlayerComponent>();
        this.filterMove = this.World.Filter.With<PositionComponent>().With<MoveComponent>().With<PlayerComponent>();
        this.filterNetMove = this.World.Filter.With<PositionComponent>().Without<MoveComponent>().With<PlayerComponent>();
        this.filterTankPlayer = this.World.Filter.With<TankComponent>().With<PlayerComponent>();
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }


    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        var data = (object[])photonEvent.CustomData;
        if (eventCode == NetworkComponent.EventSpawn)
        {
            foreach (var entity in this.filterSpawn)
            {
                ref var player = ref entity.GetComponent<PlayerComponent>();
                if (PhotonNetwork.IsMasterClient != player.IsMasterClient)
                {
                    entity.AddComponent<SpawnRunMarker>().NetID = (int)data[0];
                    //    Debug.Log($"AddComponent<SpawnRunMarker>");

                }
            }

        }
        if (eventCode == NetworkComponent.EventRefreshOrShot)
        {
            foreach (var entity in this.filterTankPlayer)
            {
                ref var player = ref entity.GetComponent<PlayerComponent>();
                if (PhotonNetwork.IsMasterClient != player.IsMasterClient && player.NetID == (int)data[0])
                {
                    ref var tank = ref entity.GetComponent<TankComponent>();
                    if (tank.dir.x != (float)data[1] || tank.dir.y != (float)data[2])
                    {
                        tank.dir.x = (float)data[1];
                        tank.dir.y = (float)data[2];
                        entity.AddComponent<UpdateTankVIewMarker>();
                    }
                    if ((bool)data[3])
                        entity.AddComponent<TankShotMarker>();
                    //    Debug.Log($"AddComponent<SpawnRunMarker>");

                }
            }

        }

        if (eventCode == NetworkComponent.EventMove)
        {
            foreach (var entity in this.filterNetMove)
            {
                ref var player = ref entity.GetComponent<PlayerComponent>();
                if (PhotonNetwork.IsMasterClient != player.IsMasterClient && player.NetID == (int)data[0])
                {
                    ref var position = ref entity.GetComponent<PositionComponent>();
                    position.Position.x = (float)data[1];
                    position.Position.y = (float)data[2];
                }
            }

        }

    }
    object[] moveData = new object[] { (int)0, (float)0f, (float)0f };
    public override void OnUpdate(float deltaTime)
    {
        foreach (var entity in this.filter)
        {
            ref var network = ref entity.GetComponent<NetworkComponent>();
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.RaiseEvent(network.Event, network.Data, raiseOthersEventOptions, sendOptions);
            }
            World.RemoveEntity(entity);
        }

        foreach (var entity in this.filterMove)
        {
            ref var player = ref entity.GetComponent<PlayerComponent>();
            if (player.NetID > 0 && PhotonNetwork.IsConnectedAndReady && PhotonNetwork.IsMasterClient == player.IsMasterClient)
            {
                ref var position = ref entity.GetComponent<PositionComponent>();
                moveData[0] = player.NetID;
                moveData[1] = position.Position.x;
                moveData[2] = position.Position.y;
                PhotonNetwork.RaiseEvent(NetworkComponent.EventMove, moveData, raiseOthersEventOptions, SendOptions.SendUnreliable);
            }
        }
    }
}