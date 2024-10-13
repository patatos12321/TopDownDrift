using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    #region UI

    public Text CurrentVelocityText = null;
    public Text CurrentXModifierText = null;
    public Text CurrentYModifierText = null;
    public Text IsAcceleratingText = null;
    public Text IsBrakingText = null;

    private void UpdateDebugInformation() 
    {
        string CurrentVelocityText = $"Velocity = {_rb.velocity.magnitude}";
        if (this.CurrentVelocityText.text != CurrentVelocityText) { 
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
