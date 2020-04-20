using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using TMPro;
using DG.Tweening;
using Photon.Pun;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(LevelSystem))]
public sealed class LevelSystem : UpdateSystem
{
    public TextMeshPro textPrefab;
    private Filter filterSpawn;
    private Filter filterEndGame;
    public override void OnAwake()
    {
        this.filterSpawn = this.World.Filter.With<SpawnComponent>().With<PlayerComponent>();
        this.filterEndGame = this.World.Filter.With<EndGameComponent>().With<DestroyMarker>();

        DOTween.Sequence().AppendCallback(() => ShowInfo(3))
                          .AppendInterval(1f)
                          .AppendCallback(() => ShowInfo(2))
                          .AppendInterval(1f)
                          .AppendCallback(() => ShowInfo(1))
                          .AppendInterval(1f)
                          .AppendCallback(() =>
                            {
                                ShowInfo(0);
                                Start();
                            });
    }
    private void Start()
    {
        foreach (var entity in this.filterSpawn)
        {
            ref var player = ref entity.GetComponent<PlayerComponent>();
            //  Debug.Log($"Start {PhotonNetwork.IsMasterClient} == {player.IsMasterClient} ");
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
    void ShowInfo(int info)
    {
        var text = GameObject.Instantiate<TextMeshPro>(textPrefab);
        text.text = info.ToString();
        text.transform.DOScale(0.3f, 0.4f).OnComplete(() => Destroy(text.gameObject));

    }
    public override void OnUpdate(float deltaTime)
    {
        foreach (var entity in filterEndGame )
        {
            ref var player = ref entity.GetComponent<PlayerComponent>();
            if (player.IsMasterClient == PhotonNetwork.IsMasterClient)
            {
                var text = GameObject.Instantiate<TextMeshPro>(textPrefab);
                text.text = "You Lost";
                text.transform.DOScale(1.1f, 0.4f).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                var text = GameObject.Instantiate<TextMeshPro>(textPrefab);
                text.text = "You Win";
                text.transform.DOScale(1.1f, 0.4f).SetLoops(-1, LoopType.Yoyo);
            }
        }
    }
}