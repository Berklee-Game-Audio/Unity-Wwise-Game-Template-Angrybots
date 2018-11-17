using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {

    public Animator Animator;
    [Space(10)]
    public Transform SpineBone;
    public Vector3 SpineAdjustment;
    [Space(10)]
    public Transform HipsBone;
    public float RotationSpeed = 380;
    [Space(10)]
    public MeshRenderer Arrow;

    private PlayerController _PlayerController;
    private UnityEngine.AI.NavMeshAgent _NavMeshAgent;
    private Vector3 _PreviousPosition;
    private Vector3 _SpineForward;
    private bool _IsFacingBackwards;

    private Quaternion _SpineInitialRotation;

    private Vector3 _HipsRotation;
    private float _HipsDeltaAngle;
    private Coroutine _RotateBody;

    void Start() {
        _PlayerController = GetComponent<PlayerController>();
        _NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _PreviousPosition = transform.position;
        _SpineInitialRotation = SpineBone.localRotation;
        _HipsRotation = HipsBone.localRotation.eulerAngles;
    }

	// Update is called once per frame
	void Update () {
        Animator.SetBool("Shooting", _PlayerController.Shooting);
        Animator.SetBool("FacingForward", !_IsFacingBackwards);
        var speed = (transform.position - _PreviousPosition).magnitude / Time.deltaTime / _NavMeshAgent.speed;
        speed += _RotateBody != null ? 0.75f : 0;
        Animator.SetFloat("Speed", Mathf.Clamp01(speed));
        _PreviousPosition = transform.position;       
    }

    void LateUpdate() {
        UpdateBodyDirection();
        UpdateSpine();
    }

    private void UpdateSpine() {
            Quaternion rotation;
            _SpineForward = Vector3.Scale(_PlayerController.CursorWorldPosition - transform.position, new Vector3(1, 0, 1));
            rotation = Quaternion.LookRotation(_SpineForward);
            rotation *= Quaternion.Euler(SpineAdjustment);
            SpineBone.rotation = rotation;
    }

    private void UpdateBodyDirection() {
        var wasFacingBackwards = _IsFacingBackwards;
        _IsFacingBackwards = Vector3.Angle(_SpineForward, transform.forward) > 90;
        if(wasFacingBackwards != _IsFacingBackwards) {
            if (_RotateBody != null)
                StopCoroutine(_RotateBody);
            var throughRight = wasFacingBackwards ? !(Vector3.Dot(_SpineForward, transform.right) > 0) : Vector3.Dot(_SpineForward, transform.right) > 0;
            _RotateBody = StartCoroutine(RotateBody(throughRight));
        }

        HipsBone.localRotation = Quaternion.Euler(_HipsRotation + new Vector3(0, _HipsDeltaAngle, 0));
    }

    private IEnumerator RotateBody(bool throughRight) {

        if (!_IsFacingBackwards) {
            if (throughRight)
                _HipsDeltaAngle = -180;
            else
                _HipsDeltaAngle = 180;
        }
        

        while (_IsFacingBackwards && Mathf.Abs(_HipsDeltaAngle) != 180 || !_IsFacingBackwards && _HipsDeltaAngle != 0) {

            var deltaRot = RotationSpeed * Time.deltaTime;
            deltaRot *= throughRight ? 1 : -1;
            _HipsDeltaAngle += deltaRot;

            if (_IsFacingBackwards) {
                _HipsDeltaAngle = Mathf.Clamp(_HipsDeltaAngle, -180, 180);
            }
            else {
                _HipsDeltaAngle = Mathf.Sign(deltaRot) == Mathf.Sign(_HipsDeltaAngle) ? 0 : _HipsDeltaAngle;
            }

            yield return null;
        }
        _RotateBody = null;
    }
}
