using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	#region Serialized Field Declarations
	[Header("Debugging")]
	[SerializeField] private bool debugMode;

	[Header("Camera Shake")]
	[Space]
	[SerializeField] private Health shakeSource;
	[SerializeField] private bool shakeOnDamageTaken = true;
	[SerializeField] private float shakeTime = 1.0f;
	[SerializeField] private float shakeIntensity = 1.0f;

	[Header("Camera Follow Options")]
	[Space]
	[SerializeField] private bool cameraFollow;
	[SerializeField] private Mover target;
	[SerializeField] private Vector2 focusAreaSize;
	[SerializeField] private float lookAheadDistX;
	[SerializeField] private float lookSmoothTimeX;
	[SerializeField] private float verticalOffset;
	[SerializeField] private float verticalSmoothTime;
	#endregion

	#region Field Declarations
	// Camera Shake Fields
	private bool isShaking;

	// Camera Follow Fields
	private FocusArea focusArea;
	private Vector2 focusPosition;
	private float currentLookAheadX;
	private float targetLookAheadX;
	private float smoothLookVelocityX;
	private float lookAheadDirX;
	private bool lookAheadStopped;
	private float smoothVelocityY;
	#endregion
	
	#region Start Up
    private void Start()
    {
		if (cameraFollow)
		{
			focusArea = new FocusArea(target.bc2d.bounds, focusAreaSize);
		}
    }
	private void OnEnable()
	{
		shakeSource.changed += CameraDamageTakenShake;
		BeeBombProjectile.shakeCamera += CameraBombShake;
	}
	#endregion

	#region Shut Down
	private void OnDisable()
	{
		shakeSource.changed -= CameraDamageTakenShake;
		BeeBombProjectile.shakeCamera -= CameraBombShake;
	}
	#endregion

	#region Update Callbacks
	private void LateUpdate()
	{
		if (cameraFollow)
		{
			CameraFollow();
		}
	}
	#endregion

	#region Camera Shake Methods

	private void CameraBombShake(float shakeIntensity, float shakeTime)
	{
		if (!isShaking)
		{
			StartCoroutine(CameraShakeRoutine(shakeIntensity,shakeTime));
		}
	}
	private void CameraDamageTakenShake()
	{
		if (shakeOnDamageTaken && !isShaking)
		{
			StartCoroutine(CameraShakeRoutine(this.shakeIntensity,this.shakeTime));
		}
	}
	private IEnumerator CameraShakeRoutine(float shakeIntensity, float shakeTime)
	{
		isShaking = true;
		Vector3 currentPos = transform.position;
		Vector3 targetPosUp = transform.position + Vector3.up * shakeIntensity;
		Vector3 targetPosDown = transform.position + Vector3.up * shakeIntensity * -1;
		float halfTime = shakeTime * 0.5f;

		float elapsedTime = 0;

		while (elapsedTime < (halfTime))
		{
			transform.position = Vector3.Lerp(currentPos, targetPosUp, (elapsedTime / halfTime));
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		elapsedTime = 0;

		while (elapsedTime < (halfTime))
		{
			transform.position = Vector3.Lerp(targetPosUp, targetPosDown, (elapsedTime / halfTime));
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		elapsedTime = 0;

		while (elapsedTime < (halfTime))
		{
			transform.position = Vector3.Lerp(targetPosDown, currentPos, (elapsedTime / halfTime));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		isShaking = false;
	}
	#endregion

	#region Camera Follow Methods
	private void CameraFollow()
	{
		focusArea.Update(target.bc2d.bounds);
		focusPosition = focusArea.center + Vector2.up * verticalOffset;

		XLookAhead();

		focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
	}
	private void XLookAhead()
	{
		if (focusArea.velocity.x != 0)
		{
			lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
			if (Mathf.Sign(target.ControllerInput.x) == Mathf.Sign(focusArea.velocity.x) &&
				target.ControllerInput.x != 0)
			{
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDirX * lookAheadDistX;
			}
			else
			{
				if (!lookAheadStopped)
				{
					lookAheadStopped = true;
					targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDistX - currentLookAheadX) * 0.25f;
				}
			}
		}

		currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);
		focusPosition += Vector2.right * currentLookAheadX;
	}
	#endregion

	#region Debugging
	private void OnDrawGizmos()
	{
		if (debugMode)
		{
			Gizmos.color = new Color(1, 0, 0, 0.5f);
			Gizmos.DrawCube(focusArea.center, focusAreaSize);
		}
	}
	#endregion

	#region Structs
	struct FocusArea
	{
		public Vector2 center;
		public Vector2 velocity;
		private float left, right;
		private float top, bottom;

		public FocusArea(Bounds targetBounds, Vector2 size)
		{
			left = targetBounds.center.x - (size.x * 0.5f);
			right = targetBounds.center.x + (size.x * 0.5f);
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			center = new Vector2((left + right) * 0.5f, (bottom + top) * 0.5f);
		}

		public void Update(Bounds targetBounds)
		{
			float shiftX = 0;
			if (targetBounds.min.x < left)
			{
				shiftX = targetBounds.min.x - left;
			}
			else if (targetBounds.max.x > right)
			{
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom)
			{
				shiftY = targetBounds.min.y - bottom;
			}
			else if (targetBounds.max.y > top)
			{
				shiftY = targetBounds.max.y - top;
			}
			top += shiftY;
			bottom += shiftY;

			center = new Vector2((left + right) * 0.5f, (bottom + top) * 0.5f);
			velocity = new Vector2(shiftX, shiftY);
		}
	}
    #endregion
}
