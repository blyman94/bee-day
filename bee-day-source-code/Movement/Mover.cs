using UnityEngine;

public class Mover : MonoBehaviour
{
	#region Constants
	private const float skinWidth = .015f;
	private const float distanceBetweenRays = 0.25f;
	#endregion

	#region Field and Property Declarations
	// Properites
	public Vector2 ControllerInput { get; private set; }

	// General Fields
	[HideInInspector] public BoxCollider2D bc2d;

	// Movement Fields
	private Vector2 velocity;
	[HideInInspector] public float moveSpeed = 6;
	[HideInInspector] public float xAccelAir = 0.2f;
	[HideInInspector] public float xAccelGround = 0.1f;
	[HideInInspector] public float accelAir = 0.1f;
	private float velocityXSmoothing;
	private float velocityYSmoothing;

	// Collision Detection Fields
	[HideInInspector] public LayerMask groundLayer;
	private CollisionInfo collisions;
	private RaycastOrigins raycastOrigins;
	private int horizontalRayCount;
	private int verticalRayCount;
	private float horizontalRaySpacing;
	private float verticalRaySpacing;

	// Jumping Fields
	[HideInInspector] public bool useGravity = true;
	[HideInInspector] public float maxJumpHeight = 4.0f;
	[HideInInspector] public float minJumpHeight = 1.0f;
	[HideInInspector] public float timeToJumpApex = 0.4f;

	private float gravity = -50;
	private float maxJumpVelocity;
	private float minJumpVelocity;
	#endregion

	#region Delegate Declarations
	public delegate void Land();
	public Land land;
	#endregion

	#region Start Up
	private void Awake()
	{
		bc2d = GetComponent<BoxCollider2D>();
	}

	private void Start()
	{
		if (useGravity)
		{
			gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
			maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
			minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
		}
		CalculateRaySpacing();
		collisions.faceDir = 1;
	}
	#endregion

	#region Update Callbacks
	void Update()
	{
		CalculateVelocity();

		Move(velocity * Time.deltaTime);

		if (collisions.above || collisions.below)
		{
			velocity.y = 0;
		}
	}
	#endregion

	#region Jump Methods
	public void OnJumpInputDown()
	{
		if (collisions.below)
		{
			velocity.y = maxJumpVelocity;
		}
	}
	public void OnJumpInputUp()
	{
		if (velocity.y > minJumpVelocity)
		{
			velocity.y = minJumpVelocity;
		}
	}
	#endregion

	#region Collision Detection Methods
	private void CalculateRaySpacing()
	{
		Bounds bounds = bc2d.bounds;
		bounds.Expand(skinWidth * -2);

		float boundsWidth = bounds.size.x;
		float boundsHeight = bounds.size.y;

		horizontalRayCount = Mathf.RoundToInt(boundsHeight / distanceBetweenRays);
		verticalRayCount = Mathf.RoundToInt(boundsWidth / distanceBetweenRays);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	private void HorizontalCollisions(ref Vector2 moveAmount)
	{
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

		if (Mathf.Abs(moveAmount.x) < skinWidth)
		{
			rayLength = 2 * skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, groundLayer);

			// DEBUG: Drawing rays
			//if (debugMode)
			//{
			//	Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);
			//}

			if (hit)
			{
				moveAmount.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;
				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
			}
		}
	}

	private void UpdateRaycastOrigins()
	{
		Bounds bounds = bc2d.bounds;
		bounds.Expand(skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	private void VerticalCollisions(ref Vector2 moveAmount)
	{
		float directionY = Mathf.Sign(moveAmount.y);
		float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;

			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, groundLayer);

			if (hit)
			{
				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;
				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
				if (collisions.below)
				{
					land?.Invoke();
				}
			}
		}
	}
	#endregion

	#region Movement Methods
	private void CalculateVelocity()
	{
		if (!useGravity)
		{
			float targetVelocityX = ControllerInput.x * moveSpeed;
			velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelAir);
			float targetVelocityY = ControllerInput.y * moveSpeed;
			velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, accelAir);
		}
		else
		{
			float targetVelocityX = ControllerInput.x * moveSpeed;
			velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (collisions.below ? xAccelGround : xAccelAir));
			velocity.y += gravity * Time.deltaTime;
		}
	}
	public void Move(Vector2 moveAmount)
	{
		UpdateRaycastOrigins();
		collisions.Reset();
		if (moveAmount.x != 0)
		{
			collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
		}
		HorizontalCollisions(ref moveAmount);
		if (moveAmount.y != 0)
		{
			VerticalCollisions(ref moveAmount);
		}
		transform.Translate(moveAmount);
	}
	public void SetDirectionalInput(Vector2 input)
	{
		ControllerInput = input;
	}
	#endregion

	#region Structs
	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;

		public int faceDir;

		public void Reset()
		{
			above = false;
			below = false;
			left = false;
			right = false;
		}
	}
	public struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
    #endregion
}
