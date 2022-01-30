using UnityEngine;

public class SlimeJumpHelper : MonoBehaviour
{
	[SerializeField] private Animator animator;
	[SerializeField] private Mover mover;
	public void OnJumpLandEnd()
	{
		animator.SetTrigger("landEnd");
	}
	public void OnJumpPrepEnd()
	{
		animator.SetTrigger("prepEnd");
		mover.OnJumpInputDown();
	}
}
