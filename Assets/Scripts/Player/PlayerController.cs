using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public static PlayerController LocalPlayer;

    // Objects to drag in
    public GameObject arrowPrefab;
    public GameObject cursorPrefab;
    public GameObject joystickPrefab;

    // Settings
    public float cameraSmoothing = 0.01f;
    public float cameraPreview = 2.0f;

    // Cursor settings
    public float cursorPlaneHeight = 0;
    public float cursorFacingCamera = 0;
    public float cursorSmallerWithDistance = 0;
    public float cursorSmallerWhenClose = 1;

    [SyncVar]
    public bool Shooting;
    [SyncVar]
    public Vector3 CursorWorldPosition;

    public Transform CursorObject { get; private set; }

    // Private memeber data
    private UnityEngine.AI.NavMeshAgent _NavMeshAgent;
    private Camera mainCamera;

    private Joystick joystickLeft;

    private Transform mainCameraTransform;
    private Vector3 cameraVelocity = Vector3.zero;
    private Vector3 cameraOffset = Vector3.zero;
    public Vector3 initOffsetToPlayer;

    // Prepare a cursor point varibale. This is the mouse position on PC and controlled by the thumbstick on mobiles.
    private Vector3 cursorScreenPosition;

    private Plane playerMovementPlane;

    private GameObject joystickRightGO;

    private Quaternion screenMovementSpace;
    private Vector3 screenMovementForward;
    private Vector3 screenMovementRight;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        LocalPlayer = this;
    }


    void Start() {

        // Set main camera
        mainCamera = Camera.main;
        mainCameraTransform = mainCamera.transform;

        // Set the initial cursor position to the center of the screen
        cursorScreenPosition = new Vector3(0.5f * Screen.width, 0.5f * Screen.height, 0);

        // caching movement plane
        playerMovementPlane = new Plane(transform.up, transform.up * cursorPlaneHeight);

        _NavMeshAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
        	    if (joystickPrefab) {
        	        // Create left joystick
        	        GameObject joystickLeftGO = Instantiate (joystickPrefab) as GameObject;
        	        joystickLeftGO.name = "Joystick Left";
        	        joystickLeft = joystickLeftGO.GetComponent<Joystick>();	
        	    }
#elif !UNITY_FLASH
        if (isLocalPlayer) {
            if (cursorPrefab) {
                CursorObject = (Instantiate(cursorPrefab) as GameObject).transform;
            }
        }
#endif

        // it's fine to calculate this on Start () as the camera is static in rotation

        screenMovementSpace = Quaternion.Euler(0, mainCameraTransform.eulerAngles.y, 0);
        screenMovementForward = screenMovementSpace * Vector3.forward;
        screenMovementRight = screenMovementSpace * Vector3.right;
    }

    void OnDisable() {
        if (joystickLeft)
            joystickLeft.enabled = false;
    }

    void OnEnable() {
        if (joystickLeft)
            joystickLeft.enabled = true;
    }



    void Update() {

        // HANDLE CHARACTER MOVEMENT DIRECTION


        //
        //	    motor.movementDirection = joystickLeft.position.x * screenMovementRight + joystickLeft.position.y * screenMovementForward;
        //#else

        if (isLocalPlayer) {
            if (Input.GetMouseButton(0)) {
                CmdStartShooting(true);
            }
            else {
                CmdStartShooting(false);
            }

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
            if (Input.GetMouseButtonDown(0)) {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                    MoveTo(hit.point);
            }
#else
             if (Input.GetMouseButtonDown(1)) {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                    MoveTo(hit.point);
            }
#endif
        }
        //#endif

        //        // Make sure the direction vector doesn't exceed a length of 1
        //        // so the character can't move faster diagonally than horizontally or vertically
        //        if (motor.movementDirection.sqrMagnitude > 1)
        //            motor.movementDirection.Normalize();


        //        // HANDLE CHARACTER FACING DIRECTION AND SCREEN FOCUS POINT

        //        // First update the camera position to take into account how much the character moved since last frame
        //        //mainCameraTransform.position = Vector3.Lerp (mainCameraTransform.position, character.position + cameraOffset, Time.deltaTime * 45.0ff * deathSmoothoutMultiplier);

        //        // Set up the movement plane of the character, so screenpositions
        //        // can be converted into world positions on this plane
        //        //playerMovementPlane = new Plane (Vector3.up, character.position + character.up * cursorPlaneHeight);

        //        // optimization (instead of newing Plane):

        //        playerMovementPlane.normal = character.up;
        //        playerMovementPlane.distance = -character.position.y + cursorPlaneHeight;

        //        // used to adjust the camera based on cursor or joystick position

        Vector3 cameraAdjustmentVector = Vector3.zero;

#if !UNITY_EDITOR && (UNITY_XBOX360 || UNITY_PS3)

        // On consoles use the analog sticks
        float axisX = Input.GetAxis("LookHorizontal");
        float axisY = Input.GetAxis("LookVertical");
        motor.facingDirection = axisX * screenMovementRight + axisY * screenMovementForward;

        cameraAdjustmentVector = (_NavMeshAgent.destination - transform.position).normalized;

#else

        // On PC, the cursor point is the mouse 
        if (isLocalPlayer) {
            cursorScreenPosition = Input.mousePosition;
            CursorWorldPosition = ScreenPointToWorldPointOnPlane(cursorScreenPosition, playerMovementPlane, mainCamera);
            CmdSetCursorWorldPosition(CursorWorldPosition);

            float halfWidth = Screen.width / 2.0f;
            float halfHeight = Screen.height / 2.0f;
            float maxHalf = Mathf.Max(halfWidth, halfHeight);

            // Acquire the relative screen position			
            Vector3 posRel = cursorScreenPosition - new Vector3(halfWidth, halfHeight, cursorScreenPosition.z);
            posRel.x /= maxHalf;
            posRel.y /= maxHalf;

            cameraAdjustmentVector = posRel.x * screenMovementRight + posRel.y * screenMovementForward;
            cameraAdjustmentVector.y = 0.0f;


            // Draw the cursor nicely
            HandleCursorAlignment(CursorWorldPosition);
        }
#endif

        //        // HANDLE CAMERA POSITION
        if (isLocalPlayer) {
            //        // Set the target position of the camera to point at the focus point
            Vector3 cameraTargetPosition = transform.position + initOffsetToPlayer + cameraAdjustmentVector * cameraPreview;

            //        // Apply some smoothing to the camera movement
            mainCameraTransform.position = Vector3.SmoothDamp(mainCameraTransform.position, cameraTargetPosition, ref cameraVelocity, cameraSmoothing);

            //        // Save camera offset so we can use it in the next frame
            cameraOffset = mainCameraTransform.position - transform.position;
        }
    }


    public void MoveTo(Vector3 target) {
        _NavMeshAgent.SetDestination(target);
        CmdMoveTo(target);
    }

    [Command]
    public void CmdMoveTo(Vector3 target) {
        _NavMeshAgent.SetDestination(target);
        RpcMoveTo(target);
    }

    [ClientRpc]
    public void RpcMoveTo(Vector3 target) {
        _NavMeshAgent.SetDestination(target);
    } 

    [Command]
    public void CmdFire(Vector3 forward, Vector3 position) {
            var rotation = Quaternion.LookRotation(Vector3.Scale(forward, new Vector3(1, 0, 1)));
            var arrowObject = Instantiate(arrowPrefab, position, rotation) as GameObject;
            NetworkServer.Spawn(arrowObject);
            //arrowObject.GetComponent<Arrow>().ArrowHost = this.gameObject;
    }

    [Command]
    public void CmdStartShooting(bool shooting) {
        Shooting = shooting;
    }

    [Command]
    public void CmdSetCursorWorldPosition(Vector3 position) {
        CursorWorldPosition = position;
    }

    public static Vector3 PlaneRayIntersection(Plane plane, Ray ray) {
        float dist;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }

    public static Vector3 ScreenPointToWorldPointOnPlane(Vector3 screenPoint, Plane plane, Camera camera) {
        // Set up a ray corresponding to the screen position
        Ray ray = camera.ScreenPointToRay(screenPoint);

        // Find out where the ray intersects with the plane
        return PlaneRayIntersection(plane, ray);
    }

    void HandleCursorAlignment(Vector3 cursorWorldPosition) {
        if (!CursorObject)
            return;

        // HANDLE CURSOR POSITION
        // Set the position of the cursor object
        CursorObject.position = cursorWorldPosition;

#if !UNITY_FLASH
        // Hide mouse cursor when within screen area, since we're showing game cursor instead
        Cursor.visible = (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height);
#endif


        // HANDLE CURSOR ROTATION

        Quaternion cursorWorldRotation = CursorObject.rotation;
        var facingDirection = cursorWorldPosition - transform.position;
        if (facingDirection != Vector3.zero)
            cursorWorldRotation = Quaternion.LookRotation(facingDirection);

        // Calculate cursor billboard rotation
        Vector3 cursorScreenspaceDirection = Input.mousePosition - mainCamera.WorldToScreenPoint(transform.position + transform.up * cursorPlaneHeight);
        cursorScreenspaceDirection.z = 0;
        Quaternion cursorBillboardRotation = mainCameraTransform.rotation * Quaternion.LookRotation(cursorScreenspaceDirection, -Vector3.forward);

        // Set cursor rotation
        CursorObject.rotation = Quaternion.Slerp(cursorWorldRotation, cursorBillboardRotation, cursorFacingCamera);


        // HANDLE CURSOR SCALING

        // The cursor is placed in the world so it gets smaller with perspective.
        // Scale it by the inverse of the distance to the camera plane to compensate for that.
        float compensatedScale = 0.1f * Vector3.Dot(cursorWorldPosition - mainCameraTransform.position, mainCameraTransform.forward);

        // Make the cursor smaller when close to character
        float cursorScaleMultiplier = Mathf.Lerp(0.7f, 1.0f, Mathf.InverseLerp(0.5f, 4.0f, facingDirection.magnitude));

        // Set the scale of the cursor
        CursorObject.localScale = Vector3.one * Mathf.Lerp(compensatedScale, 1, cursorSmallerWithDistance) * cursorScaleMultiplier;

        // DEBUG - REMOVE LATER
        if (Input.GetKey(KeyCode.O)) cursorFacingCamera += Time.deltaTime * 0.5f;
        if (Input.GetKey(KeyCode.P)) cursorFacingCamera -= Time.deltaTime * 0.5f;
        cursorFacingCamera = Mathf.Clamp01(cursorFacingCamera);

        if (Input.GetKey(KeyCode.K)) cursorSmallerWithDistance += Time.deltaTime * 0.5f;
        if (Input.GetKey(KeyCode.L)) cursorSmallerWithDistance -= Time.deltaTime * 0.5f;
        cursorSmallerWithDistance = Mathf.Clamp01(cursorSmallerWithDistance);
    }

}