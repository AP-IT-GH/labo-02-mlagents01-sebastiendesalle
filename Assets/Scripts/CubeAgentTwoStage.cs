using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.VisualScripting;

public class CubeAgentTwoStage : Agent
{
    public Transform Target;

    private bool targetCollected;
    public float speedMultiplier = 0.5f;
    public float rotationMultiplier = 5.0f;

    public override void OnEpisodeBegin()
    {
        // check if agent fell
        if (this.transform.localPosition.y < 0)
        {
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
            this.transform.localRotation = Quaternion.identity;
        }

        // random target positions
        Target.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));

        // reactivate target and reset mission state
        Target.gameObject.SetActive(true);
        targetCollected = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(targetCollected ? 1.0f : 0.0f);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.z = actionBuffers.ContinuousActions[0];
        transform.Translate(controlSignal * speedMultiplier);
        transform.Rotate(0.0f, rotationMultiplier * actionBuffers.ContinuousActions[1], 0.0f);

        AddReward(-1f / MaxStep);

        if (this.transform.localPosition.y < 0)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target") && !targetCollected)
        {
            targetCollected = true;
            Target.gameObject.SetActive(false);
            AddReward(0.5f);
        }
        else if (other.CompareTag("Green Zone") && targetCollected)
        {
            AddReward(1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }
}
