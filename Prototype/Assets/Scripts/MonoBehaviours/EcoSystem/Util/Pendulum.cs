using UnityEngine;

public class Pendulum : MonoBehaviour
{
    [SerializeField] GameObject projectileObject;
    [SerializeField] Rigidbody _projectileHolder;
    [SerializeField] Transform _startPoint;
    [SerializeField] Transform _pivotPoint;
    [SerializeField] Transform _target;
    [SerializeField] float _armLength = 2.0f;      // Length of Arm
    [SerializeField] float _armAngle = Mathf.PI/4;
    [SerializeField] float _angleVelocity = 0.0f;
    [SerializeField] float _angleAcceleration = 0.0f;
    [SerializeField] float _damping = 0.995f;
    [SerializeField] float _gravity = 0.4f;
    [SerializeField] private KeyCode _SwingKey = KeyCode.Space;

    [SerializeField] bool _debugPath;
    [HideInInspector]
    public bool _IsLaunched = false;

    public void ResetPendulum()
    {
        _projectileHolder.transform.position = _startPoint.transform.position;
        _projectileHolder.transform.rotation = _startPoint.transform.rotation;
        _IsLaunched = false;
    }


    private void Start()
    {
        _projectileHolder.useGravity = false;
    }
    private void Update()
    {
        if(Input.GetKeyDown(_SwingKey))
        {
            Launch();
        }

        if(_IsLaunched)
        {
            _angleAcceleration = (-1 * _gravity / _armLength) * Mathf.Sin(_armAngle);
            _angleVelocity += _angleAcceleration;
            _armAngle += _angleVelocity;
            _angleVelocity *= _damping;
            Vector3 velocityXYZ = new Vector3(0, _armLength * Mathf.Cos(_armAngle), _armLength * Mathf.Sin(_armAngle));
            _projectileHolder.transform.position = _pivotPoint.position + velocityXYZ;

            projectileObject.GetComponent<CharacterController>().enabled = false;
            // projectileObject.transform.position = _projectileHolder.position;
        }
        else
        {
			projectileObject.GetComponent<CharacterController>().enabled = true;
        }
        if(_debugPath)
        {
            DrawPath();
        }
    }

    void Launch(){
        Physics.gravity = Vector3.up * _gravity;
        _projectileHolder.useGravity = true;   

        // _projectileHolder.velocity = CalculateLaunchVelocity().initialVelocity;
        _IsLaunched = true;

    }

    LaunchData CalculateLaunchVelocity()
    {
        float displacmentY = _target.position.y - _projectileHolder.position.y;
        Vector3 displacmentXZ = new Vector3(_target.position.x - _projectileHolder.position.x, 0, _target.position.z - _projectileHolder.position.z);
        float time = Mathf.Sqrt(-2 * _armLength/ _gravity) + Mathf.Sqrt(2 * (displacmentY - _armLength)/_gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * _gravity * _armLength);
        Vector3 velocityXZ = displacmentXZ / time;

        //use sign(gravity) to account for negative projectile 
        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(_gravity), time);

    }

    void DrawPath() {
		LaunchData launchData = CalculateLaunchVelocity ();
		Vector3 previousDrawPoint = _projectileHolder.position;

		int resolution = 30;
		for (int i = 1; i <= resolution; i++) {
			float simulationTime = i / (float)resolution * launchData.timeToTarget;
			Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up *_gravity * simulationTime * simulationTime / 2f;
			Vector3 drawPoint = _projectileHolder.position + displacement;
			Debug.DrawLine (previousDrawPoint, drawPoint, Color.green);
			previousDrawPoint = drawPoint;
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
