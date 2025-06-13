using UnityEngine;
using UnityEngine.Animations;

namespace IdleRPG
{
    public class SceneLinkedSMB<TMonoBehaviour> : SealedSMB
        where TMonoBehaviour : MonoBehaviour
    {
        protected TMonoBehaviour tMonoBehaviour;
        bool p_FirstFrameHappened;
        bool p_LastFrameHappened;

        public static void Initialise(Animator animator, TMonoBehaviour monoBehaviour)
        {
            SceneLinkedSMB<TMonoBehaviour>[] smbs = animator.GetBehaviours<SceneLinkedSMB<TMonoBehaviour>>();
            for (int i = 0; i < smbs.Length; i++)
            {
                smbs[i].InternalInitialise(animator, monoBehaviour);
            }
        }

        protected void InternalInitialise(Animator animator, TMonoBehaviour monoBehaviour)
        {
            tMonoBehaviour = monoBehaviour;
            OnStart(animator);
        }

        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            p_FirstFrameHappened = false;
            OnSLStateEnter(animator, stateInfo, layerIndex);
        }

        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            if (!animator.gameObject.activeSelf)
                return;

            if (!animator.IsInTransition(layerIndex) && p_FirstFrameHappened)
            {
                OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            }

            if (!animator.IsInTransition(layerIndex) && !p_FirstFrameHappened)
            {
                p_FirstFrameHappened = true;
                OnSLStatePostEnter(animator, stateInfo, layerIndex);
            }
        }

        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            p_LastFrameHappened = false;
            OnSLStateExit(animator, stateInfo, layerIndex);
        }

        public virtual void OnStart(Animator animator) { }
        public virtual void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
        public virtual void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
        public virtual void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
        public virtual void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
    }

    public abstract class SealedSMB : StateMachineBehaviour
    {
        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
    }
}
