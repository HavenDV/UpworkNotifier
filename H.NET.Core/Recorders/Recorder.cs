﻿using System;

namespace H.NET.Core.Recorders
{
    public class Recorder : Module, IRecorder
    {
        #region Properties

        public bool IsStarted { get; protected set; }
        public byte[] Data { get; protected set; }

        #endregion

        #region Events

        public event EventHandler<VoiceActionsEventArgs> Started;
        protected void OnStarted(VoiceActionsEventArgs args) => Started?.Invoke(this, args);

        public event EventHandler<VoiceActionsEventArgs> Stopped;
        protected void OnStopped(VoiceActionsEventArgs args) => Stopped?.Invoke(this, args);

        protected virtual VoiceActionsEventArgs CreateArgs() => 
            new VoiceActionsEventArgs { Recorder = this, Data = Data };

        #endregion

        #region Public methods

        public virtual void Start()
        {
            IsStarted = true;
            Data = null;
            OnStarted(CreateArgs());
        }

        public virtual void Stop()
        {
            IsStarted = false;
            OnStopped(CreateArgs());
        }

        #endregion
    }
}
