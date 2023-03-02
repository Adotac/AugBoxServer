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

    public override void OnStartClient()
    {
        base.OnStartClient();

        myCam.GetComponent<Camera>().enabled = IsOwner;
        myCam.GetComponent<AudioListener>().enabled = IsOwner;
    }

    private void Update()
    {
        if (!IsOwner) return;

        _eulerAngles.x -= input._mouseY;
        _eulerAngles.x = Mathf.Clamp(_eulerAngles.x, xmin, xmax);
        myCam.localEulerAngles = _eulerAngles;
        transform.Rotate(0.0f, input._mouseX, 0.0f, Space.World);
    }
}
