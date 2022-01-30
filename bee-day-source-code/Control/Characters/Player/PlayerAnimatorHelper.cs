using UnityEngine;

public class PlayerAnimatorHelper : MonoBehaviour
{
	[SerializeField] private Animator animator;

	public delegate void Strike();
	public Strike strike;

	public void OnAttackEnd()
	{
		animator.SetTrigger("endAttack");
	}

	public void OnStrike()
	{
		strike?.Invoke();
	}
	
}
