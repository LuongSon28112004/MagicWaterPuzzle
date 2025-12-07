using UnityEngine;
using UnityEngine.UI;

public class NonBlockingRaycast : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        // Trả về false để *không chặn raycast* xuống UI bên dưới
        return true;
    }
}
