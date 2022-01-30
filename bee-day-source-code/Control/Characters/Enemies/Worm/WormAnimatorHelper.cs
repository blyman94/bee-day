using UnityEngine;

public class WormAnimatorHelper : MonoBehaviour
{
	[SerializeField] private Animator animator;

	public delegate void ShootFrame();
	public ShootFrame shootFrame;
	public delegate void AttackHit();
	public AttackHit attackHit;
	public delegate void GoUnderground();
	public GoUnderground goUnderground;

	public void OnAttackHit()
	{
		attackHit?.Invoke();
	}
	public void OnAttackEnd()
	{
		animator.SetTrigger("endAttack");
	}
	public void OnDigEnd()
	{
		animator.SetTrigger("endDig");
		goUnderground?.Invoke();
	}
	public void OnUpEnd()
	{
		animator.SetTrigger("endUp");
	}
	public void OnShootFrame()
	{
		shootFrame?.Invoke();
	}
}
