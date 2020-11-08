using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class CharacterControls : SM_CharacterBase {
	
	public float speed = 10.0f;
	public float airVelocity = 8f;
	public float maxVelocityChange = 10.0f;
	public float jumpHeight = 2.0f;
	public float maxFallSpeed = 20.0f;
	public float rotateSpeed = 25f; //Speed the player rotate
	private Vector3 moveDir;
	private GameObject cam;

	private float distToGround;

	
	

	public Vector3 checkPoint;

	[FormerlySerializedAs("_characterAnimator")] [HideInInspector]
	public SM_CharacterAnimator characterAnimator;

	bool IsGrounded ()
	{
		var isGrounded = Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.01f);
		if (!isGrounded && !characterAnimator.AnimatorState.Jumped)
		{
			characterAnimator.SetJumped(true);
		}
		var isGroundedDown = Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
		if (characterAnimator.AnimatorState.Jumped && isGroundedDown)
		{
			characterAnimator.SetJumped(false);

		}
		return isGrounded;
	}
	
	public void Init () {
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		rb.useGravity = false;

		cam = SM_SceneManager.Instance.CurCamera.gameObject;
		checkPoint = transform.position;
		characterAnimator = GetComponentInChildren<SM_CharacterAnimator>();
		characterAnimator.Init();
		distToGround = GetComponent<Collider>().bounds.extents.y;
	}
	
	void FixedUpdate ()
	{
		characterAnimator.DoUpdate(Time.deltaTime);
		//开始玩
		if (SM_GameManager.Instance.GameState!=EGameState.Playing)
			return;
		//成功
		if (characterAnimator.AnimatorState.Success)
		{
			characterAnimator.SetSuccess();
			rb.velocity = Vector3.zero;
			return;
		}
		//失败
		if (characterAnimator.AnimatorState.Failed)
		{
			characterAnimator.SetFailed();
			rb.velocity = Vector3.zero;
			return;
		}
		characterAnimator.SetIdle(IsGrounded() &&
		                           moveDir==Vector3.zero);
		characterAnimator.SetRun( IsGrounded() &&
		                           (Math.Abs(moveDir.x) > 0 || Math.Abs(moveDir.z) > 0) &&
		                           Math.Abs(moveDir.y) <= 0);
		
		
		if (canMove)
		{
			if (moveDir.x != 0 || moveDir.z != 0)
			{
				Vector3 targetDir = moveDir; //Direction of the character

				targetDir.y = 0;
				if (targetDir == Vector3.zero)
					targetDir = transform.forward;
				Quaternion tr = Quaternion.LookRotation(targetDir); //Rotation of the character to where it moves
				Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * rotateSpeed); //Rotate the character little by little
				transform.rotation = targetRotation;
			}

			if (IsGrounded())
			{
			 // Calculate how fast we should be moving
				Vector3 targetVelocity = moveDir;
				targetVelocity *= speed;

				// Apply a force that attempts to reach our target velocity
				Vector3 velocity = rb.velocity;
				if (targetVelocity.magnitude < velocity.magnitude) //If I'm slowing down the character
				{
					targetVelocity = velocity;
					rb.velocity /= 1.1f;
				}
				Vector3 velocityChange = (targetVelocity - velocity);
				velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
				velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = 0;
				if (!slide)
				{
					if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
						rb.AddForce(velocityChange, ForceMode.VelocityChange);
				}
				else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
				{
					rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
					//Debug.Log(rb.velocity.magnitude);
				}

				// Jump
				if (IsGrounded() && Input.GetButton("Jump"))
				{
					rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
				}
			}
			else
			{
				if (!slide)
				{
					Vector3 targetVelocity = new Vector3(moveDir.x * airVelocity, rb.velocity.y, moveDir.z * airVelocity);
					Vector3 velocity = rb.velocity;
					Vector3 velocityChange = (targetVelocity - velocity);
					velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
					velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
					rb.AddForce(velocityChange, ForceMode.VelocityChange);
					if (velocity.y < -maxFallSpeed)
						rb.velocity = new Vector3(velocity.x, -maxFallSpeed, velocity.z);
				}
				else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
				{
					rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
				}
			}
		}
		else
		{
			rb.velocity = pushDir * pushForce;
		}
		// We apply gravity manually for more tuning control
		rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));
	}

	private void Update()
	{
		if (SM_GameManager.Instance.GameState!=EGameState.Playing)
			return;
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		Vector3 v2 = v * cam.transform.forward; //Vertical axis to which I want to move with respect to the camera
		Vector3 h2 = h * cam.transform.right; //Horizontal axis to which I want to move with respect to the camera
		moveDir = (v2 + h2).normalized; //Global position to which I want to move in magnitude 1

		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f))
		{
			if (hit.transform.tag == "Slide")
			{
				slide = true;
			}
			else
			{
				slide = false;
			}
		}
	}

	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	

	public void LoadCheckPoint()
	{
		transform.position = checkPoint;
	}

	
}
