using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 Rotation = new Vector3(0, 100, 0);
    
    void Update()
    {
        transform.Rotate(Rotation * Time.deltaTime);
    }
}
