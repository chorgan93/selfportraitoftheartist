#if UNITY_SWITCH
using UnityEngine; 
using nn.hid;
using System.Collections.Generic;

/// <summary>
/// The main logic script for the game.
/// Primarily handles player input and movement. 
/// </summary>
public class GameLogic : MonoBehaviour
{
    public float maxSpeed = 10.0f;
    public Camera mainCamera;
    public GameObject player;

    private CharacterController m_characterController;
    private NpadId[] m_npadIds = { NpadId.No1, NpadId.Handheld };
    private NpadId m_currentNpadId = NpadId.Handheld;   // Tracks which NpadId we're using. Default to Handheld. If Handheld isn't available, use No1.
    private NpadStyle m_currentNpadStyle = NpadStyle.None;
    private bool m_wasHandheldActiveLastFrame = false;          // Used to detect when Joy-Con controllers are attached to the system
    private bool m_wereAppletButtonsPressedLastFrame = false;   // Used to detect changes in the "pressed" state of buttons used to open the applet
    private bool m_hadWallCollisionLastFrame = false;   // Used to detect changes in collision state

#region VibrationSpecificMembers

    public string vibrationHitEffect = "SampleA.bnvib"; // This will be played whenever the player bumps into a wall
    public float MaxTextureAmplitude = 0.5f; // This value was arrived at by testing the particular effect with different controllers.

    private const float m_vibrationInterval = 0.005f;   // BNVIB files are sampled at a rate of 200 Hz == every 5 milliseconds == 0.005 seconds
    private Dictionary<NpadId, BasicVibratingController> m_vibratingControllers = new Dictionary<NpadId, BasicVibratingController>(); // Maps NpadId's to the corresponding vibration functionality
    private VibrationFileResource m_movementVibrationEffectFile; // BNVIB data for vibrationHitEffect file
    private VibrationFilePlayer m_movementVibrationEffect; // Manages state for the vibrationHitEffect file
    // Since BNVIB files are updated at a different rate than Update() occurs, set this value in Update() and then incorporate it in the vibration update
    private float m_textureRumble = 0.0f;
    /* Since most controller configurations have a left and right vibration device, a simple 
     * way to add immersion is to make vibrations stronger in the left or right device based on location. 
     * We can do this by multiplying the right amplitudes by a value in the range [0.0, 1.0] representing how far to the right the player is,
     * and by multiplying the left amplitudes by 1.0 minus the right value (e.g., leftPanMultiplier = (1.0 - m_rightPanMultiplier))
    */
    private float m_rightPanMultiplier = 0.0f; 

#endregion

    // Use this for initialization
    void Start ()
    {
        // Not vibration-related
        Debug.Assert(mainCamera != null);
        Debug.Assert(player != null && player.GetComponent<CharacterController>() != null);

        InitializeNpad();

        m_wasHandheldActiveLastFrame = Npad.GetStyleSet(NpadId.Handheld) != NpadStyle.None;
        if (!m_wasHandheldActiveLastFrame && (Npad.GetStyleSet(NpadId.No1) != NpadStyle.None))
        {
            // No1 is attached and there are no controllers attached for Handheld, 
            // so we're safe to go with No1.
            m_currentNpadId = NpadId.No1;
        }

        m_characterController = player.GetComponent<CharacterController>();

        // Vibration-related
        InitializeVibration();
    }
    
    // Update is called once per frame
    void Update ()
    {
        UpdateControllerConfigurations(false); 

        NpadState npadState = new NpadState();
        Npad.GetState(ref npadState, m_currentNpadId, m_currentNpadStyle);

        bool areAppletButtonsPressed = false;
        if ((npadState.buttons & (NpadButton.Minus | NpadButton.Plus)) != 0)
        {
            areAppletButtonsPressed = true;
            if (!m_wereAppletButtonsPressedLastFrame)
            {
                UpdateControllerConfigurations(true);
            }
        }

        m_wereAppletButtonsPressedLastFrame = areAppletButtonsPressed;

        // We could get the NpadState again here for the (possibly) new controller,
        // but (a) that's wasteful in the majority of cases
        // and (b) there's nothing wrong with getting last known good data from the previously active controller

        Vector3 baseMovement = InputToMovementData(m_currentNpadId, npadState);

        // Note: Diagonals will be faster with this movement method, but it's good enough for demo purposes
        Vector3 scaledMovement = baseMovement * maxSpeed * Time.deltaTime;

        m_characterController.Move(scaledMovement);

        // Update vibration-related stuff
        foreach (KeyValuePair<NpadId, BasicVibratingController> vibratingControllerPair in m_vibratingControllers)
        {
            // If the controller setup changes, we need to update the vibration device handles.
            vibratingControllerPair.Value.UpdateVibrationDeviceHandles();
        }
        ReactToCollisions(m_characterController);
        UpdateMovementRumbleData(m_characterController);
    }

    /// <summary>
    /// If needed or requested, calls the Controller Applet, ensuring that unused wireless
    /// controllers are deactivated and that the desired NpadId is set as the current one.
    /// </summary>
    /// <param name="mustShowApplet">Forces the controller applet to open, even
    /// if the current controller states do not require it</param>
    void UpdateControllerConfigurations(bool mustShowApplet)
    {
        bool isHandheldAttached = Npad.GetStyleSet(NpadId.Handheld) != NpadStyle.None;
        bool isNo1Attached = Npad.GetStyleSet(NpadId.No1) != NpadStyle.None;

        if (   mustShowApplet   // Force applet to open
            || (!isHandheldAttached && !isNo1Attached) // No controllers attached
            || (isNo1Attached && (m_currentNpadId == NpadId.Handheld))  // No1 just became active
            || (!isNo1Attached && (m_currentNpadId == NpadId.No1))      // Connection to No1 was just lost
            || (isHandheldAttached && !m_wasHandheldActiveLastFrame))   // Joy-Con controllers were just attached to Switch
        {
            ShowControllerApplet(); // Either no controllers attached or too many

            // At this point, the only way No1 can be active is if it's been chosen by the player 
            // as the active controller. Selecting handheld deactivates it. 
            // We need to call GetStyleSet() again because the state has most likely changed.
            if (Npad.GetStyleSet(NpadId.No1) != NpadStyle.None)
            {
                m_currentNpadId = NpadId.No1;
            }
            else
            {
                m_currentNpadId = NpadId.Handheld;
            }
        }

        // If the user attaches controllers used for No1 to make Handheld mode, 
        // isHandheldAttached will report as false. As a result, we must check
        // the value again instead of just using isHandheldAttached.
        m_wasHandheldActiveLastFrame = Npad.GetStyleSet(NpadId.Handheld) != NpadStyle.None;
    }

    /// <summary>
    /// Does all Npad-related initialization for the scene.
    /// </summary>
    void InitializeNpad()
    {
        Npad.Initialize();
        // Not using Input Manager
        Npad.SetSupportedStyleSet(NpadStyle.FullKey | NpadStyle.JoyDual | NpadStyle.JoyLeft | NpadStyle.JoyRight | NpadStyle.Handheld);
        NpadJoy.SetHoldType(NpadJoyHoldType.Horizontal);
        NpadJoy.SetHandheldActivationMode(NpadHandheldActivationMode.Dual);
        Npad.SetSupportedIdType(m_npadIds);
    }

    /// <summary>
    /// For a given NpadId, returns a Vector3 that holds corresponding movement data.
    /// The x and y for the Vector3 will be in the range [-1.0, 1.0], and the z-value will be 0.
    /// </summary>
    /// <param name="npadId">The NpadId of the controller to get movement data from</param>
    /// <param name="npadState">The state of the controller that corresponds to npadId</param>
    /// <returns>A Vector3 that holds movement data. 
    /// The x and y for the Vector3 will be in the range [-1.0, 1.0], and the z-value will be 0.</returns>
    Vector3 InputToMovementData(NpadId npadId, NpadState npadState)
    {
        m_currentNpadStyle = Npad.GetStyleSet(npadId);

        if (m_currentNpadStyle == NpadStyle.None)
        {
            return Vector3.zero;
        }

        // If the style has two joysticks, use the left stick. Otherwise, use the right stick.
        AnalogStickState analogStickState = npadState.analogStickL;
        if (m_currentNpadStyle == NpadStyle.JoyRight)
        {
            analogStickState = npadState.analogStickR;
        }

        // Scale dx and dy so they're in the range [-1.0, 1.0]
        float dx = (float)(analogStickState.x) / AnalogStickState.Max;
        float dz = (float)(analogStickState.y) / AnalogStickState.Max;

        // If using just one Joy-Con held with two hands, we need to programmatically rotate the analog stick input
        if (m_currentNpadStyle == NpadStyle.JoyRight) // We did this check above, but do it again here for simplicity
        {
            Swap(ref dx, ref dz);
            dz *= -1.0f;
        }
        else if (m_currentNpadStyle == NpadStyle.JoyLeft)
        {
            Swap(ref dx, ref dz);
            dx *= -1.0f;
        }

        return new Vector3(dx, 0.0f, dz);
    }

    /// <summary>
    /// Based on player position and movement, generates a vibration amplitude
    /// and panning value (for panning vibrations left or right).
    /// There are no vibration-related API calls in this function.
    /// </summary>
    /// <param name="characterController">CharacterController for the object to base speed and position on</param>
    void UpdateMovementRumbleData(CharacterController characterController)
    {
        // Get x position so we can pan the vibration left/right accordingly.
        // We don't need to worry about going out of screen bounds in this case because we know that never happens in our scene.
        Vector3 screenPos = mainCamera.WorldToScreenPoint(player.transform.position);
        m_rightPanMultiplier = screenPos.x / Screen.width;

        /* Use Perlin noise to provide some "texture" to the surface we're moving over.
         * (Note this update is happening at a higher rate (200 Hz) than transform.position is being updated,
         *  so we might consider lerping values to get all of the in-between values 
         *  rather than using the same one several times per frame,
         *  but that's not really what this example is about, so that's left as an exercise for the reader.)
        */
        float noiseValue = Mathf.Clamp01(Mathf.PerlinNoise(screenPos.x, screenPos.y)); // According to Mathf.PerlinNoise documentation, PerlinNoise's result may exceed 1.0f

        // The effect should be more subtle at lower speeds, so vary the intensity based on speed
        float speedMultiplier = Mathf.Clamp01(characterController.velocity.sqrMagnitude / (maxSpeed * maxSpeed));

        m_textureRumble = MaxTextureAmplitude * speedMultiplier * noiseValue;
    }

    /// <summary>
    /// Shows the controller applet, allowing users to choose their controller configuration.
    /// </summary>
    void ShowControllerApplet()
    {
        // Set the Arguments For the Applet
        ControllerSupportArg controllerSupportArgs = new ControllerSupportArg();
        controllerSupportArgs.SetDefault();
        controllerSupportArgs.enableSingleMode = true;

        // Suspend Unity Processes to Call the Applet
        UnityEngine.Switch.Applet.Begin(); // Always call before calling a system applet
        
        // Call the Applet
        nn.hid.ControllerSupport.Show(controllerSupportArgs);

        // Resume the Suspended Unity Processes
        UnityEngine.Switch.Applet.End(); //always call after calling a system applet
    }

    /// <summary>
    /// Convenience function that swaps the two given values.
    /// </summary>
    /// <typeparam name="T">The type of the values to be swapped</typeparam>
    /// <param name="a">The first value to swap</param>
    /// <param name="b">The second value to swap</param>
    static void Swap<T>(ref T a, ref T b)
    {
        T tmp = a;
        a = b;
        b = tmp;
    }

#region VibrationSpecificFunctions

    /// <summary>
    /// Does all vibration-related initialization for the scene.
    /// Assumes Npad initialization has already happened.
    /// Specifically, it: 
    /// - Initializes all controllers that will be used
    /// - Loads the BNVIB file that will be used
    /// - Kicks off the vibration update function
    /// </summary>
    void InitializeVibration()
    {
        // Initialize vibration for all controllers we plan to support
        foreach (NpadId npadId in m_npadIds)
        {
            m_vibratingControllers.Add(npadId, new BasicVibratingController(npadId));
        }

        // Load binary vibration file and initialize it
        byte[] fileBuffer = Resources.Load<TextAsset>(vibrationHitEffect).bytes;
        m_movementVibrationEffectFile = new VibrationFileResource(fileBuffer);
        m_movementVibrationEffect = new VibrationFilePlayer(m_movementVibrationEffectFile);

        InvokeRepeating("VibrationUpdate", 0.1f, m_vibrationInterval);    // Invoke every 5ms/at a rate of 200 Hz (as discussed in documentation)
    }

    /// <summary>
    /// This method should be called 200 times per second (200 Hz), which is once every 5 milliseconds.
    /// This is important when using BNVIB files, since they're sampled at a rate of 200 Hz. 
    /// If the application isn't using BNVIB files, you can just set vibrations at whatever rate you want (e.g., in Update()). 
    /// Since we're using a mix of both BNVIB-based playback and procedurally generated vibrations,
    /// we're just setting both in the function for simplicity.
    /// 
    /// To update BNVIB file playback only using Update(), you'd need to process multiple samples per Update call. This can 
    /// be approached in various ways, such as using an average of the samples used or just throwing out samples. 
    /// It's better to just do this at the recommended 200 Hz.
    /// </summary>
    void VibrationUpdate()
    {
        // Begin with getting the current sample (if any) for the file we might be playing back
        VibrationValue currValue = m_movementVibrationEffect.GetNextSample();

        /* Add in the runtime-generated rumble for the movement over the surface. 
         * If the amplitude sum goes over 1.0, it will be automatically scaled down.
         * For this, we're only using amplitudeHigh as a design choice. We could have also used amplitudeLow,
         * but in testing, using amplitudeHigh was more subtle and amplitudeLow tended to be so forceful 
         * that the impact effect wasn't enough of a contrast to be noticeable. 
         * We're leaving the frequencies as whatever they were set to by GetNextSample().
         * If the sample isn't currently playing, these will be the resonant frequencies by default.
        */
        currValue.amplitudeHigh += m_textureRumble;

        // Copy over for both left and right. Note that VibrationValue is a struct, so it's a deep copy.
        // The only potential difference for left and right vibrations in this demo is how they will be panned based on x-position, 
        // so everything besides that is the same for both.
        // You may need to change this if you're using different sources for left and right vibrations.
        VibrationValue leftVibration = currValue;
        VibrationValue rightVibration = currValue;

        // If using an NpadStyle that has two vibration devices, pan vibrations for both vibration devices based on position. 
        // If not, don't do any panning. It won't hurt anything to send vibration data to an inactive device, so we can just leave both at full strength.
        if (m_currentNpadStyle != NpadStyle.JoyLeft && m_currentNpadStyle != NpadStyle.JoyRight)
        {
            float leftPanMultiplier = 1.0f - m_rightPanMultiplier;
            leftVibration.amplitudeHigh *= leftPanMultiplier;
            leftVibration.amplitudeLow *= leftPanMultiplier;
            rightVibration.amplitudeHigh *= m_rightPanMultiplier;
            rightVibration.amplitudeLow *= m_rightPanMultiplier;
        }

        // Apply the effect to the current controller
        BasicVibratingController controller = m_vibratingControllers[m_currentNpadId];
        controller.SetLeftVibration(leftVibration);
        controller.SetRightVibration(rightVibration);
    }

    /// <summary>
    /// Checks for collisions and plays a collision vibration effect if there was one.
    /// </summary>
    /// <param name="characterController">CharacterController for the object to check</param>
    void ReactToCollisions(CharacterController characterController)
    {
        bool hasCollided = (characterController.collisionFlags & CollisionFlags.Sides) != 0;
        if (hasCollided && !m_hadWallCollisionLastFrame)
        {
            // Whenever the player runs into a wall, play the impact vibration effect
            m_movementVibrationEffect.SetPlayPositionToFileStart();
            m_movementVibrationEffect.Play();
        }

        m_hadWallCollisionLastFrame = hasCollided;
    }

#endregion

}
#endif 