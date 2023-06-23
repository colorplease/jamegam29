using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public bool start = false;
    public AnimationCurve curve;
    public float duration;
    public float intensifier; //curve to strong need to reduce, will be a decimal
    public Vector3 startPosition; //passed in from cameraHandler;
    
    void Start()
    {
        RecordCurrentScreenPos();
    }
    
    void FixedUpdate()
    {
        if(start)
        {
            StartCoroutine(shaking());
            start = false;
        }
    }

   public void ShakeShake(float durationInput, AnimationCurve intensity, float intensifierInput)
    {
        //passes in information from the playerscript that is custom per gun
        curve = intensity;
        duration = durationInput;
        start = true;
        intensifier = intensifierInput;
    }

    public void Freeze()
    {
        //called during Freeze Player
        StopAllCoroutines();
    }

    IEnumerator shaking()
    {
        //shakes the screen using funny math idk im in calculus
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + (Random.insideUnitSphere * strength * intensifier);
            yield return null;
        }

        transform.position = startPosition;
    }

    public void RecordCurrentScreenPos()
    {
        startPosition = transform.localPosition;
    }
}
