using UnityEngine;

public class PorcupineAnimatorHelper : MonoBehaviour
{
	[SerializeField] private Animator animator;

	public delegate void AttackHit();
	public AttackHit attackHit;

	public void OnAttackHit()
	{
		attackHit?.Invoke();
	}

	public void OnAttackEnd()
	{
		animator.SetTrigger("endAttack");
	}
}
