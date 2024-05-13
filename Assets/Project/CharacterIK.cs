using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterIK : MonoBehaviour
{
    public Animator animator;
    public Rig rig;
    public MultiAimConstraint head;
    public TwoBoneIKConstraint rightArm;
    public TwoBoneIKConstraint leftArm;
    public TwoBoneIKConstraint leftArmStand;
    public TwoBoneIKConstraint rightArmLieAim;
    public TwoBoneIKConstraint leftArmLieAim;
    public MultiAimConstraint body;
    public MultiAimConstraint weapon;

    void Start()
    {
    }

    void Update()
    {
    }

    private void OnAnimatorIK()
    {
        // SelectCurrentWeapon(); // Remake

        bool isBusy = !animator.GetBool("ReloadOver") || !animator.GetBool("TakeWeaponOver") || !animator.GetBool("ThrowGrenadeOver");
        var condition = 0;
        if (isBusy)
            condition = 1;
        if ((animator.GetBool("Aim") || animator.GetBool("Fire")) && !isBusy) //  && !animator.GetBool("Lie")
        {
            if (head.weight == 0)
                StartCoroutine(UpdateWeight(0, 1, isBusy));
        }
        else
        {
            if (head.weight == 1 || leftArmStand.weight == condition)
                StartCoroutine(UpdateWeight(1, 0, isBusy));
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

    IEnumerator UpdateWeight(float start, float end, bool isReload)
    {
        float elapsedTime = 0;
        var condition = start;
        if (isReload)
            condition = end;
        while (head.weight != end || leftArmStand.weight != condition)
        {
            if (isReload)
            {
                leftArmStand.weight = Mathf.Lerp(start, end, (elapsedTime / 0.3f));
                leftArm.weight = Mathf.Lerp(start, end, (elapsedTime / 0.3f));
            }
            else
            {
                leftArmStand.weight = Mathf.Lerp(end, start, (elapsedTime / 0.3f));
                leftArm.weight = Mathf.Lerp(start, end, (elapsedTime / 0.3f));
            }
            head.weight = Mathf.Lerp(start, end, (elapsedTime / 0.3f));
            rightArm.weight = Mathf.Lerp(start, end, (elapsedTime / 0.3f));
            if (animator.GetBool("Lie") && (animator.GetBool("Aim") || animator.GetBool("Fire")))
            {
                body.weight = Mathf.Lerp(0, 0, (elapsedTime / 0.3f));
                rightArmLieAim.weight = Mathf.Lerp(0, 1, (elapsedTime / 0.3f));
                leftArmLieAim.weight = Mathf.Lerp(0, 1, (elapsedTime / 0.3f));
            }
            else
            {
                if (animator.GetBool("Lie"))
                {
                    body.weight = Mathf.Lerp(0, 0, (elapsedTime / 0.3f));
                }
                else
                {
                    body.weight = Mathf.Lerp(start, end, (elapsedTime / 0.3f));
                }
                rightArmLieAim.weight = Mathf.Lerp(0, 0, (elapsedTime / 0.3f));
                leftArmLieAim.weight = Mathf.Lerp(0, 0, (elapsedTime / 0.3f));
            }
            weapon.weight = Mathf.Lerp(start, end, (elapsedTime / 0.3f));

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void SelectCurrentWeapon()
    {
        rightArm.data.target = RecursiveFindChild(transform, "RightHandAim_target");
        rightArm.data.hint = RecursiveFindChild(transform, "RightHandAim_hint");
        leftArm.data.target = RecursiveFindChild(transform, "LeftHandAim_target");
        leftArm.data.hint = RecursiveFindChild(transform, "LeftHandAim_hint");
        leftArmStand.data.target = RecursiveFindChild(transform, "LeftHandStand_target");
        leftArmStand.data.hint = RecursiveFindChild(transform, "LeftHandStand_hint");
        rightArmLieAim.data.target = RecursiveFindChild(transform, "RightHandLieAim_target");
        rightArmLieAim.data.hint = RecursiveFindChild(transform, "RightHandLieAim_hint");
        leftArmLieAim.data.target = RecursiveFindChild(transform, "LeftHandLieAim_target");
        leftArmLieAim.data.hint = RecursiveFindChild(transform, "LeftHandLieAim_hint");
        weapon.data.constrainedObject = RecursiveFindChild(transform, "WeaponHolder").transform.GetChild(0);
    }
}
