using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Jumper : Agent
{
    private Rigidbody rBody;
    private Vector3 startingPosition;
    private int score = 0;

    public event Action OnReset;

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        startingPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Right();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Left();
        RequestDecision();
    }



    public override void OnActionReceived(float[] vectorAction)
    {

        if (Mathf.FloorToInt(vectorAction[1]) == 1)
            Left();
        if (Mathf.FloorToInt(vectorAction[0]) == 1)
            Right();
    }

    private void Left()
    {
        if (transform.position.x > 1f)
            transform.position = new Vector3(transform.position.x - (2.5f), 0, 0);
    }

    private void Right()
    {
        if (transform.position.x < 6f)
            transform.position = new Vector3(transform.position.x + (2.5f), 0, 0);
    }

    public override void OnEpisodeBegin()
    {
        Reset();
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = 0;
        actionsOut[1] = 0;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            actionsOut[0] = 1;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            actionsOut[1] = 1;

    }


    private void Reset()
    {
        score = 0;

        //Reset Movement and Position
        rBody.velocity = Vector3.zero;

        OnReset?.Invoke();
    }

    private void OnCollisionEnter(Collision collidedObj)
    {

        if (collidedObj.gameObject.CompareTag("Mover"))
        {
            Destroy(collidedObj.gameObject);
            AddReward(-50.0f);
            EndEpisode();
                     
        }
    }

    private void OnTriggerEnter(Collider collidedObj)
    {
        if (collidedObj.gameObject.CompareTag("score"))
        {
            Destroy(collidedObj.gameObject);
            AddReward(1.0f);
            score++;
            ScoreCollector.Instance.AddScore(score);
            
        }
    }
}
