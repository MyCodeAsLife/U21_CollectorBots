using System;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private ResourceType _resourceType;

    public event Action<Resource> Harvest;

    public ResourceType ResourceType => _resourceType;

    public void Delete()
    {
        Harvest?.Invoke(this);
    }
}
