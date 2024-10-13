using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public bool Accelerating { get; private set; }
    public bool Braking { get;  private set; }
    public float MaxVelocity = 100000;
    //Direction of the front of the vehicle for the y axis. 1 means pointing totally upwards. -1 means pointing totally downwards.
    public float yModifier = 1;
    //Direction of the front of the vehicle for the x axis. 1 means pointing totally right. -1 means pointing totally left.
    public float xModifier = 0;

    public int Horsepower = 1500;
    public int BrakingPower = 6000;

    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Accelerating = true;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            Accelerating = false;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Braking = true;
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            Braking = false;
        }
    }

    void FixedUpdate()
    {
        if (Accelerating)
        {
            Accelerate();
        }
        if (Braking)
        {
            Decelerate();
        }

        Debug.Log($"My current position is : Ymodifier = {yModifier}\r\nXmodifier = {xModifier}\r\nVelocity={_rb.velocity}");

    }

    private void Accelerate()
    {
        if (IsAtMaxSpeed())
        {
            return;
        }

        _rb.AddForce(new Vector2(Horsepower * xModifier * Time.deltaTime, Horsepower * yModifier * Time.deltaTime));
    }

    private bool IsAtMaxSpeed()
    {
        return _rb.velocity.magnitude > MaxVelocity;
    }

    private void Decelerate()
    {
        //todo1: fix exploit of breaking being faster than going forward.
        //todo1-notes: breaking should not take into account direction of vehicle, only direction of inertia
        _rb.AddForce(new Vector2(BrakingPower * -xModifier * Time.deltaTime, BrakingPower * -yModifier * Time.deltaTime));
    }
}
