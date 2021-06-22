using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Set moving speed multiplier.")]
    public float Speed = 30f;
    [Tooltip("Set boost speed to for boosting with shift")]
    public float BoostSpeed = 150f;
    [Tooltip("Set rotation sensitivity")]
    public float Sensitivity = 1.5f;

    private float xRot;
    private float yRot;
    private float currSpeed;
    private bool controlsActive = true;

    void Start()
    {
        if (PlayerPrefs.HasKey("sensitivity"))
            Sensitivity = PlayerPrefs.GetFloat("sensitivity");
        else
            PlayerPrefs.SetFloat("sensitivity", Sensitivity);
        currSpeed = Speed;
        yRot = -transform.eulerAngles.x;
        xRot = transform.eulerAngles.y;
        Selection.instance.OnPanelToggle += ChangeControlsActive;
        GameManager.instance.OnPause += ChangeControlsActive;
        ObjectEditor.instance.OnPanelChange += ChangeControlsActive;
    }

    public void OnSensitivityChange(float val)
    {
        Sensitivity = val;
        PlayerPrefs.SetFloat("sensitivity", val);
        GameManager.instance.UpdateSensitivitySlider();
    }

    private void ChangeControlsActive(bool b)
    {
        controlsActive = !b;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !GameManager.instance.IsPaused)
        {
            Selection.instance.Select(null);
            Selection.instance.Toggle();
        }
    }

    void FixedUpdate()
    {
        if (controlsActive)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                Move(BoostSpeed);
            else
                Move(Speed);
            Rotate();
        }
    }

    private void Rotate()
    {
        float x = Input.GetAxis("Mouse X") * Sensitivity;
        float y = Input.GetAxis("Mouse Y") * Sensitivity;

        xRot = (xRot + x) % 360;
        yRot = Mathf.Clamp(yRot + y, -90, 90);

        transform.rotation = Quaternion.Euler(-yRot, xRot, 0f);
    }

    private void Move(float speed)
    {
        currSpeed = Mathf.Lerp(currSpeed, speed, Time.fixedDeltaTime);
        Vector3 dir = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        transform.Translate(dir.normalized * currSpeed * Time.fixedDeltaTime, Space.World);
    }
}
