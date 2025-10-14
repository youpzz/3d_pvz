using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 90f;
    public Space rotationSpace = Space.Self;

    void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, rotationSpace);
    }
}
