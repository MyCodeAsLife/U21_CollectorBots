using UnityEngine;

public class CollectorBotModel
{
    public SingleReactiveProperty<float> CollectionProgress = new();
    public BaseResource Resource;
    public MainBase MainBase;
    public Coroutine Moving;

    public Vector3 ResourceAttachmentPoint;
    public Vector3 TargetPoint;

    public bool IsWork;
    public bool HaveCollectedResource;
    public float MoveSpeed;
    public float MainBaseSize;
    public float DurationOfCollecting;
}