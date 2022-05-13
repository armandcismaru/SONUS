using UnityEngine;

public class AnimationController : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(1, 1);  
        animator.gameObject.GetComponentInParent<PlayerController>().setKnife(true);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(1, 0);
        animator.gameObject.GetComponentInParent<PlayerController>().setKnife(false);
    }
}
