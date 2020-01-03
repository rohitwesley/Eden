using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected GameObject projectileObject;
    [SerializeField] protected float _projectileObjectSpeed = 10;
    [SerializeField] protected Rigidbody _projectileHolder;
    [SerializeField] protected Transform _target;
    [SerializeField] protected float _height = 25;      // should always be greater then target height
    [SerializeField] protected float _gravity = -18;
    [SerializeField] protected KeyCode _JumpKey = KeyCode.Tab;
    [SerializeField] protected KeyCode _LaunchKey = KeyCode.Return;

    [SerializeField] public bool _debugPath;
    [HideInInspector]
    public bool _IsLaunched = false;
    protected Transform _objectParent;
    
    public void SetProjectile()
    {
        // _projectileHolder.transform.parent = _startPoint;
        _objectParent = projectileObject.transform.parent;
        _projectileHolder.transform.SetPositionAndRotation(new Vector3(0.0f,1.0f,0.0f), Quaternion.identity);
        _projectileHolder.transform.SetPositionAndRotation(projectileObject.transform.position + Vector3.up * 3, projectileObject.transform.rotation);
        _IsLaunched = false;
    }

    private void Start()
    {
        _projectileHolder.useGravity = false;
        SetProjectile();
    }

    private void Update()
    {
        if(Input.GetKey(_JumpKey))
        {
            SetProjectile();
        }
        if(Input.GetKeyDown(_LaunchKey))
        {
            Launch();
            _IsLaunched = true;
        }

        if(_IsLaunched)
        {
            projectileObject.GetComponent<CharacterController>().enabled = false;
            _objectParent = projectileObject.transform.parent;
            projectileObject.transform.parent = _projectileHolder.transform;
        }
        else
        {
            projectileObject.transform.parent = _objectParent;
			projectileObject.GetComponent<CharacterController>().enabled = true;
        }
        if(_debugPath)
        {
            DrawPath();
        }
    }

    protected void Launch(){
        Physics.gravity = Vector3.up * _gravity;
        _projectileHolder.useGravity = true;
        _projectileHolder.velocity = CalculateLaunchVelocity().initialVelocity;
        

    }

    LaunchData CalculateLaunchVelocity()
    {
        float displacmentY = _target.position.y - _projectileHolder.position.y;
        Vector3 displacmentXZ = new Vector3(_target.position.x - _projectileHolder.position.x, 0, _target.position.z - _projectileHolder.position.z);
        float time = Mathf.Sqrt(-2 * _height/ _gravity) + Mathf.Sqrt(2 * (displacmentY - _height)/_gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * _gravity * _height);
        Vector3 velocityXZ = displacmentXZ / time;

        //use sign(gravity) to account for negative projectile 
        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(_gravity), time);

    }

    public void DrawPath() {
		LaunchData launchData = CalculateLaunchVelocity ();
		Vector3 previousDrawPoint = _projectileHolder.position;

        Gizmos.color =  Color.green;
		int resolution = 30;
		for (int i = 1; i <= resolution; i++) {
			float simulationTime = i / (float)resolution * launchData.timeToTarget;
			Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up *_gravity * simulationTime * simulationTime / 2f;
			Vector3 drawPoint = _projectileHolder.position + displacement;
			Debug.DrawLine (previousDrawPoint, drawPoint, Color.green);
			previousDrawPoint = drawPoint;
		}
	}

    void OnDrawGizmos()
    {
        if(_debugPath && _target != null)
        {
            LaunchData launchData = CalculateLaunchVelocity ();
            Vector3 previousDrawPoint = _projectileHolder.position;

            Gizmos.color =  Color.green;
            int resolution = 30;
            for (int i = 1; i <= resolution; i++) {
                float simulationTime = i / (float)resolution * launchData.timeToTarget;
                Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up *_gravity * simulationTime * simulationTime / 2f;
                Vector3 drawPoint = _projectileHolder.position + displacement;
                Gizmos.DrawLine(previousDrawPoint, drawPoint);
                previousDrawPoint = drawPoint;
            }
        }
        
    }

    struct LaunchData {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;
        public LaunchData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }
    }
    


}
