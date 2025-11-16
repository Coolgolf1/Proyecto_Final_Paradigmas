using UnityEngine;

public interface IFactory<T> where T : IObject
{
    T Build(Transform transform);
}