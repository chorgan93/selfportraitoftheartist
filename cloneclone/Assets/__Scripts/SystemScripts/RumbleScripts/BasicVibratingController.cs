#if UNITY_SWITCH
using UnityEngine;
using nn.hid;

/// <summary>
/// Simple class that takes care of vibration-related state, initialization, 
/// and vibration value setting for a given logical controller. 
/// </summary>
public class BasicVibratingController
{
    private const int VIBRATION_DEVICE_COUNT_MAX = 2;   // Two is currently the maximum number of vibration devices (left and right).

    private NpadId m_npadId;    // The NpadId for this controller.
    private NpadStyle m_previousNpadStyle = NpadStyle.Invalid;  // Use this to track changes in the NpadStyle so we know when to update vibration devices.
    private VibrationDeviceHandle? m_vibrationDeviceHandleLeft = null;     // Make the devices nullable to indicate when they're valid easily.
    private VibrationDeviceHandle? m_vibrationDeviceHandleRight = null;

    /// <summary>
    /// Updates the vibration device handles. This must be called at least as often as the controller styles change.
    /// This function is lightweight enough to be called every frame.
    /// Assumes Npad initialization has already occurred.
    /// </summary>
    public void UpdateVibrationDeviceHandles()
    {
        // Assumes Npad initialization has already happened
        NpadStyle currentStyle = Npad.GetStyleSet(m_npadId);

        if (currentStyle == m_previousNpadStyle)
        {
            return;
        }

        m_previousNpadStyle = currentStyle;

        // We should check if a device goes from inactive to active so we can clear any vibration it's still outputting from when it became inactive.
        bool wasLeftDeviceNull  = (m_vibrationDeviceHandleLeft  == null);
        bool wasRightDeviceNull = (m_vibrationDeviceHandleRight == null);

        // Set these as null. By the end of the function, any active vibration devices will have a set handle.
        m_vibrationDeviceHandleLeft = null;
        m_vibrationDeviceHandleRight = null;

        if (currentStyle == NpadStyle.None || currentStyle == NpadStyle.Invalid)
        {
            // Invalid or disconnected controller.
            return;
        }

        VibrationDeviceHandle[] vibrationDeviceHandles = new VibrationDeviceHandle[VIBRATION_DEVICE_COUNT_MAX]; // Temporary buffer to get handles
        int vibrationDeviceCount = Vibration.GetDeviceHandles(vibrationDeviceHandles, VIBRATION_DEVICE_COUNT_MAX, m_npadId, currentStyle);

        for (int i = 0; i < vibrationDeviceCount; i++)
        {
            Vibration.InitializeDevice(vibrationDeviceHandles[i]);

            VibrationDeviceInfo vibrationDeviceInfo = new VibrationDeviceInfo(); // This is basically just used as an 'out' parameter for GetDeviceInfo
            Vibration.GetDeviceInfo(ref vibrationDeviceInfo, vibrationDeviceHandles[i]);

            // Cache references to our device handles.
            switch (vibrationDeviceInfo.position)
            {
                case VibrationDevicePosition.Left:
                    m_vibrationDeviceHandleLeft = vibrationDeviceHandles[i];
                    if (wasLeftDeviceNull)
                    {
                        SetLeftVibration(new VibrationValue()); // Clear out any old value the device may still want to output
                    }
                    break;

                case VibrationDevicePosition.Right:
                    m_vibrationDeviceHandleRight = vibrationDeviceHandles[i];
                    if (wasRightDeviceNull)
                    {
                        SetRightVibration(new VibrationValue()); // Clear out any old value the device may still want to output
                    }
                    break;

                default:    // This should never happen
                    Debug.Assert(false, "Invalid VibrationDevicePosition specified");
                    break;
            }
        }
    }

    /// <summary>
    /// Constructor for BasicVibrationController. Does some basic initialization. 
    /// </summary>
    /// <param name="npadId">The controller/player channel (No1, No2, [...], No8, Handheld)</param>
    public BasicVibratingController(NpadId npadId)
    {
        m_npadId = npadId;

        UpdateVibrationDeviceHandles(); // Do an initial setting of the device handles
    }

    /// <summary>
    /// Sets the vibration for the left side of the controller.
    /// The device will continue to output this value until the next time this function is called.
    /// In other words, this value does not need to be updated every frame or vibration update cycle. 
    /// It's okay to call this function when the device is inactive.
    /// </summary>
    /// <param name="vibrationValue">The value to output</param>
    public void SetLeftVibration(VibrationValue vibrationValue)
    {
        if (m_vibrationDeviceHandleLeft != null)
        {
            Vibration.SendValue(m_vibrationDeviceHandleLeft.Value, vibrationValue);
        }
    }

    /// <summary>
    /// Sets the vibration for the right side of the controller.
    /// The device will continue to output this value until the next time this function is called.
    /// In other words, this value does not need to be updated every frame or vibration update cycle. 
    /// It's okay to call this function when the device is inactive.
    /// </summary>
    /// <param name="vibrationValue">The value to output</param>
    public void SetRightVibration(VibrationValue vibrationValue)
    {
        if (m_vibrationDeviceHandleRight != null)
        {
            Vibration.SendValue(m_vibrationDeviceHandleRight.Value, vibrationValue);
        }
    }
}
#endif
