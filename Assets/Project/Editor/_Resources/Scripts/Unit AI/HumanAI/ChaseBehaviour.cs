using UnityEngine;

public class ChaseBehaviour : StateMachineBehaviour
{
    PlayerController playerController;
    Vector3 aimingOffset = new Vector3(0f, 1.4f, 0f);

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController = animator.GetComponent<PlayerController>();
        if (animator.GetBool("EnableAI") && playerController.attackType != "hold")
        {
            playerController.TurnOffFiring();
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("EnableAI"))
        {
            if (playerController.unitVision.currentTarget != null)
            {
                if (!(Input.GetKey(KeyCode.LeftControl) && playerController.IsThisCurrentChar()))
                {
                    if (playerController.movementType != "hold")
                    {
                        if (playerController.agent.destination != playerController.unitVision.currentTarget.transform.position)
                        {
                            if (animator.GetBool("Lie") || animator.GetBool("Sit"))
                            {
                                playerController.poseToLie = false;
                                playerController.ChangeBodyPosition();
                            }
                            else
                            {
                                if (animator.GetBool("ChangePositionOver") && animator.GetBool("ThrowGrenadeOver") && !animator.GetBool("Lie") && !animator.GetBool("Sit"))
                                {
                                    playerController.MoveToPoint(playerController.unitVision.currentTarget.transform.position);
                                }
                            }
                        }
                    }
                    if (playerController.attackType != "hold")
                    {
                        // playerController.rig.transform.position = playerController.unitVision.currentTarget.transform.position + aimingOffset;
                        playerController.Aiming(playerController.unitVision.currentTarget.transform.position + aimingOffset, 0f, false);
                    }
                    else
                    {
                        playerController.TurnOffAiming();
                    }
                }

                float distance = Vector3.Distance(playerController.transform.position, playerController.unitVision.currentTarget.transform.position);

                if (((playerController.movementType == "free" && distance < playerController.selection.currentWeapon.effectiveDistance) || 
                    (playerController.movementType == "hold" && distance < playerController.selection.currentWeapon.effectiveDistance * 1.5f)) && playerController.unitVision.seeEnemy)
                {
                    animator.SetBool("AttackAI", true);
                }
                if (distance > playerController.selection.currentWeapon.effectiveDistance * 1.5f)
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
        if (animator.GetBool("EnableAI") && playerController.movementType != "hold")
        {
            playerController.agent.ResetPath();
        }
    }
}
