using System;
using UnityEngine;

public class BaseResource : MonoBehaviour
{
    [SerializeField] private ResourceType _resourceType;

    public event Action<BaseResource> Harvest;

    public ResourceType ResourceType => _resourceType;

    public void Delete()
    {
        Harvest?.Invoke(this);
    }
}
