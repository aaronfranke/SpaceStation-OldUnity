
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : Player
{

    public Transform movementTransform;
    public Transform movementTransformRight;
    public bool isMoving = true;

    void Update()
    {
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
        {
            movementTransform = movementTransformRight;
        }

        Toggles();
        SprintCheck();

        PlayerTasks();

        if (isMoving)
        {
            if (onStation)
            {
                HorizontalMovement();
                GroundCheck();
            }

            if (jetpackEnabled)
            {
                Jetpacking();
            }
        }

        UpdateInfoHUD();

        if (Input.GetKey(KeyCode.J))
        {
            World.worldObject.transform.position = Vector3.zero;
            Station.stationObject.transform.localPosition = Vector3.one * 100000.0f;
        }
        else if (Input.GetKey(KeyCode.K))
        {
            Station.stationObject.transform.localPosition = Vector3.one * 100000.0f;
            World.worldObject.transform.localPosition -= transform.position;
        }
        else
        {
            World.worldObject.transform.position = Vector3.zero;
            Station.stationObject.transform.localPosition = Vector3.zero;
        }
    }

    protected void Toggles()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
            {
                jetpackEnabled = false;
                onStation = !jetpackEnabled;
                transform.localPosition = new Vector3(5, 0, 5);
            }
            if (OVRInput.Get(OVRInput.Button.Back))
            {
                velocityDampening = !velocityDampening;
            }
            return;
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))
        {
            isMoving = !isMoving;
        }

        if (OVRInput.GetDown(OVRInput.Button.Back))
        {
            jetpackEnabled = !jetpackEnabled;
            onStation = !jetpackEnabled;
        }

        syncPosition.CmdSyncStats(velocityDampening, jetpackEnabled, onStation);
    }

    protected void SprintCheck()
    {
        if (Input.GetKey(KeyCode.LeftShift) == true)
        {
            movementSpeed = 7;
        }
        else
        {
            //movementSpeed = 4;
            movementSpeed = 10;
        }
    }

    protected void GroundCheck()
    {
        onGround = false;
        RaycastHit hit;
        onGround = Physics.Raycast(transform.position, -transform.up, out hit);
        if (hit.distance < 1.1)
        {
            onGround = true;
        }
    }

    protected void UpdateInfoHUD()
    {
        if (InGameMenu.speedText == null)
        {
            return;
        }
        float speed = 0;
        string speedString = "0.0";
        if (onStation)
        {
            speed = (positionOffset.magnitude / Time.deltaTime) + physicsRoot.velocity.magnitude;
        }
        else
        {
            speed = physicsRoot.velocity.magnitude;
        }
        if (speed > 10)
        {
            speedString = Mathf.RoundToInt(speed).ToString();
        }
        else
        {
            speedString = speed.ToString("F1");
        }

        InGameMenu.healthText.text = "Health: " + Mathf.RoundToInt(health) + "% ";
        if (health < 10)
        {
            InGameMenu.healthText.color = new Color(0.8f, 0.2f, 0.2f);
        }
        else
        {
            InGameMenu.healthText.color = new Color(0.8f, 0.8f, 0.8f);
        }

        InGameMenu.energyText.text = "Energy: " + Mathf.RoundToInt(energy) / 0.0f + " kJ ";
        if (energy < 10)
        {
            InGameMenu.energyText.color = new Color(0.8f, 0.2f, 0.2f);
        }
        else
        {
            InGameMenu.energyText.color = new Color(0.8f, 0.8f, 0.8f);
        }
        InGameMenu.speedText.text = "Speed: " + speedString + " m/s ";

        InGameMenu.referenceFrameText.text = "Rel-to: " + transform.parent.name;

        if (jetpackEnabled)
        {
            InGameMenu.jetpackText.text = "Jetpack: Enabled";
        }
        else
        {
            InGameMenu.jetpackText.text = "Jetpack: Disabled";
        }

        if (velocityDampening)
        {
            InGameMenu.velocityDampeningText.text = "Vel-Damp: Enabled";
        }
        else
        {
            InGameMenu.velocityDampeningText.text = "Vel-Damp: Disabled";
        }

    }

    protected Vector3 positionOffset = Vector3.zero;

    protected void HorizontalMovement()
    {
        // Horizontal Movement, checks for diagonals first, then single directions. 
        // Moving sideways is slow, moving backwards is slower. Moving forwards is fastest. Diagonals are in-between. 
        positionOffset = Vector3.zero;
        Vector2 touchpad = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);

        if (touchpad.y > 0.1 && (touchpad.x > 0.1 || touchpad.x < -0.1))
        {
            positionOffset += Time.deltaTime * movementSpeed * 0.60f * movementTransform.forward * touchpad.y;
            positionOffset += Time.deltaTime * movementSpeed * 0.60f * movementTransform.right * touchpad.x;
        }
        else if (touchpad.y < -0.1 && (touchpad.x > 0.1 || touchpad.x < -0.1))
        {
            positionOffset += Time.deltaTime * movementSpeed * 0.35f * movementTransform.forward * touchpad.y;
            positionOffset += Time.deltaTime * movementSpeed * 0.35f * movementTransform.right * touchpad.x;
        }
        else if (touchpad.y > 0.1)
        {
            positionOffset += Time.deltaTime * movementSpeed * 1.00f * movementTransform.forward * touchpad.y;
        }
        else if (touchpad.y < -0.1)
        {
            positionOffset += Time.deltaTime * movementSpeed * 0.45f * movementTransform.forward * touchpad.y;
        }
        else if (touchpad.x > 0.1 || touchpad.x < -0.1)
        {
            positionOffset += Time.deltaTime * movementSpeed * 0.70f * movementTransform.right * touchpad.x;
        }
        physicsRoot.transform.position += positionOffset;
    }

    protected void Jumping()
    {
        // In order to jump, you must have a low vertical velocity (not being able to jump while walking uphill is intentional), and be near the ground. 
        if (Input.GetAxis("Jump") > 0.1 && onGround == true)
        {
            physicsRoot.velocity = new Vector3(physicsRoot.velocity.x, jumpStrength, physicsRoot.velocity.z);
        }
    }

    protected void Jetpacking()
    {
        Vector2 touchpad = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        if (touchpad.y > 0.1 || touchpad.y < -0.1)
        {
            physicsRoot.velocity += Time.deltaTime * movementSpeed * jetpackSpeed * movementTransform.forward * touchpad.y / physicsRoot.mass;
        }
        else if (velocityDampening)
        {
            Dampening(movementTransform.forward);
        }
        if (touchpad.x > 0.1 || touchpad.x < -0.1)
        {
            physicsRoot.velocity += Time.deltaTime * movementSpeed * jetpackSpeed * movementTransform.right * touchpad.x / physicsRoot.mass;
        }
        else if (velocityDampening)
        {
            Dampening(movementTransform.right);
        }
        if (Input.GetAxis("Jump") > 0.1 || Input.GetAxis("Jump") < -0.1)
        {
            physicsRoot.velocity += Time.deltaTime * movementSpeed * jetpackSpeed * movementTransform.up * Input.GetAxis("Jump") / physicsRoot.mass;
        }
        else if (velocityDampening)
        {
            Dampening(movementTransform.up);
        }
        movementTransform.Rotate(0, 0, -Input.GetAxis("Roll") * 60 * Time.deltaTime);
    }

    protected void Dampening(Vector3 direction)
    {
        Vector3 projection = Vector3.Project(physicsRoot.velocity, direction);
        float directionScaler = Vector3.Dot(projection.normalized, direction);
        Vector3 maxSlowdown = Time.deltaTime * directionScaler * direction * jetpackSpeed * movementSpeed / physicsRoot.mass;
        byte maxBeforeFull = movementSpeed;
        if (projection.magnitude > maxBeforeFull)
        {
            physicsRoot.velocity -= maxSlowdown;
        }
        else
        {
            physicsRoot.velocity -= projection.magnitude * maxSlowdown / (maxBeforeFull * 2);
        }
    }

}
