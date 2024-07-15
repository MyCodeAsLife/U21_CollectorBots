using UnityEngine;

public class CollectedResource : MonoBehaviour
{
    public ResourceType Type { get; private set; }

    public CollectedResource(ResourceType resourceType)
    {
        Type = resourceType;
    }
}
