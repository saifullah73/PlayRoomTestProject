using UnityEngine;

public class Wiper : MonoBehaviour
{
    public float minY = 0f;
    public float maxY = 5f;
    public float moveSpeed = 1f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;
    }

    void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * moveSpeed, 1f);
        float newY = Mathf.Lerp(minY, maxY, t);
        Vector3 newLocalPos = transform.localPosition;
        newLocalPos.y = newY;

        if (rb != null)
        {
            rb.MovePosition(transform.parent != null 
                ? transform.parent.TransformPoint(newLocalPos) 
                : newLocalPos);
        }
        else
        {
            transform.localPosition = newLocalPos;
        }
    }
}
