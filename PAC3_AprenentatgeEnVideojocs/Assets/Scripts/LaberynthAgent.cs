using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class LaberynthAgent : Agent
{
    private Rigidbody rb;
    private float timePassed;

    public float forceMultiplier = 10;
    public Transform[] startingPoints;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        transform.localPosition = startingPoints[Random.Range(0, startingPoints.Length)].localPosition;
        timePassed = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        rb.AddForce(controlSignal * forceMultiplier);

        if (transform.localPosition.y < 0)
        {
            //AddReward(-0.5f);
            SetReward(-0.5f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    /*private void FixedUpdate()
    {
        timePassed += Time.fixedDeltaTime;
        if (timePassed >= 20)
        {
            //AddReward(-0.25f);
            EndEpisode();
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            AddReward(1.0f);
            //SetReward(1.0f);
            EndEpisode();
        }
        else if(other.CompareTag("DeadEnd"))
        {
            //AddReward(-0.1f);
            SetReward(-0.1f);
            EndEpisode();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.collider.CompareTag("Wall"))
        {
            timePassed += Time.deltaTime;
            if (timePassed >= 20)
            {
                SetReward(-0.1f);
                EndEpisode();
            }
            //AddReward(-0.1f);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            timePassed = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            AddReward(-0.01f);
            //EndEpisode();
        }
    }
}