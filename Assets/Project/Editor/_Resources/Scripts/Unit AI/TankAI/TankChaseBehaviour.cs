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
            if (tankController.tankVision.currentTarget != null)
            {
                if (!(Input.GetKey(KeyCode.LeftControl) && tankController.IsThisCurrentChar()))
                {
                    if (tankController.movementType != "hold")
                    {
                        if (tankController.agent.destination != tankController.tankVision.currentTarget.transform.position)
                        {
                            tankController.MoveToPoint(tankController.tankVision.currentTarget.transform.position);
                        }
                    }
                    if (tankController.attackType != "hold")
                    {
                        tankController.Rotate(tankController.tankVision.currentTarget.transform.position + aimingOffset);
                    }
                }

                float distance = Vector3.Distance(tankController.transform.position, tankController.tankVision.currentTarget.transform.position);

                if (((tankController.movementType == "free" && distance < tankController.currentWeapon.effectiveDistance) || 
                    (tankController.movementType == "hold" && distance < tankController.currentWeapon.effectiveDistance * 1.5f)) && tankController.tankVision.seeEnemy)
                {
                    animator.SetBool("AttackAI", true);
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
