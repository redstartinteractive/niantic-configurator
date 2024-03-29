using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// A component that can be used to access the most recently received basic light estimation information
/// for the physical environment as observed by an AR device.
/// </summary>
[RequireComponent(typeof(Light))]
public class BasicLightEstimation : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The ARCameraManager which will produce frame events containing light estimation information.")]
    private ARCameraManager m_CameraManager;

    /// <summary>
    /// Get or set the <c>ARCameraManager</c>.
    /// </summary>
    public ARCameraManager cameraManager
    {
        get => m_CameraManager;
        set
        {
            if(m_CameraManager == value)
            {
                return;
            }

            if(m_CameraManager != null)
            {
                m_CameraManager.frameReceived -= FrameChanged;
            }

            m_CameraManager = value;

            if((m_CameraManager != null) & enabled)
            {
                m_CameraManager.frameReceived += FrameChanged;
            }
        }
    }
    
    /// <summary>
    /// The estimated direction of the main light of the physical environment, if available.
    /// </summary>
    public Vector3? mainLightDirection { get; private set; }

    /// <summary>
    /// The estimated brightness of the physical environment, if available.
    /// </summary>
    public float? brightness { get; private set; }

    /// <summary>
    /// The estimated color temperature of the physical environment, if available.
    /// </summary>
    public float? colorTemperature { get; private set; }

    /// <summary>
    /// The estimated color correction value of the physical environment, if available.
    /// </summary>
    public Color? colorCorrection { get; private set; }
    
    /// <summary>
    /// The estimated color of the main light of the physical environment, if available.
    /// </summary>
    public Color? mainLightColor { get; private set; }

    private void Awake()
    {
        m_Light = GetComponent<Light>();
    }

    private void OnEnable()
    {
        if(m_CameraManager != null)
        {
            m_CameraManager.frameReceived += FrameChanged;
        }
    }

    private void OnDisable()
    {
        if(m_CameraManager != null)
        {
            m_CameraManager.frameReceived -= FrameChanged;
        }
    }

    private void FrameChanged(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.mainLightDirection.HasValue)
        {
            mainLightDirection = args.lightEstimation.mainLightDirection;
            m_Light.transform.rotation = Quaternion.LookRotation(mainLightDirection.Value);
        }

        if(args.lightEstimation.averageBrightness.HasValue)
        {
            brightness = args.lightEstimation.averageBrightness.Value;
            m_Light.intensity = brightness.Value;
        } else
        {
            brightness = null;
        }

        if(args.lightEstimation.averageColorTemperature.HasValue)
        {
            colorTemperature = args.lightEstimation.averageColorTemperature.Value;
            m_Light.colorTemperature = colorTemperature.Value;
        } else
        {
            colorTemperature = null;
        }

        if(args.lightEstimation.colorCorrection.HasValue)
        {
            colorCorrection = args.lightEstimation.colorCorrection.Value;
            m_Light.color = colorCorrection.Value;
        } else
        {
            colorCorrection = null;
        }
        
        if (args.lightEstimation.mainLightColor.HasValue)
        {
            mainLightColor = args.lightEstimation.mainLightColor;
            m_Light.color = mainLightColor.Value;
        }
        else
        {
            mainLightColor = null;
        }
    }

    private Light m_Light;
}
