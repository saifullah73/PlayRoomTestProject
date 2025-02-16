using UnityEngine;

public class FanRotator : MonoBehaviour
{
    public float rotationSpeed = 50f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            Quaternion deltaRotation = Quaternion.Euler(Vector3.up * rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
        else
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
