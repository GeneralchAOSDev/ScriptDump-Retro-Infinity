using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public float targetWidth = 768f;
    public float targetHeight = 1111f;
    public float pixelsPerUnit = 512f;
    
    private void Start()
    {
        AdjustCamera();
    }
    
    // Keep this for debugging, but remove for production
    private void Update()
    {
        AdjustCamera();
    }
    
    private void AdjustCamera()
    {
        float targetAspect = targetWidth / targetHeight;
        float screenAspect = (float)Screen.width / Screen.height;
        
        if (screenAspect >= targetAspect)
        {
            // Screen is wider than target: match height
            Camera.main.orthographicSize = targetHeight / (2f * pixelsPerUnit);
        }
        else
        {
            // Screen is taller than target: expand orthographic size to ensure width fits
            float difference = targetAspect / screenAspect;
            Camera.main.orthographicSize = targetHeight / (2f * pixelsPerUnit) * difference;
        }
    }
}