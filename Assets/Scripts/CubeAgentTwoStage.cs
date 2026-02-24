using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CubeAgentTwoStage : Agent
{
    public Transform Target;
    public Transform GreenZone;

    private bool targetCollected;
    public float speedMultiplier = 0.5f;

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

    }
}
