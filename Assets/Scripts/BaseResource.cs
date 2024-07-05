using UnityEngine;

public class BaseResource : MonoBehaviour
{
    public ResourceType ResourceType { get; private set; }

    public BaseResource(ResourceType resourceType)
    {
        ResourceType = resourceType;
    }
}
