using UnityEngine;

public enum FlowerColor {WHITE,PINK,YELLOW,NONE}
public class Flower : MonoBehaviour
{
	public FlowerColor flowerColor;
	public GameObject ungottenGlow;
	public CircleCollider2D collectTrigger;

	public void TurnOffGlow()
	{
		ungottenGlow.SetActive(false);
		collectTrigger.enabled = false;
	}
}
