using UnityEngine;

public interface IFactory<T> where T : Airport
{
    T Build(Transform transform);
}
