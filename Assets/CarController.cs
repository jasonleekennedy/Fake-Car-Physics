using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarController : MonoBehaviour
{
    public Transform[] wheels;
    public float rollingFriction = 0.05f;
    public float slidingFriction = 0.95f;
    public float motorPower = 100f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float weightPerWheel = rb.mass * Physics.gravity.magnitude / 4f;
        float turn = Input.GetAxisRaw("Horizontal");
        float power = Input.GetAxisRaw("Vertical");
        Debug.Log(power);

        for (int i = 0; i < wheels.Length; i++)
        {
            Vector3 netForce = Vector3.zero;
            Vector3 wheelLocalVelocity = localVelocity(wheels[i].localPosition);

            if (i < 2) //frontWheels
            {
                wheels[i].localEulerAngles = new Vector3(0f, turn * 30f, 0f);
            }
            else //backWheels
            {
                netForce += wheels[i].forward * motorPower * power;
            }

            //rolling Drag
            netForce -= wheels[i].forward * weightPerWheel * rollingFriction * Vector3.Dot(wheels[i].forward, wheelLocalVelocity);

            // Side Drag
            netForce -= wheels[i].right * weightPerWheel * slidingFriction * Vector3.Dot(wheels[i].right, wheelLocalVelocity);

            netForce = Vector3.ClampMagnitude(netForce, weightPerWheel * slidingFriction);

            Debug.DrawRay(wheels[i].position, netForce);
            rb.AddForceAtPosition(netForce, wheels[i].position, ForceMode.Force);
        }
    }

    Vector3 localVelocity(Vector3 position)
    {
        return rb.velocity + Mathf.Sign(position.z) * transform.right * (rb.angularVelocity.y * position.magnitude);
    }
}
