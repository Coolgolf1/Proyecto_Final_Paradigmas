using UnityEngine;

public interface ITypedFactory<T, TTypes> where T : Airplane
{
    T Build(TTypes types, Transform transform);
}
