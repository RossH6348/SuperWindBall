using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

public class WindControls : MonoBehaviour
{

    //Gyroscope part.
    private Gyroscope gyroControl;

    //Microphone input part with the help of this guy named GameDevStev: https://www.reddit.com/r/Unity3D/comments/49wuld/best_way_to_implement_microphone_input/
    AudioClip microphoneInput;
    bool microphoneInitialized = false;
    private float sensitivity = 0.2f;
    private float maxLevel = 1.0f;

    //UI stuff.
    [SerializeField] private Image windSpeedImage;
    [SerializeField] private Text debugText;

    [SerializeField] private Rigidbody ballRigid;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log(SystemInfo.deviceName);
        if (SystemInfo.supportsGyroscope)
        {
            Debug.Log("Gyroscope found!");
            Input.gyro.enabled = true;
            gyroControl = Input.gyro;
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }

        if (Permission.HasUserAuthorizedPermission(Permission.Microphone) && Microphone.devices.Length > 0)
        {
            microphoneInput = Microphone.Start(Microphone.devices[0], true, 999, 44100);
            microphoneInitialized = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (gyroControl != null)
            transform.rotation = gyroControl.attitude;

        if (microphoneInitialized)
        {

            int dec = 256;
            float[] waveData = new float[dec];
            int micPosition = Microphone.GetPosition(null) - (dec + 1); // null means the first microphone
            microphoneInput.GetData(waveData, micPosition);

            // Getting a peak on the last 128 samples
            float levelMax = 0;
            for (int i = 0; i < dec; i++)
            {
                float wavePeak = waveData[i] * waveData[i];
                if (levelMax < wavePeak)
                {
                    levelMax = wavePeak;
                }
            }
            float level = Mathf.Sqrt(Mathf.Sqrt(levelMax));

            debugText.text = level.ToString();

            //Get height of the game window.
            int HEIGHT = Screen.height - 32;

            //Need to be a certain sensitivity.
            level -= sensitivity;
            if (level < 0.0f)
                level = 0.0f;

            float ratio = level / maxLevel;
            if (ratio > 1.0f)
                ratio = 1.0f;

            windSpeedImage.rectTransform.sizeDelta = new Vector2(64.0f, (float)HEIGHT * ratio);
            windSpeedImage.color = new Color(ratio, 1.0f - ratio, 0.0f);

            Vector3 flatForward = transform.forward;
            flatForward = new Vector3(flatForward.x, 0.0f, flatForward.z).normalized;

            ballRigid.velocity += flatForward * ratio * 0.5f;

            if (ballRigid.velocity.magnitude >= 2.0f)
                ballRigid.velocity = ballRigid.velocity.normalized * 2.0f;

        }

    }

    public void onRecalibrateClick()
    {
        if (gyroControl != null) {
            gyroControl.enabled = false;
            transform.rotation = Quaternion.identity;
            gyroControl.enabled = true;
        }
    }
}
