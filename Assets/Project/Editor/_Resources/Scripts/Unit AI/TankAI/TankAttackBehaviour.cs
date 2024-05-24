using UnityEngine;

public class TankAttackBehaviour : StateMachineBehaviour
{
    TankController tankController;

    int burstSize;
    float nextBurst = 0;
    float nextBurstTime;
    float spreadSize;

    Vector3 aimingOffsetStandTarget = new Vector3(0f, 1.4f, 0f);
    Vector3 aimingOffsetSitTarget = new Vector3(0f, 0.7f, 0f);
    Vector3 aimingOffsetLieTarget = new Vector3(0f, 0.2f, 0f);

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        tankController = animator.GetComponent<TankController>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("EnableAI") && tankController.getAnyCrewmate() != null)
        {
            if (tankController.tankVision.currentTarget != null && tankController.tankVision.seeEnemy)
            {
                float distance = Vector3.Distance(tankController.transform.position, tankController.tankVision.currentTarget.transform.position);
                var currentWeapon = tankController.currentWeapon;
                if ((tankController.movementType == "free" && distance >= currentWeapon.effectiveDistance) || (tankController.movementType == "hold" && distance >= currentWeapon.effectiveDistance * 1.5f))
                {
                    animator.SetBool("AttackAI", false);
                    animator.SetBool("ChaseAI", true);
                }
                else
                {
                    if (!(Input.GetKey(KeyCode.LeftControl) && !tankController.UIController.isActiveManualControl && tankController.IsThisCurrentChar()))
                    {
                        if (tankController.attackType != "hold")
                        {
                            var currentOffset = aimingOffsetStandTarget;
                            if (tankController.tankVision.currentTarget.animator.GetBool("Sit"))
                                currentOffset = aimingOffsetSitTarget;
                            else if (tankController.tankVision.currentTarget.animator.GetBool("Lie"))
                                currentOffset = aimingOffsetLieTarget;
                            tankController.Rotate(tankController.tankVision.currentTarget.transform.position + currentOffset);

                            if (burstSize == 0)
                            {
                                burstSize = tankController.pairedMgun.burstSize + Random.Range(-tankController.pairedMgun.burstSizeDelta, tankController.pairedMgun.burstSizeDelta);
                                nextBurstTime = tankController.pairedMgun.nextBurstTime + Random.Range(-tankController.pairedMgun.nextBurstTimeDelta, tankController.pairedMgun.nextBurstTimeDelta);
                                nextBurst = Time.time + nextBurstTime;
                            }

                            tankController.currentWeapon = tankController.mainGun;
                            spreadSize = currentWeapon.effectiveDistance / tankController.spreadSize - tankController.firingAimDecrease * tankController.spreadSize;
                            if (tankController.ShootMainGun(spreadSize, false))
                            {
                                tankController.ReloadMainGun(false);
                            }

                            if (nextBurst < Time.time && burstSize > 0)
                            {
                                tankController.currentWeapon = tankController.pairedMgun;
                                spreadSize = currentWeapon.effectiveDistance / tankController.spreadSize - tankController.firingAimDecrease * tankController.spreadSize;
                                if (tankController.ShootMgun(spreadSize, false))
                                {
                                    burstSize--;
                                }
                            }

                            if (tankController.pairedMgun.currentAmmo == 0)
                            {
                                tankController.ReloadMgun(false);
                                burstSize = 0;
                            }
                        }
                    }
                }
            }
            else
            {
                animator.SetBool("AttackAI", false);
                animator.SetBool("ChaseAI", true);
            }
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
