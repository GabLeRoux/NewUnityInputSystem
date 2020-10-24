/*
Copyright (c) 2020 Razeware LLC

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or
sell copies of the Software, and to permit persons to whom
the Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

Notwithstanding the foregoing, you may not use, copy, modify,
merge, publish, distribute, sublicense, create a derivative work,
and/or sell copies of the Software in any work that is designed,
intended, or marketed for pedagogical or instructional purposes
related to programming, coding, application development, or
information technology. Permission for such use, copying,
modification, merger, publication, distribution, sublicensing,
creation of derivative works, or sale is expressly withheld.

This project and source code may use libraries or frameworks
that are released under various Open-Source licenses. Use of
those libraries and frameworks are governed by their own
individual licenses.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	// Constant
	const float jumpCheckPreventionTime = 0.5f;

	// Callback
	public delegate void CollectCoinCallback();
	public CollectCoinCallback onCollectCoin;

	// Public
	[Header("Physic Setting")]
	public LayerMask groundLayerMask;

	[Header("Move & Jump Setting")]
	public float moveSpeed = 10;
	public float fallWeight = 5.0f;
	public float jumpWeight = 0.5f;
	public float jumpVelocity = 100.0f;

	// Internal Data

	// State of the player (jumping or not)
	protected bool jumping = false;			// state of player (jumping or not )

	//
	protected Vector3 moveVec = Vector3.zero; // movement speed of player
	protected float jumpTimestamp;			// start jump timestamp

	protected Animator animator;				// reference to the animator
	protected Rigidbody rigidbody;			// reference to the rigidbody



	// Start is called before the first frame update
	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
		rigidbody = GetComponent<Rigidbody>();
	}
	
	void UpdateWhenJumping()
	{
		bool isFalling = rigidbody.velocity.y <= 0;

		float weight = isFalling ? fallWeight : jumpWeight;

		// Assign new velocity
		rigidbody.velocity = new Vector3(moveVec.x * moveSpeed, rigidbody.velocity.y, moveVec.z * moveSpeed);
		rigidbody.velocity += Vector3.up * Physics.gravity.y * weight * Time.deltaTime;

		GroundCheck();
	}

	void UpdateWhenGrounded()
	{
		// 1 
		rigidbody.velocity = moveVec * moveSpeed;

		// 2
		if (moveVec != Vector3.zero)
		{
			transform.LookAt(this.transform.position + moveVec.normalized);
		}

		// 3
		CheckShouldFall();
	}

	private void FixedUpdate()
	{
		if (jumping == false)
		{
			// 2
			UpdateWhenGrounded();
		}
		else
		{
			// 3
			UpdateWhenJumping();
		}
	}

	// Update is called once per frame
	void Update()
	{
		UpdateAnimation();
	}

	public void OnJump()
    {
		HandleJump();
    }

	public void OnMove(InputValue input)
    {
		Vector2 inputVec = input.Get<Vector2>();

		moveVec = new Vector3(inputVec.x, 0, inputVec.y);
    }

	#region Jump & Fall & Ground Logic

	protected bool HandleJump()
	{
		if (jumping)
		{
			return false;
		}

		jumping = true;
		jumpTimestamp = Time.time;
		rigidbody.velocity = new Vector3(0, jumpVelocity, 0); // Set initial jump velocity

		return true;
	}

	void CheckShouldFall()
	{
		if(jumping)
		{
			return;	// No need to check if in the air
		}

		bool hasHit = Physics.CheckSphere(transform.position, 0.1f, groundLayerMask);

		if (hasHit == false)
		{
			jumping = true;
		}
	}

	void GroundCheck()
	{
		if(jumping == false)
		{
			return;	// No need to check
		}

		if (Time.time < jumpTimestamp + jumpCheckPreventionTime)
		{
			return;
		}

		bool hasHit = Physics.CheckSphere(transform.position, 0.1f, groundLayerMask);
		
		if(hasHit)
		{
			jumping = false;
		}
	}

	#endregion

	void UpdateAnimation()
	{
		if (animator == null)
		{
			return;
		}

		animator.SetBool("jumping", jumping);
		animator.SetFloat("moveSpeed", moveVec.magnitude);
	}

	#region Coin Collect Logic

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Coin")
		{
			HandleCoinCollect(other);
		}
	}

	void HandleCoinCollect(Collider collision)
	{
		Coin coin = collision.transform.GetComponent<Coin>();
		if(coin == null)
		{
			return;
		}
		coin.Collect();

		if(onCollectCoin != null)
		{
			onCollectCoin();
		}
	}

	#endregion

}
