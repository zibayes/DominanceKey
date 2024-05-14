using UnityEngine;
using UnityEngine.AI;
using VariableCode.Cursor;

public class TankController : MonoBehaviour
{
    public SelectManager selectManager;
    public SampleScene inventoryManager;
    public GameObject inventory;
    public UIController UIController;
    public CursorSwitcher cursorSwitcher;

    public Animator animator;
    public Rigidbody rigidbody;
    public NavMeshAgent agent;
    public SelectableCharacter selection;
    public float roatationSpeed = 0.7f;
    public float speed = 6f;
    public Vector3 directionVector;
    public float nextShoot = 0f;
    public LayerMask whatCanBeClickedOn;

    public Transform canvas;
    public Transform cameraRig;

    void Start()
    {
        selectManager = GameObject.Find("SelectingBox").GetComponent<SelectManager>();
        inventoryManager = GameObject.Find("CanvasParent").GetComponent<SampleScene>();
        inventory = GameObject.Find("Inventory");
        UIController = GameObject.Find("UIController").GetComponent<UIController>();
        canvas = GameObject.Find("Canvas").transform;
        cameraRig = GameObject.Find("CameraRig").transform;
        cursorSwitcher = GameObject.Find("CursorManager").GetComponent<CursorSwitcher>();
    }
    void Update()
    {
        if ((Input.GetKey(KeyCode.LeftControl) || UIController.isActiveManualControl))
        {
            // Calculate direction vector
            if (agent.remainingDistance == 0)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                /*
                GameObject direction = new GameObject();
                direction.transform.position = cameraRig.position;
                direction.transform.localEulerAngles = new Vector3(0, cameraRig.localEulerAngles.y, cameraRig.localEulerAngles.z);
                directionVector = direction.transform.right * h + direction.transform.forward * v;
                DestroyImmediate(direction);
                */
                directionVector = transform.forward * v;
                directionVector.y = 0;
                transform.Rotate(0, h * roatationSpeed, 0, Space.Self);
                // animator.SetFloat("Speed", Vector3.ClampMagnitude(directionVector, 1).magnitude);
                if (Vector3.ClampMagnitude(directionVector, 1).magnitude > 0)
                {
                    rigidbody.velocity = Vector3.ClampMagnitude(directionVector, 1) * speed * 1.5f;
                    // animator.SetBool("Move", true);
                    // firingAimDecrease += 0.15f;
                }
                else
                {
                    rigidbody.velocity = Vector3.zero;
                }

                var localCoords = transform.InverseTransformDirection(directionVector);
                // animator.SetFloat("h", localCoords.z);
                // animator.SetFloat("v", localCoords.x);
            }
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
        }            
    }
}
