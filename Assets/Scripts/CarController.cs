using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    public bool Accelerating { get; private set; }
    public bool Braking { get; private set; }
    public bool Turning { get; private set; }
    public float MaxVelocity = 100000;
    //Direction of the front of the vehicle for the y axis. 1 means pointing totally upwards. -1 means pointing totally downwards.
    public float yModifier = 1;
    //Direction of the front of the vehicle for the x axis. 1 means pointing totally right. -1 means pointing totally left.
    public float xModifier = 0;

    public int Horsepower = 1500;
    public int BrakingPower = 6000;
    //Lower TurningRate is better. Represents the amount of fixed updates needed to complete a quarter of a turn at low speed.
    public int TurningRate = 600;

    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Accelerating = Input.GetKey(KeyCode.UpArrow);
        Braking = Input.GetKey(KeyCode.DownArrow);
        Turning = Input.GetKey(KeyCode.LeftArrow) ^ Input.GetKey(KeyCode.RightArrow); //one or the other, not both
    }

    void FixedUpdate()
    {
        //Order of operation is important
        if (Accelerating)
        {
            Accelerate();
        }
        if (Braking)
        {
            Decelerate();
        }
        if (Turning) {
            Turn();
        }
        UpdateDebugInformation();
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

    private void Turn()
    {
        if (_rb.velocity.magnitude == 0) return;

        //separate the 360 degrees into x segments, where x is the turning rate
        float rotation = 1f / TurningRate;
        if (Input.GetKey(KeyCode.LeftArrow)) // there's no reason to chose right as the default instead of left, I just had to pick one
        {
            rotation = -rotation;
        }
        
        //turning right makes the front of the car aim leftwards when the ymodifier is negative (since we're aiming down, turning right goes left)
        xModifier += yModifier < 0 ? rotation : -rotation;
        //turning right makes the front of the car aim upwards when the xmodifier is negative (since we're looking left, turning right goes up)
        //"                                      " aim downwards when the xmodifier is positive (since we're looking right, turning right does down)
        yModifier += xModifier < 0 ? rotation : -rotation;


        //ensure values are between 1 and -1
        xModifier = Math.Max(Math.Min(xModifier, 1), -1);
        yModifier = Math.Max(Math.Min(yModifier, 1), -1);

        var newZRotation = yModifier * 360;
        if (xModifier < 0)
            newZRotation = -newZRotation;

        this.transform.rotation = Quaternion.Euler(0,0,newZRotation);
    }

    #region UI

    public Text CurrentVelocityText = null;
    public Text CurrentXModifierText = null;
    public Text CurrentYModifierText = null;
    public Text IsAcceleratingText = null;
    public Text IsBrakingText = null;

    private void UpdateDebugInformation()
    {
        string CurrentVelocityText = $"Velocity = {_rb.velocity.magnitude}";
        if (this.CurrentVelocityText.text != CurrentVelocityText)
        {
            this.CurrentVelocityText.text = CurrentVelocityText;
        }

        string CurrentXModifierText = $"X modifier = {this.xModifier}";
        if (this.CurrentXModifierText.text != CurrentXModifierText)
        {
            this.CurrentXModifierText.text = CurrentXModifierText;
        }

        string CurrentYModifierText = $"Y modifier = {this.yModifier}";
        if (this.CurrentYModifierText.text != CurrentYModifierText)
        {
            this.CurrentYModifierText.text = CurrentYModifierText;
        }

        IsAcceleratingText.color = Accelerating ? Color.green : Color.gray;
        IsBrakingText.color = Braking ? Color.red : Color.gray;
    }

    #endregion
}
