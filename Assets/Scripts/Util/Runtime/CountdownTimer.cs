using UnityEngine;
using UnityEngine.Events;

public class CountdownTimer : MonoBehaviour
{
    public float StartTime { get => m_startTime; }
    public float TimeRemaining { get => m_timeRemaining; }

    public UnityEvent OnTimerStarted;
    public UnityEvent OnTimerFinished;

    [SerializeField]
    private float m_startTime = 0f;
    private float m_timeRemaining = 0f;
    private bool m_timerRunning = false;

    #region Interface

    public void StartTimer ( float duration )
    {
        m_startTime = duration;

        StartTimer ();
    }

    public void StartTimer ()
    {
        m_timeRemaining = m_startTime;
        m_timerRunning = true;

        OnTimerStarted?.Invoke ();
    }

    public void Stop ()
    {
        m_timerRunning = false;
        ResetTimer ();
    }

    public void ResetTimer ()
    {
        m_timeRemaining = m_startTime;
    }

    #endregion

    #region Runtime

    private void Update ()
    {
        if ( m_timerRunning && m_timeRemaining > 0f )
        {
            m_timeRemaining -= Time.deltaTime;
        }
        else
        {
            m_timeRemaining = 0f;
            m_timerRunning = false;

            OnTimerFinished?.Invoke ();
        }
    }

    #endregion
}