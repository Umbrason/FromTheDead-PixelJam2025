using UnityEngine;

[ExecuteAlways]
public class CRTTime : MonoBehaviour
{
    [SerializeField] Material crtMat;
    void Update()
    {
        crtMat.SetFloat("_CRTTime", Time.unscaledTime);
    }
}
