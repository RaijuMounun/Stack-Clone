using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float time, upValue;
    [SerializeField] AnimationCurve curve;

    public void Up()
    {
        StopAllCoroutines();
        Vector3 target = transform.position;
        target.y += upValue;
        StartCoroutine(Move(transform, target, time, curve));
    }

    private IEnumerator Move(Transform current, Vector3 target, float time, AnimationCurve curve)
    {
        float passed = 0f;
        Vector3 init = current.position;
        while (passed < time)
        {
            passed += Time.deltaTime;
            float normalized = passed / time;
            normalized = curve.Evaluate(normalized);
            current.position = Vector3.Lerp(init, target, normalized);
            yield return null;
        }
    }
}


