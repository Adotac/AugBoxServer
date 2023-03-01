using FishNet.Object;
using UnityEngine;

public sealed class PawnCamera : NetworkBehaviour
{
    private PawnInput input;

    [SerializeField] private Transform myCam;
    [SerializeField] private float xmin;
    [SerializeField] private float xmax;

    private Vector3 _eulerAngles;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        input = GetComponent<PawnInput>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        _eulerAngles.x -= input._mouseY;
        _eulerAngles.x = Mathf.Clamp(_eulerAngles.x, xmin, xmax);
        myCam.eulerAngles = _eulerAngles;
        transform.Rotate(0.0f, input._mouseY, 0.0f, Space.World);
    }
}
