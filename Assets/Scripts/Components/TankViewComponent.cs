using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using DG.Tweening;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct TankViewComponent : IComponent {

    public SpriteRenderer srTank;
    public Sprite[] SpriteMoveLevel1;
    public SpriteRenderer srImmortality;
    public Sprite[] SpritesImmortality;
    public float TimeAnimImmortality;
}