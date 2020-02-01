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
        AddVectorObs(target.position);
        AddVectorObs(transform.position);
        AddVectorObs(rigidbody.velocity.x);
        AddVectorObs(rigidbody.velocity.z);
    }

    public override void AgentAction(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rigidbody.AddForce(controlSignal * speed);

        float distanceToTarget = Vector3.Distance(this.transform.position, target.position);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            Done();
        }

        if (this.transform.position.y < 0)
        {
            Done();
        }
    }

    public override float[] Heuristic()
    {
        return new float[] { Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical") };
    }
}
