using UnityEngine;

class ResourceMarble : BaseResource
{
    public ResourceMarble() : base(ResourceType.Marble) { }

    protected override CollectedResource GetCollectedResource()
    {
        return Resources.Load<CollectedResource>("Prefabs/CollectedMarble");
    }
}
