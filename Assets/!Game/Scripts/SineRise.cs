using UnityEngine;

public class SineRise : MonoBehaviour
{
    public float Amplitude = 1f;
    public float Frequency = 1f;
    public float Wavelength = 1f;

    Vector3 _startPos;
    float _phaseOffset;
    bool _initialized;
    
    void Start()
    {
        _startPos = transform.position;
        _phaseOffset = _startPos.magnitude / Mathf.Max(Wavelength, 0.0001f);
    }

    void OnValidate()
    {
        if (!_initialized)
        {
            _initialized = true;
            return; // don't run on start.
        }
        _phaseOffset = _startPos.magnitude / Mathf.Max(Wavelength, 0.0001f);
    }

    void Update()
    {
        var phase = Time.time * Frequency + _phaseOffset;
        var pos = _startPos;
        pos.y += Mathf.Sin(phase) * Amplitude;
        transform.position = pos;
    }
}