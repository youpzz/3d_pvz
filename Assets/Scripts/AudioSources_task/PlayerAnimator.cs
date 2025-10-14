using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public static PlayerAnimator Instance;
    private Animator animator;

    void Start()
    {
        Instance = this;
        animator = GetComponent<Animator>();
    }


    public void SetTrigger(string name) => animator.SetTrigger(name);
}
