using UnityEngine;

public interface ITypedFactory<T, TTypes> where T : IObject
{
    T Build(TTypes types, Transform transform);
}
