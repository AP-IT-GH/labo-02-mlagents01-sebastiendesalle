using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Net.Sockets;

public class ObelixAgent : Agent
{
    [Header("Beweging")]
    public float speedMultiplier = 0.5f;
    public float rotationMultiplier = 5.0f;

    [Header("SpelObjecten")]
    public List<GameObject> menhirs;
    public List<GameObject> destinations;

    [Header("Visueel & Status")]
    public GameObject menhirOpRug;
    public Material transparantMateriaal;
    public Material volMateriaal;

    private bool draagtMenhir = false;
    private int correctGeplaatsteMenhirs = 0;

    public override void OnEpisodeBegin()
    {
        if (this.transform.position.y < 0)
        {
            this.transform.localPosition = new Vector3(0, 1f, 0);
            this.transform.localRotation = Quaternion.identity;
        }

        draagtMenhir = false;
        menhirOpRug.SetActive(false);
        correctGeplaatsteMenhirs = 0;

        foreach (GameObject m in menhirs)
        {
            m.SetActive(true);
            m.transform.localPosition = new Vector3(Random.Range(-8f, 8f), 1.0f, Random.Range(-8f, 8f));
            m.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }

        foreach (GameObject d in destinations)
        {
            d.GetComponent<Collider>().enabled = true;
            d.GetComponent<Renderer>().material = transparantMateriaal;
            d.transform.localPosition = new Vector3(Random.Range(-8f, 8f), 0.5f, Random.Range(-8f, 8f));
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(draagtMenhir ? 1.0f : 0.0f);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.z = actionBuffers.ContinuousActions[0];
        transform.Translate(controlSignal * speedMultiplier);
        transform.Rotate(0.0f, rotationMultiplier * actionBuffers.ContinuousActions[1], 0.0f);

        AddReward(-0.001f);

        if (this.transform.localPosition.y < 0)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Menhir") && !draagtMenhir)
        {
            draagtMenhir = true;
            menhirOpRug.SetActive(true);
            collision.gameObject.SetActive(false);
            AddReward(1.0f);
            Debug.Log("Menhir opgepakt");
        }
        else if (collision.gameObject.CompareTag("Menhir") && draagtMenhir)
        {
            AddReward(-0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination") && draagtMenhir)
        {
            draagtMenhir = false;
            menhirOpRug.SetActive(false);

            other.GetComponent<Renderer>().material = volMateriaal;
            other.GetComponent<Collider>().enabled = false;

            AddReward(1.0f);
            correctGeplaatsteMenhirs++;

            if (correctGeplaatsteMenhirs >= menhirs.Count)
            {
                EndEpisode();
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }
}
