using UnityEngine;

public class TankIdleBehabiour : StateMachineBehaviour
{
    TankController tankController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        tankController = animator.GetComponent<TankController>();
        if (animator.GetBool("EnableAI") && tankController.getAnyCrewmate() != null)
        {
            tankController.agent.ResetPath();
            if (tankController.attackType != "hold")
            {
                if (ReferenceEquals(tankController.currentWeapon, tankController.mainGun))
                {
                    tankController.ReloadMainGun(false, tankController.mainGun);
                }
                else if (tankController.currentWeapon.currentAmmo <= tankController.currentWeapon.magSize * 0.5f)
                {
                    tankController.ReloadMgun(false, tankController.pairedMgun);
                }
            }
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
    {
        if (animator.GetBool("EnableAI") && tankController.getAnyCrewmate() != null)
        {
            if (tankController.tankVision.seeEnemy)
            {
                animator.SetBool("ChaseAI", true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
