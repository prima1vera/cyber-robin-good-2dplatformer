using UnityEngine;

public class AttackSMB : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var pc = animator.GetComponentInParent<PlayerController>();
        if (pc != null)
        {
            pc.OnAttackAnimationEnd();
        }
    }
}
