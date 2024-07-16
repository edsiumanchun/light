using UnityEngine;

public class CircleLightController : MonoBehaviour
{
    public Color color = Color.white;
    public Vector3 centerPosition = new Vector3(0.5f, 0.5f, 0);
    public float radius = 0.2f;
    public float intensity = 2.0f;

    private Material material;

    private void Start()
    {
        // Get the material and set the properties
        material = GetComponent<Renderer>().material;
        SetIlluminationProperties();
        ActivateIllumination();
    }

    public void ActivateIllumination()
    {
        // Update the center position and radius
        centerPosition = transform.position;
        SetIlluminationProperties();
    }

    private void SetIlluminationProperties()
    {
        material.SetColor("_Color", color);
        material.SetVector("_CenterPosition", centerPosition);
        material.SetFloat("_Radius", radius);
        material.SetFloat("_Intensity", intensity);
    }
}