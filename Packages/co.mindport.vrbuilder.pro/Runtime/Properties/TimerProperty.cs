using System;
using UnityEngine;
using UnityEngine.Events;
using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Properties
{
    /// <summary>
    /// Property that acts as a timer and stores time on the required <see cref="NumberDataProperty"/>.
    /// </summary>
    [RequireComponent(typeof(NumberDataProperty))]
    public class TimerProperty : ProcessSceneObjectProperty, ITimerProperty
    {
        [Header("Events")]
        [SerializeField]
        private UnityEvent<TimerPropertyEventArgs> timerStart = new UnityEvent<TimerPropertyEventArgs>();

        [SerializeField]
        private UnityEvent<TimerPropertyEventArgs> timerStop = new UnityEvent<TimerPropertyEventArgs>();

        [SerializeField]
        private UnityEvent<TimerPropertyEventArgs> timerZero = new UnityEvent<TimerPropertyEventArgs>();

        private NumberDataProperty timeProperty;
        private float startTime;
        private float timerOriginalState;

        /// <inheritdoc/>
        public bool IsCountdown { get; set; }

        /// <inheritdoc/>
        public bool IsRunning { get { return isRunning; } }

        /// <inheritdoc/>
        public UnityEvent<TimerPropertyEventArgs> TimerStart => timerStart;

        /// <inheritdoc/>
        public UnityEvent<TimerPropertyEventArgs> TimerStop => timerStop;

        /// <inheritdoc/>
        public UnityEvent<TimerPropertyEventArgs> TimerZero => timerZero;

        private bool isRunning = false;

        /// <inheritdoc/>
        public event EventHandler<EventArgs> TimerStarted;

        /// <inheritdoc/>
        public event EventHandler<EventArgs> TimerStopped;

        /// <inheritdoc/>
        public event EventHandler<EventArgs> TimerAtZero;

        private void Awake()
        {
            timeProperty = GetComponent<NumberDataProperty>();
            timeProperty.OnValueReset.AddListener(OnValueReset);
        }

        private void OnValueReset()
        {
            if (isRunning)
            {
                startTime = Time.time;
                timerOriginalState = timeProperty.GetValue();
            }
        }

        private void Update()
        {
            if (isRunning == false)
            {
                return;
            }

            if (IsCountdown)
            {
                timeProperty.SetValue(Mathf.Max(timerOriginalState - (Time.time - startTime), 0));

                if (timeProperty.GetValue() <= 0)
                {
                    TimerAtZero?.Invoke(this, EventArgs.Empty);
                    TimerZero?.Invoke(new TimerPropertyEventArgs());
                    StopTimer();
                }
            }
            else
            {
                timeProperty.SetValue(timerOriginalState + (Time.time - startTime));
            }
        }

        /// <inheritdoc/>
        public void StartTimer()
        {
            isRunning = true;
            startTime = Time.time;
            timerOriginalState = timeProperty.GetValue();
            TimerStarted?.Invoke(this, EventArgs.Empty);
            TimerStart?.Invoke(new TimerPropertyEventArgs());
        }

        /// <inheritdoc/>
        public void StopTimer()
        {
            isRunning = false;
            TimerStopped?.Invoke(this, EventArgs.Empty);
            TimerStop?.Invoke(new TimerPropertyEventArgs());
        }
    }
}