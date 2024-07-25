using System.Collections.Generic;
using UnityEngine;

public class ResourceScaner
{
    private int _resourceMask;
    private Vector3 _scanningArea;

    public ResourceScaner(Transform map)
    {
        const float PlaneScale = 10;
        _resourceMask = LayerMask.NameToLayer("Resource");
        _scanningArea = new Vector3(map.localScale.x * PlaneScale, map.localScale.y * PlaneScale, map.localScale.z * PlaneScale);
    }

    public IList<Resource> MapScaning()
    {
        IList<Resource> list = new List<Resource>();
        Collider[] hits = Physics.OverlapBox(Vector3.zero, _scanningArea,Quaternion.identity, _resourceMask);       // создать int.max и вычесть из него маску _resourceMask  a ^ b https://informatics.msk.ru/mod/book/view.php?id=578&chapterid=306

        foreach (Collider hit in hits)
            if (hit.gameObject.layer == _resourceMask)
                list.Add(hit.GetComponent<Resource>());

        return list;
    }
}