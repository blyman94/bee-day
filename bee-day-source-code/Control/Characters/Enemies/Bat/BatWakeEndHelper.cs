using UnityEngine;

public class BatWakeEndHelper : MonoBehaviour
{
	[SerializeField] private Animator animator;
	public void OnWakeEnd()
	{
		// Responds to the wake end animation event
		animator.SetTrigger("wakeEnd");
	}
}
