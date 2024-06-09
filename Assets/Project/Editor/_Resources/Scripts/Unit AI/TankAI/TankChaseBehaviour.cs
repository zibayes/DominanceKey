using UnityEngine;

public class TankChaseBehaviour : StateMachineBehaviour
{
    TankController tankController;
    Vector3 aimingOffset = new Vector3(0f, 1.4f, 0f);

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        tankController = animator.GetComponent<TankController>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("EnableAI") && tankController.getAnyCrewmate() != null)
        {
            Transform targetTransform = null;
            if (tankController.tankVision.currentTankTarget != null)
                targetTransform = tankController.tankVision.currentTankTarget.transform;
            else if (tankController.tankVision.currentTarget != null)
                targetTransform = tankController.tankVision.currentTarget.transform;
            if (targetTransform != null)
            {
                if (!(Input.GetKey(KeyCode.LeftControl) && tankController.IsThisCurrentChar()))
                {
                    if (tankController.movementType != "hold")
                    {
                        if (tankController.agent.destination != targetTransform.position)
                        {
                            if (tankController.getDriver() == null)
                                tankController.setDriver(tankController.getCrewmateWithLowestPriority());
                            if (tankController.getDriver() != null)
                            {
                                tankController.MoveToPoint(targetTransform.position);
                            }
                        }
                    }
                    if (tankController.attackType != "hold")
                    {
                        tankController.Rotate(targetTransform.position + aimingOffset);
                    }
                }

                float distance = Vector3.Distance(tankController.transform.position, targetTransform.position);

                if (((tankController.movementType == "free" && distance < tankController.currentWeapon.effectiveDistance) || 
                    (tankController.movementType == "hold" && distance < tankController.currentWeapon.effectiveDistance * 1.5f)) && tankController.tankVision.seeEnemy)
                {
                    animator.SetBool("AttackAI", true);
                    animator.SetBool("ChaseAI", false);
                }
                if (distance > tankController.currentWeapon.effectiveDistance * 1.5f)
                {
                    animator.SetBool("ChaseAI", false);
                }
            }
            else
            {
                animator.SetBool("ChaseAI", false);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("EnableAI") && tankController.movementType != "hold" && tankController.getAnyCrewmate() != null)
        {
            tankController.agent.ResetPath();
        }
    }
}
