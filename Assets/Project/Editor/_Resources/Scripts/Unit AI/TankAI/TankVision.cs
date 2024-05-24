using UnityEngine;

public class TankVision : MonoBehaviour
{
    public TankController tankController;
    public string targetTag = "Player";
    public int rays = 8;
    public int distance = 33;
    public float angle = 40;
    // public Vector3 offset;
    // public GameObject[] targets;
    public bool seeEnemy = false;
    public PlayerController currentTarget;

    void Start()
    {
        // targets = GameObject.FindGameObjectsWithTag(targetTag);
    }

    bool GetRaycast(Vector3 dir)
    {
        bool result = false;
        RaycastHit hit;
        // Vector3 pos = transform.position + offset;
        var offset = transform.forward * 0.25f + transform.up;
        Vector3 pos = transform.position + offset;
        if (Physics.Raycast(pos, dir, out hit, distance))
        {
            var obj = hit.transform.GetComponentInParent<PlayerController>();
            if (obj != null)
            {
                if (tankController.selectManager.selectableChars.Contains(obj.selection))
                {
                    if (tankController.selection.player != obj.selection.player)
                    {
                        result = true;
                        if (currentTarget == null)
                        {
                            currentTarget = obj;
                        }
                        else if (currentTarget != obj)
                        {
                            if (Vector3.Distance(currentTarget.transform.position, transform.position) > Vector3.Distance(obj.transform.position, transform.position))
                            {
                                currentTarget = obj;
                            }
                        }
                        Debug.DrawLine(pos, hit.point, Color.green);
                    }
                    else
                    {
                        Debug.DrawLine(pos, hit.point, Color.yellow);
                    }
                }
                else
                {
                    Debug.DrawLine(pos, hit.point, Color.blue);
                }
            }
            else
            {
                Debug.DrawLine(pos, hit.point, Color.blue);
            }
        }
        else
        {
            Debug.DrawRay(pos, dir * distance, Color.red);
        }
        return result;
    }

    bool RayToScan()
    {
        bool result = false;
        bool a = false;
        bool b = false;
        float j = 0;
        for (int i = 0; i < rays; i++)
        {
            var x = Mathf.Sin(j);
            var y = Mathf.Cos(j);

            j += angle * Mathf.Deg2Rad / rays;

            Vector3 dir = transform.TransformDirection(new Vector3(x, 0, y));
            if (GetRaycast(dir)) a = true;

            if (x != 0)
            {
                dir = transform.TransformDirection(new Vector3(-x, 0, y));
                if (GetRaycast(dir)) b = true;
            }
        }

        if (a || b) result = true;
        return result;
    }

    void Update()
    {
        //if (Vector3.Distance(transform.position, target.position) < distance) // Вариант оптимизации...
        {
            if (RayToScan())
            {
                seeEnemy = true;   // Контакт с целью
            }
            else
            {
                seeEnemy = false;  // Поиск цели...
                if (currentTarget == null)
                {
                    currentTarget = tankController.noiseDetectionTarget;
                }
                else if(!Input.GetKey(KeyCode.LeftControl) && !tankController.UIController.isActiveManualControl)
                {
                    tankController.Rotate(currentTarget.transform.position);
                }
            }
            if (currentTarget != null)
            {
                if (currentTarget.selection.health <= 0)
                    currentTarget = null;
            }
        }
    }
}