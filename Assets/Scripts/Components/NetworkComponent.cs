using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]

public struct NetworkComponent : IComponent {
    public static int CountNetID =1;
    public const byte EventSpawn = 1;
    public const byte EventRefreshOrShot = 2;
    public const byte EventMove = 3;
    public byte Event;
    public object[] Data;
}