using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float MoveSpeed = 50;
	public float JumpSpeed = 100;
	private BaseObject _baseObject;
	private Animator _animator =null;	

	// Use this for initialization
	void Start () 
	{
		_animator = GetComponent<Animator>();
		_baseObject = GetComponent<BaseObject>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//update velocity 
		float xInput = Input.GetAxisRaw("Horizontal");
		if (updateDirection(xInput)==true)
		{
			xInput = 0;
		}

		float jumpInput =Input.GetAxis("Jump");
		if (_baseObject.Grounded==false)
		{
			jumpInput = 0;
		}
		
        Vector2 acceleration =_baseObject.Acceleration;
		acceleration.x = 500*xInput;
		_baseObject.Acceleration = acceleration;

		Vector2 maxVelocity = _baseObject.MaxVelocity;
		maxVelocity.x = MoveSpeed;
		_baseObject.MaxVelocity = maxVelocity;

		Vector2 velocity = _baseObject.Velocity;
		velocity.y += jumpInput*JumpSpeed;
		if (acceleration.x==0)
		{
			velocity.x = 0;
		}
		_baseObject.Velocity = velocity;

		updateAnimation();
	}

	void updateAnimation()
	{
		Vector2 velocity = _baseObject.Velocity;
		float speed = Mathf.Abs(velocity.x);
		_animator.SetFloat("speed",speed);
		
		bool grounded = _baseObject.Grounded;
		// Debug.Log("grounded:"+grounded);
		_animator.SetBool("grounded",grounded);
	}

	bool updateDirection(float xInput)
	{
		if (_baseObject==null)
		{
			return false;
		}	
		if (xInput==0)
		{
			return false;
		}

		bool changeDir = false;
		BaseObject.FaceToType oldFaceToType = _baseObject.CurFaceTo;
		if (xInput<0.0f)	
		{
			_baseObject.CurFaceTo = BaseObject.FaceToType.Left;
		}
		else if(xInput>0.0f)
		{
			_baseObject.CurFaceTo = BaseObject.FaceToType.Right;
		}
		if (oldFaceToType!=_baseObject.CurFaceTo)
		{
			changeDir = true;
		}

		return changeDir;
	}

}
