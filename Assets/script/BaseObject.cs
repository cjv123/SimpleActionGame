using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour {
	public float gravity = 400;

	public enum FaceToType{
		Left,Right
	}

	public delegate void onHit(GameObject gameObject);
	public event onHit onLeftHit;
	public event onHit onRightHit;
	public event onHit onUpHit;
	public event onHit onDownHit;

	private FaceToType _curFaceTo=FaceToType.Right;

	private CharacterController _characterCtrler = null;

	private Vector2 _velocity; 
	private Vector2 _acceleration;
	private Vector2 _maxVelocity;
	private GameObject _boundingBox;
	private bool _grounded=true;


	// Use this for initialization
	void Start () 
	{
		_characterCtrler = GetComponent<CharacterController>();
		_velocity = new Vector2();
		_acceleration = new Vector2();		
		_acceleration.y = -gravity;

		_boundingBox = transform.Find("boundingbox").gameObject;
		_boundingBox.GetComponent<BoxCollider2D>().enabled=false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		updateVelocity();	
		Vector2 moveDelta = _velocity*Time.deltaTime;
		updateCollisionX(ref moveDelta);
		updateCollisionY(ref moveDelta);
		updateMove(moveDelta);
	}

	void updateVelocity()
	{
        _velocity += _acceleration * Time.deltaTime;
		if (_maxVelocity.x!=0 && Mathf.Abs(_velocity.x)>_maxVelocity.x)
		{
			if (_velocity.x<0)	
			{
				_velocity.x = -_maxVelocity.x;
			}
			else
			{
                _velocity.x = _maxVelocity.x;
			}
		}	
		if (_maxVelocity.y!=0 && Mathf.Abs(_velocity.y)>_maxVelocity.y)
		{
			if (_velocity.y<0)
			{
                _velocity.y = -_maxVelocity.y;
			}
			else
			{
                _velocity.y = _maxVelocity.y;
			}
		}
	}

	void updateMove(Vector2 moveDelta)
	{
		_characterCtrler.Move(moveDelta);	
	}

	void updateCollisionX(ref Vector2 moveDelta)
	{
		if (moveDelta.x==0)
		{
			return;
		}

		Rect boundingBox= getBoundingBox();
		Vector2 direction = Vector2.right;
		Vector2 orginPoint = boundingBox.max;
		if (moveDelta.x<0)
		{
			direction = Vector2.left;
			orginPoint.x = boundingBox.xMin;
		}

		Debug.DrawRay(orginPoint,moveDelta,Color.red);

		float distance=Mathf.Abs(moveDelta.x);
		RaycastHit2D raycastHit2D = Physics2D.Raycast(orginPoint,direction,distance);
		if (raycastHit2D.collider!=null)
		{
			Vector2 hitPoint = raycastHit2D.point;
			// Debug.Log("XhitPoint:"+hitPoint);
			moveDelta.x=hitPoint.x-orginPoint.x;
			_velocity.x = 0;

			if (direction==Vector2.right)
			{
				if (null!=onRightHit)
                    onRightHit(gameObject);
			}
			else
			{
				if (null!=onLeftHit)
                    onLeftHit(gameObject);
			}
		}
	}

	void updateCollisionY(ref Vector2 moveDelta)
	{
		if (moveDelta.y==0)
		{
			return;
		}

		Rect boundingBox = getBoundingBox();
		Vector2 direction=Vector2.down;
		Vector2 orginPoint = new Vector2(boundingBox.center.x,boundingBox.yMin);
		if (moveDelta.y>0)
		{
			direction = Vector2.up;
			orginPoint.y = boundingBox.yMax;
		}

		bool grounded = false;
		
		Debug.DrawRay(orginPoint,moveDelta,Color.red);
		float distance = Mathf.Abs(moveDelta.y);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(orginPoint,direction,distance);
		if (raycastHit2D.collider!=null)	
		{
			Vector2 hitPoint = raycastHit2D.point;
			// Debug.Log("YhitPoint:"+hitPoint);
			moveDelta.y=hitPoint.y - orginPoint.y;
			_velocity.y =0;

			if (direction==Vector2.up)
			{
				if (null!=onUpHit)
                    onUpHit(gameObject);
			}
			else
			{
				if (null!=onDownHit)
                    onDownHit(gameObject);
				grounded = true;
			}
		}

		_grounded = grounded;
	}

	Rect getBoundingBox()
	{
		BoxCollider2D boxCollider2D = _boundingBox.GetComponent<BoxCollider2D>();
        Vector2 pos = boxCollider2D.transform.position;
		Rect rect=new Rect();
		if (boxCollider2D)		
		{
            Vector2 size = boxCollider2D.size;
            Vector2 centerPos = pos + boxCollider2D.offset;
            Vector2 minPos = new Vector2(centerPos.x - size.x / 2, centerPos.y - size.y / 2);
            rect = new Rect(minPos, size);
        }
		else
		{
			SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();			
			if (spriteRenderer)
			{
				Vector2 size = spriteRenderer.bounds.size;
				Vector2 minPos = new Vector2(pos.x-size.x/2,pos.y-size.y/2);
				rect = new Rect(minPos,size);
			}
		}
		return rect;
	}

	public FaceToType CurFaceTo
	{
		get
		{
			return _curFaceTo; 
		}
		set
		{
			if (_curFaceTo==value)
			{
				return;
			}

			_curFaceTo = value;

			Vector3 curScale = transform.localScale;
			if (_curFaceTo==FaceToType.Right)
			{
				transform.localScale = new Vector3(Mathf.Abs(curScale.x),curScale.y,curScale.z);
			}
			else
			{
				transform.localScale = new Vector3(-Mathf.Abs(curScale.x),curScale.y,curScale.z);
			}
		}
	}

	public Vector2 Velocity
	{
		set
		{
			_velocity = value;
		}
		get
		{
			return _velocity;
		}
	}

	public Vector2 MaxVelocity
	{
		get
		{
			return _maxVelocity;
		}

		set
		{
			_maxVelocity = value;
		}
	}

	public Vector2 Acceleration
	{
		get
		{
			return _acceleration;
		}	

		set
		{
			_acceleration = value;
		}
	}

	public bool Grounded
	{
		get
		{
			return _grounded;
		}
	}
}
