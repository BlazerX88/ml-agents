using MLAgents;
using UnityEngine;

public class RollerAgent : Agent
{
    // Start is called before the first frame update
    const float FixedY = 0.5f;
    [SerializeField] float speed = 10f;
    [SerializeField] Transform floor = null;
    [SerializeField] Transform target = null;
    private new Rigidbody rigidbody;

    public void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public override void AgentReset()
    {
        // Make the momentum zero
        if (transform.localPosition.y < 0f)
        {
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.velocity = Vector3.zero;
            transform.localPosition = new Vector3(0f, FixedY, 0f);
        }
        transform.localPosition = new Vector3(Random.value * 8 - 4, FixedY, Random.value * 8 - 4);
    }

    public override void CollectObservations()
    {
        AddVectorObs(target.localPosition); // 3
        AddVectorObs(transform.localPosition); // + 3
        AddVectorObs(Normalize(rigidbody.velocity.x, 0f, speed)); // + 1
        AddVectorObs(Normalize(rigidbody.velocity.z, 0f, speed)); // + 1
        // = 8 vector observations
    }

    public override void AgentAction(float[] vectorAction)
    {
        var controlSignal = new Vector3(vectorAction[0], 0f, vectorAction[1]); // 1 + 1
        print(controlSignal);
        // = 2 vector actions
        rigidbody.AddForce(controlSignal * speed);

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, target.localPosition);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            // SetReward(0.1f);
            SetReward(1f);
            Done();
        }

        // fell off the floor
        if (this.transform.localPosition.y < 0)
        {
            // SetReward(-1f);
            Done();
        }

        Monitor.Log("Reward", GetReward(), floor);
        Monitor.Log("Cumulative Reward", GetCumulativeReward());
        // SetReward(-0.05f);
    }

    public override float[] Heuristic()
    {
        return new float[] { Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") };
    }

    private float Normalize(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }
}
