using System.Collections.Generic;
using UnityEngine;

public class ResourceScaner
{
    private Vector3 _scanningArea;

    public ResourceScaner(Transform map)
    {
        const float PlaneScale = 10;
        _scanningArea = new Vector3(map.localScale.x * PlaneScale, map.localScale.y * PlaneScale, map.localScale.z * PlaneScale);

    }

    public IList<BaseResource> MapScaning()
    {
        IList<BaseResource> list = new List<BaseResource>();
        Collider[] hits = Physics.OverlapBox(Vector3.zero, _scanningArea);

        foreach (Collider hit in hits)
            if (hit.TryGetComponent(out BaseResource resource))
                list.Add(resource);

        return list;
    }
}
