using UnityEngine;
using UnityEngine.Rendering.PostProcessing;



public class PostProcessingController : MonoBehaviour

{

    [SerializeField] PostProcessVolume _ppVolume;
    [Range(0.0f,2f)]
    [SerializeField] float _gamma = 1.6f;
    [Range(0.0f,2f)]
    [SerializeField] float _bloom = 1f;
    [Range(-100,100)]
    [SerializeField] float _saturation = 0;
    [Range(-100,100)]
    [SerializeField] float _contrast = 0;
    [Range(-10,10)]
    [SerializeField] float _exposure = 1f;
    
    Bloom            _bloomLayer            = null;
    AmbientOcclusion _ambientOcclusionLayer = null;
    ColorGrading     _colorGradingLayer     = null;

    private void Start() {
        _ppVolume.profile.TryGetSettings(out _bloomLayer);
        _ppVolume.profile.TryGetSettings(out _ambientOcclusionLayer);
        _ppVolume.profile.TryGetSettings(out _colorGradingLayer);
        
        // ambientOcclusionLayer.enabled.value = true;
 
        _bloomLayer.enabled.value = true;
        _bloom = _bloomLayer.intensity.value;
        
        _colorGradingLayer.enabled.value = true;
        _saturation = _colorGradingLayer.saturation.value;
        _contrast = _colorGradingLayer.contrast.value;
        _gamma = _colorGradingLayer.gamma.value.z;
        _exposure = _colorGradingLayer.postExposure.value;

    }
    public void UpdateBloom(float value) {
        _bloom *= value;        
    }

    public void UpdateSaturation(float value) {
        _saturation *= value;
    }

    public void UpdateContrast(float value) {
        _contrast *= value;        
    }

    public void UpdateExposure(float value) {
        _exposure *= value;        
    }

    public void UpdateGamma(float value) {
        _gamma *= value;
    }
    
    private void Update() {
        
        _bloomLayer.intensity.value = _bloom;
        
        _colorGradingLayer.saturation.value = _saturation;
        _colorGradingLayer.contrast.value = _contrast;
        _colorGradingLayer.gamma.value.z = _gamma;
        _colorGradingLayer.postExposure.value = _exposure;

    }



}

