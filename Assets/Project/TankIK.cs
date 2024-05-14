using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class TankIK : MonoBehaviour
{
    public TankController tankController;
    public Animator animator;
    public Rig rig;
    public MultiAimConstraint Mantlet;
    public MultiAimConstraint Turret;

    private void Start()
    {
        animator = tankController.animator;
    }

    private void OnAnimatorIK()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000))
        {
            if (tankController.rig.transform.position != hit.point)
            {
                if (Turret.weight == 1)
                    StartCoroutine(UpdateWeight(0, 1));
            }
        }
    }

    Transform RecursiveFindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            else
            {
                Transform found = RecursiveFindChild(child, childName);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }

    IEnumerator UpdateWeight(float start, float end)
    {
        float elapsedTime = 0;
        while (Turret.weight != end)
        {
            Turret.weight = Mathf.Lerp(start, end, (elapsedTime / 0.1f));
            Mantlet.weight = Mathf.Lerp(start, end, (elapsedTime / 0.1f));

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
