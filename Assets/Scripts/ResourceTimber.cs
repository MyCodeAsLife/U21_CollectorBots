using UnityEngine;

class ResourceTimber : BaseResource
{
    public ResourceTimber() : base(ResourceType.Timber) { }

    protected override CollectedResource GetCollectedResource()
    {
        return Resources.Load<CollectedResource>("Prefabs/CollectedTimber");
    }
}
