using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAgents;
using MLAgents.Sensors;
using UnityEngine.UIElements;
using Random = System.Random;

public class CarAgent : Agent
{
    public bool debug;
    
    private Rigidbody _rigidbody;

    public Transform _carStartTransform;
    public Vector3 _carStartPosition;
    public Quaternion _carStartRotation;

    public float xAxis;
    private DateTime _episodeStart;
    public float speed;
    public float time;
    
    public GameObject[] rewardCubes;

    public int SensorRange = 15;

    private int _sensorLayerMask;

    public float reward;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        
        // We save the car's start position and rotation separately,
        // because we can't the Transform constructor is protected.
        _carStartTransform = transform;

        var position = _carStartTransform.position;
        _carStartPosition = new Vector3(position.x, position.y, 
            position.z);

        var rotation = _carStartTransform.rotation;
        _carStartRotation =new Quaternion(rotation.x, rotation.y, 
            rotation.z, rotation.w);
        
        rewardCubes = GameObject.FindGameObjectsWithTag("RewardCube");

        // Bit shift the index of the layer (12 (rayWalls)) to get a bit mask
        _sensorLayerMask = 1 << 12;

        reward = 0;
    }
   
    public override void OnEpisodeBegin()
    {
        // Resets the car's position/movement in environment
        transform.position = _carStartPosition; 
        transform.rotation = _carStartRotation;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;

        foreach (var cube in rewardCubes)
        {
            cube.SetActive(true);
        }

        reward = 0;
        // Starts/Resets the episode timer
        _episodeStart = DateTime.Now;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float frontSensor = 1;
            
        float leftFrontSensor = 1;
        float leftSensor = 1;
            
        float rightFrontSensor = 1;
        float rightSensor = 1;
            
            // Front sensor
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(
                new Vector3(0, 0, 1).normalized), out hit, SensorRange, _sensorLayerMask))
            {
                Color color = Color.green;
                if (hit.distance / SensorRange < (2 / 3f))
                {
                    color = Color.yellow;
                }
                if (hit.distance / SensorRange < (1 / 3f))
                {
                    color = Color.red;
                }
                frontSensor = hit.distance / SensorRange;
                Debug.DrawRay(transform.position, transform.TransformDirection(
                    new Vector3(0, 0, 1).normalized)*hit.distance, color);
            }
            else
            {
                Debug.DrawRay(transform.position , transform.TransformDirection(
                                                       new Vector3(0, 0, 1).normalized)*SensorRange, Color.green);
            }

            if (Physics.Raycast(transform.position, transform.TransformDirection(
                new Vector3(0.5f, 0, 1).normalized), out hit, SensorRange, _sensorLayerMask))
            {
                Color color = Color.green;
                if (hit.distance / SensorRange < (2 / 3f))
                {
                    color = Color.yellow;
                }

                if (hit.distance / SensorRange < (1 / 3f))
                {
                    color = Color.red;
                }

                rightFrontSensor = hit.distance / SensorRange;
                Debug.DrawRay(transform.position, transform.TransformDirection(
                    new Vector3(0.5f, 0, 1).normalized) * hit.distance, color);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(
                                                      new Vector3(0.5f, 0, 1).normalized)*SensorRange, Color.green);
            }

            if (Physics.Raycast(transform.position, transform.TransformDirection(
                new Vector3(1, 0, 0).normalized), out hit, SensorRange, _sensorLayerMask))
            {
                Color color = Color.green;
                if (hit.distance / SensorRange < (2 / 3f))
                {
                    color = Color.yellow;
                }
                if (hit.distance / SensorRange < (1 / 3f))
                {
                    color = Color.red;
                }
                rightSensor = hit.distance / SensorRange;
                    Debug.DrawRay(transform.position, transform.TransformDirection(
                        new Vector3(1, 0, 0).normalized)*hit.distance, color);

            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(
                                                      new Vector3(1, 0, 0).normalized)*SensorRange, Color.green);
            }

            if (Physics.Raycast(transform.position, transform.TransformDirection(
                new Vector3(-0.5f, 0, 1).normalized), out hit, SensorRange, _sensorLayerMask))
            {
                Color color = Color.green;
                if (hit.distance / SensorRange < (2 / 3f))
                {
                    color = Color.yellow;
                }
                if (hit.distance / SensorRange < (1 / 3f))
                {
                    color = Color.red;
                }
                leftFrontSensor = hit.distance / SensorRange;
                    Debug.DrawRay(transform.position, transform.TransformDirection(
                        new Vector3(-0.5f, 0, 1).normalized)*hit.distance, color);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(
                                                      new Vector3(-0.5f, 0, 1).normalized)*SensorRange, Color.green);
            }

            if (Physics.Raycast(transform.position, transform.TransformDirection(
                new Vector3(-1, 0, 0).normalized), out hit, SensorRange, _sensorLayerMask))
            {
                Color color = Color.green;
                if (hit.distance / SensorRange < (2 / 3f))
                {
                    color = Color.yellow;
                }
                if (hit.distance / SensorRange < (1 / 3f))
                {
                    color = Color.red;
                }
                leftSensor = hit.distance / SensorRange;
                    Debug.DrawRay(transform.position, transform.TransformDirection(
                        new Vector3(-1, 0, 0).normalized)*hit.distance, color);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(
                                                      new Vector3(-1, 0, 0).normalized)*SensorRange, Color.green);
            }
            
            sensor.AddObservation(frontSensor);
            
            sensor.AddObservation(leftFrontSensor);
            sensor.AddObservation(leftSensor);
            
            sensor.AddObservation(rightFrontSensor);
            sensor.AddObservation(rightSensor);
            
            // XXL sensors
            float leftLeftFrontSensor = 1;
            float rightRightFrontSensor = 1;
            
            float leftFrontFrontSensor = 1;
            float rightFrontFrontSensor = 1;

            float leftLeftLeftFrontSensor = 1;
            float rightRightRightFrontSensor = 1;
            
            
            // leftLeftFrontSensor
            if (Physics.Raycast(transform.position, transform.TransformDirection(
                new Vector3(-0.75f, 0, 1).normalized), out hit, SensorRange, _sensorLayerMask))
            {
                Color color = Color.green;
                if (hit.distance / SensorRange < (2 / 3f))
                {
                    color = Color.yellow;
                }
                if (hit.distance / SensorRange < (1 / 3f))
                {
                    color = Color.red;
                }
                leftLeftFrontSensor = hit.distance / SensorRange;
                    Debug.DrawRay(transform.position, transform.TransformDirection(
                        new Vector3(-0.75f, 0, 1).normalized)*hit.distance, color);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(
                    new Vector3(-0.75f, 0, 1).normalized)*SensorRange, Color.green);
            }
            
            // rightRightFrontSensor
            if (Physics.Raycast(transform.position, transform.TransformDirection(
                new Vector3(0.75f, 0, 1).normalized), out hit, SensorRange, _sensorLayerMask))
            {
                Color color = Color.green;
                if (hit.distance / SensorRange < (2 / 3f))
                {
                    color = Color.yellow;
                }
                if (hit.distance / SensorRange < (1 / 3f))
                {
                    color = Color.red;
                }
                rightRightFrontSensor = hit.distance / SensorRange;
                    Debug.DrawRay(transform.position, transform.TransformDirection(
                        new Vector3(0.75f, 0, 1).normalized)*hit.distance, color);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(
                    new Vector3(0.75f, 0, 1).normalized)*SensorRange, Color.green);
            }
            
            // leftFrontFrontSensor
            if (Physics.Raycast(transform.position, transform.TransformDirection(
                new Vector3(-0.25f, 0, 1).normalized), out hit, SensorRange, _sensorLayerMask))
            {
                Color color = Color.green;
                if (hit.distance / SensorRange < (2 / 3f))
                {
                    color = Color.yellow;
                }
                if (hit.distance / SensorRange < (1 / 3f))
                {
                    color = Color.red;
                }
                leftFrontFrontSensor = hit.distance / SensorRange;
                    Debug.DrawRay(transform.position, transform.TransformDirection(
                        new Vector3(-0.25f, 0, 1).normalized)*hit.distance, color);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(
                    new Vector3(-0.25f, 0, 1).normalized)*SensorRange, Color.green);
            }
            
            // rightFrontFrontSensor
            if (Physics.Raycast(transform.position, transform.TransformDirection(
                new Vector3(0.25f, 0, 1).normalized), out hit, SensorRange, _sensorLayerMask))
            {
                Color color = Color.green;
                if (hit.distance / SensorRange < (2 / 3f))
                {
                    color = Color.yellow;
                }
                if (hit.distance / SensorRange < (1 / 3f))
                {
                    color = Color.red;
                }
                leftFrontFrontSensor = hit.distance / SensorRange;
                    Debug.DrawRay(transform.position, transform.TransformDirection(
                        new Vector3(0.25f, 0, 1).normalized)*hit.distance, color);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(
                    new Vector3(0.25f, 0, 1).normalized)*SensorRange, Color.green);
            }
            
            // leftLeftLeftFrontSensor
            if (Physics.Raycast(transform.position, transform.TransformDirection(
                new Vector3(-3f, 0, 1).normalized), out hit, SensorRange, _sensorLayerMask))
            {
                Color color = Color.green;
                if (hit.distance / SensorRange < (2 / 3f))
                {
                    color = Color.yellow;
                }
                if (hit.distance / SensorRange < (1 / 3f))
                {
                    color = Color.red;
                }
                leftLeftLeftFrontSensor = hit.distance / SensorRange;
                    Debug.DrawRay(transform.position, transform.TransformDirection(
                        new Vector3(-3f, 0, 1).normalized)*hit.distance, color);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(
                    new Vector3(-3f, 0, 1).normalized)*SensorRange, Color.green);
            }
            
            // rightRightRightFrontSensor
            if (Physics.Raycast(transform.position, transform.TransformDirection(
                new Vector3(3f, 0, 1).normalized), out hit, SensorRange, _sensorLayerMask))
            {
                Color color = Color.green;
                if (hit.distance / SensorRange < (2 / 3f))
                {
                    color = Color.yellow;
                }
                if (hit.distance / SensorRange < (1 / 3f))
                {
                    color = Color.red;
                } 
                rightRightFrontSensor = hit.distance / SensorRange;
                    Debug.DrawRay(transform.position, transform.TransformDirection(
                        new Vector3(3f, 0, 1).normalized)*hit.distance, color);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(
                    new Vector3(3f, 0, 1).normalized)*SensorRange, Color.green);
            }

            sensor.AddObservation(leftLeftFrontSensor);
            sensor.AddObservation(rightRightFrontSensor);
            
            sensor.AddObservation(leftFrontFrontSensor);
            sensor.AddObservation(rightFrontFrontSensor);
            
            sensor.AddObservation(leftLeftLeftFrontSensor);
            sensor.AddObservation(rightRightRightFrontSensor);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // Default output from PPO algorithm pre-clamps values into [-1, 1] range,
        // But we want this script to work with all possible algorithms so we clamp the value manually.
        xAxis = Mathf.Clamp(vectorAction[0], -1, 1);

        time = (DateTime.Now - _episodeStart).Seconds;
        speed = (_rigidbody.velocity.magnitude);
        
        // If the car is not moving (close to zero speed) (it is stuck somewhere), we end the episode
        if (_rigidbody.velocity.magnitude < 0.05)
        {
            EndEpisode();
        }

        // If the car is below the stage, we end the episode.
        if (transform.position.y < -1)
        {
            EndEpisode();
        }
        
        // If the episode lasts more than 10 minutes, end it (the car is stuck somewhere or has fallen of the stage)
        if ((DateTime.Now - _episodeStart).Minutes > 45)
        {
            EndEpisode();
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        // If the car collides with something other than the RewardCubes or the Road, which is obviously not good,
        // We end the current learning episode and start a new one
        if(!other.gameObject.CompareTag("Road"))
        {
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the car collides with a RewardCube
        if (other.gameObject.CompareTag("RewardCube"))
        {
            // The RewardCube is deactivated so the agent can't use it again to gain reward points
            other.gameObject.SetActive(false);
            // We give the agent the reward
            // According to the ML-Agents docs,
            // "The magnitude of any given reward should typically not be greater than 1.0
            // in order to ensure a more stable learning process."
            AddReward(0.0025f);
            reward += 0.0025f;
        }
    }
}
