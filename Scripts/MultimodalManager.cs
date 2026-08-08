using UnityEngine;
using UnityEngine.XR; //to access VR controllers and haptic features

//This class manages multimodal feedback in the scene
public class MultimodalManager : MonoBehaviour
{
// --- VISUAL FEEDBACK ---
    // These GameObjects represent the visual walkers used for feedback.
    // Red walker = negative feedback
    // Green walker = positive feedback
    [Header("Feedback Walkers")]
    public GameObject walkerRed;
    public GameObject walkerGreen;
 // --- REFERENCES TO OBJECTS IN THE SCENE ---
    // realAgent = the agent whose position is monitored
    // targetCube = the cube the agent should approach
    [Header("References")]
    public Transform realAgent; // we use trasform because we focus only on position, rotation of the ml models
    public Transform targetCube;
// --- DISTANCE DETECTION ---
    // Defines the threshold distance between agent and cube.
    // If the agent is closer than this value, positive feedback is triggered.
    [Header("Distance Detection")]
    public float closeDistance = 3.5f;
 // --- AUDIO FEEDBACK ---
    // Two audio sources allow playing different sounds
    // depending on the feedback state.
    [Header("Audio")]
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioClip cheerClip; //positive sound
    public AudioClip uncheerClip; //negative sound
// --- HAPTIC FEEDBACK ---
    // Defines which controller sends vibration and how strong it is.
    [Header("Haptics")]
    public XRNode controllerNode = XRNode.RightHand;
    public float vibrationAmplitude = 0.8f;// Amplitude = intensity of the vibration (0–1)
    public float vibrationDuration = 0.2f; // Duration = how long the vibration lasts
 // XR device reference used to send haptic impulses
    private InputDevice device;
// Tracks whether the agent is currently within the close distance
    private bool isClose = false;
    

    void Start()
    {// Ensure references to agent and cube exist
        FindReferences();
        // Retrieve the controller device from the XR system
        device = InputDevices.GetDeviceAtXRNode(controllerNode);
// Disable both walkers at the beginning of the scene
        if (walkerRed) walkerRed.SetActive(false);
        if (walkerGreen) walkerGreen.SetActive(false);
    }

   
    void Update()
    {// If references are missing, stop execution
    if (realAgent == null || targetCube == null) return;
// Calculate the current distance between the agent and the cube
    float currentDistance = Vector3.Distance(realAgent.position, targetCube.position);
        Debug.Log("Distance between agent and cube: " + currentDistance);
        // Check if the agent is within the threshold distance
    bool currentlyClose = currentDistance <= closeDistance;

    // Only trigger feedback when the state changes
        // (avoids repeatedly activating the same feedback every frame)
    if (currentlyClose != isClose)
        {// Update the state
            isClose = currentlyClose;
 // If the agent is close → positive feedback
            if (isClose)
                ActivateGreen();
                // Otherwise → negative feedback
            else
             ActivateRed();
        }
    }
// --- NEGATIVE FEEDBACK FUNCTION ---
    void ActivateRed()
    {// Activate red walker and disable green walker
        walkerRed.SetActive(true);
        walkerGreen.SetActive(false);

         // Play negative audio feedback if not already playing
        if (audioSource2 && uncheerClip && !audioSource2.isPlaying)
        {
            audioSource2.PlayOneShot(uncheerClip);
        }
    }
// --- POSITIVE FEEDBACK FUNCTION ---
    void ActivateGreen()
    {// Activate green walker and disable red walker
        walkerGreen.SetActive(true);
        walkerRed.SetActive(false);
         // Play positive audio feedback if not already playing
        if (audioSource1 && cheerClip && !audioSource1.isPlaying)
        {
            audioSource1.PlayOneShot(cheerClip);
        }
// Trigger haptic vibration on the controller
        SendHaptics(vibrationAmplitude, vibrationDuration);
        Debug.Log("Haptic feedback triggered");
    }
 // --- HAPTIC FEEDBACK FUNCTION ---
    void SendHaptics(float amplitude, float duration)
    {// If the device reference became invalid, retrieve it again
        if (!device.isValid)
            device = InputDevices.GetDeviceAtXRNode(controllerNode);
// Check if the controller supports haptic capabilities
        if (device.TryGetHapticCapabilities(out HapticCapabilities capabilities))
        {
            if (capabilities.supportsImpulse)
            { //// Send vibration impulse to channel 0
                // amplitude = vibration strength
                // duration = time the vibration lasts
                device.SendHapticImpulse(0u, amplitude, duration);
            }
        }
    }
// --- AUTOMATIC DETECTION REFERENCE SEARCH ---
    void FindReferences()
    {// If the agent reference is missing, find it using the tag "agent"
        if (realAgent == null)
        {
            var agentObj = GameObject.FindWithTag("agent");
            if (agentObj) realAgent = agentObj.transform;
        }
// If the cube reference is missing, find it using the tag "target"
        if (targetCube == null)
        {
            var cubeObj = GameObject.FindWithTag("target");
            if (cubeObj) targetCube = cubeObj.transform;
        }
    }
}
