﻿using System;
using H.NET.Core.Recorders;

namespace H.NET.Core.Managers
{
    public class BaseManager : ParentRecorder
    {
        #region Properties

        public new IRecorder Recorder {
            get => base.Recorder;
            set {
                if (value == null && base.Recorder != null)
                {
                    base.Recorder.Stopped -= OnStoppedRecorder;
                }

                base.Recorder = value;

                if (value != null)
                {
                    base.Recorder.Stopped += OnStoppedRecorder;
                }
            }
        }

        public IConverter Converter { get; set; }

        public string Text { get; private set; }

        #endregion

        #region Events

        public delegate void TextDelegate(string key);
        public event TextDelegate NewText;
        private void OnNewText() => NewText?.Invoke(Text);
        
        protected override VoiceActionsEventArgs CreateArgs() => new VoiceActionsEventArgs
        {
            Recorder = Recorder,
            Converter = Converter,
            Data = Data,
            Text = Text
        };

        #endregion

        #region Constructors

        #endregion

        #region Public methods

        public void ProcessText(string text)
        {
            Text = text;
            OnNewText();
        }

        public async void ProcessSpeech(byte[] bytes)
        {
            var converter = Converter ?? throw new Exception("Converter is null");

            ProcessText(await converter.Convert(bytes));
        }

        public override void Start()
        {
            Text = null;
            base.Start();
        }

        public override void Stop()
        {
            var recorder = Recorder ?? throw new Exception("Recorder is null");

            recorder.Stop();
        }

        public void Change()
        {
            if (!IsStarted)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        #endregion

        #region Event handlers

        private void OnStoppedRecorder(object sender, VoiceActionsEventArgs args)
        {
            IsStarted = false;

            var recorder = Recorder ?? throw new Exception("Recorder is null");

            Data = recorder.Data;
            OnStopped(CreateArgs());

            ProcessSpeech(Data);
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            Recorder?.Dispose();
            Recorder = null;

            Converter?.Dispose();
            Converter = null;

            base.Dispose();
        }

        #endregion
    }
}
