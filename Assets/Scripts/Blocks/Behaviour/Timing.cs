using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timing : MonoBehaviour
{
    public List<float> timeIntervals = new List<float>();
    bool coroutineRunning = false;
    public bool canrun = true;
    public string actionName = "DefaultAction";
    Vector3 startposition;
    public Vector3 destination;
    public enum TimingState
    {
        TurningOn,
        CoolDown1,
        TurningOff,
        CoolDown2
    }
    void Start()
    {
        startposition = transform.position;
    }
    void Update()
    {
        if (!coroutineRunning && timeIntervals.Count > 0 && canrun)
        {
            StartCoroutine(ExecuteTimedActions());
        }
    }
    private IEnumerator ExecuteTimedActions()
    {
        coroutineRunning = true;
        TimingState currentState = TimingState.TurningOn;
        foreach (float interval in timeIntervals)
        {
            if (currentState == TimingState.TurningOn)
            {
                yield return StartCoroutine(MoveOverTime(transform.position, destination, interval));
            }
            else if (currentState == TimingState.TurningOff)
            {
                yield return StartCoroutine(MoveOverTime(transform.position, startposition, interval));
            }
            else
            {
                yield return new WaitForSeconds(interval);
            }

            switch (currentState)
            {
                case TimingState.TurningOn:
                    currentState = TimingState.CoolDown1;
                    break;
                case TimingState.CoolDown1:
                    currentState = TimingState.TurningOff;
                    break;
                case TimingState.TurningOff:
                    currentState = TimingState.CoolDown2;
                    break;
                case TimingState.CoolDown2:
                    currentState = TimingState.TurningOn;
                    break;
            }
            Debug.Log($"Action executed after {interval} seconds. State switched to {currentState}");
            actionName = currentState.ToString();
        }
        coroutineRunning = false;
    }

    private IEnumerator MoveOverTime(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = to;
    }
}
