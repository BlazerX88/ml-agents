using MLAgents;
using UnityEngine;

public class RollerAgent : Agent
{
    // Start is called before the first frame update
    const float FixedY = 0.5f;
    [SerializeField] float speed = 10f;
    [SerializeField] Transform target = null;
    private new Rigidbody rigidbody;

    public void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public override void AgentReset()
    {
        if (transform.position.y < 0f)
        {
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.velocity = Vector3.zero;
            transform.position = new Vector3(0f, FixedY, 0f);
        }
        transform.position = new Vector3(Random.value * 8 - 4, FixedY, Random.value * 8 - 4);
    }

    public override void CollectObservations()
    {
        AddVectorObs(target.position); // 3
        AddVectorObs(transform.position); // + 3
        AddVectorObs(rigidbody.velocity.x); // + 1
        AddVectorObs(rigidbody.velocity.z); // + 1
        // = 8 vector observations
    }

    public override void AgentAction(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0]; // 1
        controlSignal.z = vectorAction[1]; // + 1
        // = 2 vector actions
        rigidbody.AddForce(controlSignal * speed);

        float distanceToTarget = Vector3.Distance(this.transform.position, target.position);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            Done();
        }

        // fell off the floor
        if (this.transform.position.y < 0)
        {
            SetReward(-0.125f);
            Done();
        }
    }

    public override float[] Heuristic()
    {
        return new float[] { Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical") };
    }
}
