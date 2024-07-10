using UnityEngine;

class ResourceFood : BaseResource
{
    public ResourceFood() : base(ResourceType.Food) { }

    protected override CollectedResource GetCollectedResource()
    {
        return Resources.Load<CollectedResource>("Prefabs/CollectedFood");
    }
}
