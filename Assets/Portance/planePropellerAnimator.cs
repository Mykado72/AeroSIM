using System;
using UnityEngine;

    public class planePropellerAnimator : MonoBehaviour
    {
        [SerializeField] private Transform m_PropellorModel;                          // The model of the the aeroplane's propellor.
        [SerializeField] private Transform m_PropellorBlur;                           // The plane used for the blurred propellor textures.
        [SerializeField] [Range(0f, 1f)] private float m_ThrottleBlurStart = 0.25f;   // The point at which the blurred textures start.
        [SerializeField] [Range(0f, 1f)] private float m_ThrottleBlurEnd = 0.5f;      // The point at which the blurred textures stop changing.
        [SerializeField] private float m_MaxRpm = 1;                               // The maximum speed the propellor can turn at.

        private PlaneControleur m_Plane;      // Reference to the aeroplane controller.
        private int m_PropellorBlurState = -1;    // To store the state of the blurred textures.
        private const float k_RpmToDps = 1200f;     // For converting from revs per minute to degrees per second.
        private Renderer m_PropellorModelRenderer;
        private Renderer m_PropellorBlurRenderer;

    private void Awake()
        {
            // Set up the reference to the aeroplane controller.
            m_Plane = GetComponent<PlaneControleur>();
            // m_PropellorModelRenderer = m_PropellorModel.GetComponent<Renderer>();
            // m_PropellorBlurRenderer = m_PropellorBlur.GetComponent<Renderer>();

    }


    private void Update()
    {
        // Rotate the propellor model at a rate proportional to the throttle.
        m_PropellorModel.Rotate(0, 0, -(m_Plane.Throttle* k_RpmToDps + 600) * Time.deltaTime );

        // Create an integer for the new state of the blur textures.
        var newBlurState = 0;

        // choose between the blurred textures, if the throttle is high enough
        if (m_Plane.Throttle > m_ThrottleBlurStart)
        {
            var throttleBlurProportion = Mathf.InverseLerp(m_ThrottleBlurStart, m_ThrottleBlurEnd, m_Plane.Throttle);
            m_PropellorBlur.gameObject.SetActive(true);
           
        }
        else
        {
            m_PropellorBlur.gameObject.SetActive(false);
        }
    }
}