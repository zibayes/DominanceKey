using UnityEngine;

public class IdleBehabiour : StateMachineBehaviour
{
    PlayerController playerController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController = animator.GetComponent<PlayerController>();
        if (animator.GetBool("EnableAI"))
        {
            playerController.agent.ResetPath();
            playerController.TurnOffAiming();
            if (playerController.selection.currentWeapon.currentAmmo <= playerController.selection.currentWeapon.magSize * 0.5f)
            {
                playerController.Reload(false);
            }

            if (playerController.movementType != "hold")
            {
                if (animator.GetBool("Lie") || animator.GetBool("Sit"))
                {
                    playerController.poseToLie = false;
                    playerController.ChangeBodyPosition();
                }
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
    {
        if (animator.GetBool("EnableAI"))
        {
            if (playerController.unitVision.seeEnemy)
            {
                animator.SetBool("ChaseAI", true);
            }
            if (playerController.selection.health <= playerController.selection.maxHealth * 0.75f)
            {
                playerController.HealSelf();
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
