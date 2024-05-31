using UnityEngine;

public class AttackBehaviour : StateMachineBehaviour
{
    PlayerController playerController;

    int burstSize;
    float nextBurst = 0;
    float nextBurstTime;

    Vector3 aimingOffsetStandTarget = new Vector3(0f, 1.4f, 0f);
    Vector3 aimingOffsetSitTarget = new Vector3(0f, 0.7f, 0f);
    Vector3 aimingOffsetLieTarget = new Vector3(0f, 0.2f, 0f);

    float changePositionChance = 10f;
    float changePositionCooldown = 5f;
    float changePositionTime = 0f;

    float throwGrenadeChance = 4f;
    float throwGrenadeCooldown = 12f;
    float throwGrenadeTime = 0f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController = animator.GetComponent<PlayerController>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("EnableAI"))
        {
            if (playerController.unitVision.currentTarget != null && playerController.unitVision.seeEnemy)
            {
                float distance = Vector3.Distance(playerController.transform.position, playerController.unitVision.currentTarget.transform.position);
                var currentWeapon = playerController.selection.currentWeapon;
                if ((playerController.movementType == "free" && distance >= currentWeapon.effectiveDistance) || (playerController.movementType == "hold" && distance >= currentWeapon.effectiveDistance * 1.5f))
                {
                    animator.SetBool("AttackAI", false);
                    animator.SetBool("ChaseAI", true);
                }
                else
                {
                    if (!(Input.GetKey(KeyCode.LeftControl) && !playerController.UIController.isActiveManualControl && playerController.IsThisCurrentChar()))
                    {
                        if (playerController.attackType != "hold")
                        {
                            if (playerController.movementType != "hold")
                            {
                                if (Time.time > changePositionTime && !animator.GetBool("Lie") && !animator.GetBool("Sit"))
                                {
                                    if (changePositionChance >= Random.Range(0, 100))
                                    {
                                        playerController.ChangeBodyPosition();
                                    }
                                    changePositionTime = Time.time + changePositionCooldown + Random.Range(-2, 2);
                                }
                            }
                            if (Time.time > throwGrenadeTime && Vector3.Distance(playerController.weaponHolder.position, playerController.unitVision.currentTarget.transform.position) <= 18.5f)
                            {
                                if (throwGrenadeChance >= Random.Range(0, 100))
                                {
                                    for (int i = 0; i < playerController.selection.inventory_items.Count; i++)
                                    {
                                        if (playerController.selection.inventory_items[i].type == "grenade")
                                        {
                                            playerController.selection.currentGrenade = playerController.selection.inventory_items[i];
                                            break;
                                        }
                                    }
                                    if (playerController.selection.currentGrenade != null)
                                    {
                                        playerController.TurnOffFiring();
                                        playerController.TurnOffAiming();
                                        var flightTime = (playerController.weaponHolder.position - playerController.unitVision.currentTarget.transform.position).magnitude * 0.2f;
                                        var throwForceGrenade = -(playerController.weaponHolder.position + Physics.gravity * Mathf.Pow(flightTime, 2f) / 2f -
                                            playerController.unitVision.currentTarget.transform.position) / flightTime;
                                        playerController.ThrowGrenade(throwForceGrenade, false);
                                    }
                                }
                                throwGrenadeTime = Time.time + throwGrenadeCooldown + Random.Range(-5, 5);
                            }
                            else
                            {
                                if (animator.GetBool("ChangePositionOver") && !playerController.isUsingWeapon())
                                {
                                    float spreadSize = playerController.effectiveDistance / playerController.currentSpread - playerController.firingAimDecrease * playerController.currentSpread;
                                    var currentOffset = aimingOffsetStandTarget;
                                    if (playerController.unitVision.currentTarget.animator.GetBool("Sit"))
                                        currentOffset = aimingOffsetSitTarget;
                                    else if (playerController.unitVision.currentTarget.animator.GetBool("Lie"))
                                        currentOffset = aimingOffsetLieTarget;
                                    playerController.Aiming(playerController.unitVision.currentTarget.transform.position + currentOffset, false);

                                    if (burstSize == 0)
                                    {
                                        burstSize = currentWeapon.burstSize + Random.Range(-currentWeapon.burstSizeDelta, currentWeapon.burstSizeDelta);
                                        nextBurstTime = currentWeapon.nextBurstTime + Random.Range(-currentWeapon.nextBurstTimeDelta, currentWeapon.nextBurstTimeDelta);
                                        nextBurst = Time.time + nextBurstTime;
                                    }
                                    
                                    if (nextBurst < Time.time && burstSize > 0)
                                    {
                                        if (playerController.Shoot(spreadSize, false))
                                        {
                                            burstSize--;
                                        }
                                    }
                                    playerController.TurnOffFiring();

                                    if (currentWeapon.currentAmmo == 0)
                                    {
                                        playerController.Reload(false);
                                        burstSize = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            playerController.TurnOffAiming();
                            playerController.TurnOffFiring();
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
        if (animator.GetBool("EnableAI"))
        {
            playerController.TurnOffFiring();
        }
    }
}
