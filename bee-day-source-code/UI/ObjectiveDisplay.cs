using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class ObjectiveDisplay : MonoBehaviour
{
    #region Field Declarations
    [SerializeField] private Image currentObjectiveImage;
	[SerializeField] private Sprite whiteFlowerSprite;
	[SerializeField] private Sprite yellowFlowerSprite;
	[SerializeField] private Sprite pinkFlowerSprite;
	[SerializeField] private Sprite treeSprite;

	private readonly List<Sprite> flowerSprites = new List<Sprite>();
	private int currentSprite;
    #endregion

    #region Start Up / Shutdown
	public void ResetObjectiveDisplay()
	{
		CancelInvoke();
		currentObjectiveImage.rectTransform.localScale = new Vector3(2.0f, 1.32f, 1);
		currentObjectiveImage.rectTransform.anchoredPosition = new Vector3(0, 31.0f, 0);
		flowerSprites.Add(whiteFlowerSprite);
		flowerSprites.Add(yellowFlowerSprite);
		flowerSprites.Add(pinkFlowerSprite);
		InvokeRepeating("CycleFlowerColors", 0, 2.0f);
	}
	#endregion

	#region Objective Methods
	public void CompleteObjective()
	{
		if(flowerSprites.Count != 0)
		{
			InvokeRepeating("CycleFlowerColors", 0, 2.0f);
		}
		else
		{
			CancelInvoke();
			currentObjectiveImage.rectTransform.localScale = new Vector3(0.75f, 0.75f, 1);
			currentObjectiveImage.rectTransform.anchoredPosition = new Vector3(0, 0, 0);
			currentObjectiveImage.sprite = treeSprite;
		}
	}

	public void SetObjective(FlowerColor flowerColor)
	{
		CancelInvoke();
		switch (flowerColor)
		{
			case FlowerColor.WHITE:
				flowerSprites.Remove(whiteFlowerSprite);
				currentObjectiveImage.sprite = whiteFlowerSprite;
				break;
			case FlowerColor.YELLOW:
				flowerSprites.Remove(yellowFlowerSprite);
				currentObjectiveImage.sprite = yellowFlowerSprite;
				break;
			case FlowerColor.PINK:
				flowerSprites.Remove(pinkFlowerSprite);
				currentObjectiveImage.sprite = pinkFlowerSprite;
				break;
			case FlowerColor.NONE:
				Debug.LogError("ObjectiveDisplay.cs :: FlowerColor.NONE was passed to SetObjective(FlowerColor flowerColor)");
				break;
		}
	}
    #endregion

    #region Display Methods
    private void CycleFlowerColors()
	{
		if (flowerSprites.Count  == 0)
		{
			currentSprite = 0;
		}
		else
		{
			if (currentSprite + 1 < flowerSprites.Count)
			{
				currentSprite++;
			}
			else
			{
				currentSprite = 0;
			}
		}
		currentObjectiveImage.sprite = flowerSprites[currentSprite];
	}
	#endregion
}
