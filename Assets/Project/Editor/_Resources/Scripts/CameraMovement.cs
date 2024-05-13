using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    public SelectManager selectManager;

    private int screenWidth;
    private int screenHeight;

    public float speed;
    public float scrollSpeed;
    public bool useCameraMovement;

    public float movementCameraVelocity = 0.02f;
    public float sensitivity = 1.5f;
    public float smoothing = 1.0f;
    private float X, Y;

    private float mapBorderX = 20f;
    private float mapBorderZ = 30f;

    // Start is called before the first frame update
    void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        selectManager = GameObject.Find("SelectingBox").GetComponent<SelectManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Camera position Controls
        Vector3 camPos = new Vector3(0, 0, 0);

        if (Input.mousePosition.x <= 20)
        {
            camPos.x -= Time.deltaTime * speed;
        }
        else if (Input.mousePosition.x >= screenWidth - 20)
        {
            camPos.x += Time.deltaTime * speed;
        }
        else if (Input.mousePosition.y <= 20)
        {
            camPos.z -= Time.deltaTime * speed;
        }
        else if (Input.mousePosition.y >= screenHeight - 20)
        {
            camPos.z += Time.deltaTime * speed;
        }

        // transform.position = new Vector3(Mathf.Clamp(camPos.x, -20f, 20f), camPos.y, Mathf.Clamp(camPos.z, -30f, 30f));
        float yCoord = transform.position.y;
        transform.Translate(new Vector3(camPos.x, camPos.y, camPos.z));
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -mapBorderX, mapBorderX), yCoord, Mathf.Clamp(transform.position.z, -mapBorderZ, mapBorderZ));

        if (Input.GetKey(KeyCode.LeftControl) && selectManager.selectedArmy.Any())
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            /*
            var soldier = selectManager.selectedArmy[0].transform.GetComponentInParent<PlayerController>();
            var directionVector = Camera.main.transform.right * h + Camera.main.transform.forward * v;
            var directionVector = Vector3.ClampMagnitude(soldier.directionVector, 1) * soldier.speed * 1.5f;
            transform.position += directionVector;
            transform.position += new Vector3(Mathf.Clamp(h, -movementCameraVelocity, movementCameraVelocity), 0, Mathf.Clamp(v, -movementCameraVelocity, movementCameraVelocity));
            */
            
            var soldier = selectManager.selectedArmy[0].selfRigidbody;
            transform.position += Vector3.ClampMagnitude(soldier.velocity, 0.1f);
        }

        // Camera angle Controls
        if (Input.GetMouseButton(2))
        {
            X = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
            Y += Input.GetAxis("Mouse Y") * sensitivity;
            Y = Mathf.Clamp(Y, -65, -10);
            transform.localEulerAngles = new Vector3(-Y, X, 0);
        }

        // Camera zoom Controls
        Vector3 camPosZoom = new Vector3(0, 0, 0);
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && !IsMouseOverUI())
        {
            camPosZoom.z += Time.deltaTime * scrollSpeed;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && !IsMouseOverUI())
        {
            camPosZoom.z -= Time.deltaTime * scrollSpeed;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0 && !IsMouseOverUI())
        {
            float xPos = transform.position.x;
            float zPos = transform.position.z;
            transform.Translate(new Vector3(camPosZoom.x, camPosZoom.y, Mathf.Clamp(camPosZoom.z, -40f, 40f)));
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            /*
            float yPos = Mathf.Clamp(transform.position.y, 12, 30);
            if (yPos <= 12 || yPos >= 30)
            {
                transform.position = new Vector3(xPos, yPos, zPos);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            }
            */
        }
        

        // Camera Raycasting [PART OF DPO TASK]
        /*
        if (Input.GetMouseButtonDown(3))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit))
            {
                if (hit.collider.gameObject.GetComponent<Actor>())
                {
                    if (GameObject.FindGameObjectWithTag("Light").GetComponent<Light>().enabled)
                        hit.collider.gameObject.GetComponent<Actor>().OffLight();
                    else
                        hit.collider.gameObject.GetComponent<Actor>().OnLight();
                }
            }
        }
        */
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
