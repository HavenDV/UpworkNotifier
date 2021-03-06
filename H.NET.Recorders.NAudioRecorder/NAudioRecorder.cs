﻿using H.NET.Core.Recorders;
using NAudio.Wave;

namespace H.NET.Recorders
{
    public class NAudioRecorder : Recorder
    {
        private WaveInEvent Capture { get; set; } = new WaveInEvent();

        #region Constructors

        public NAudioRecorder()
        {
            Capture.DataAvailable += (sender, args) =>
            {
                //var data = new List<byte>();
                //data.AddRange(args.Buffer.Take(args.BytesRecorded));

                //Data = data.ToArray();
                //var waveHeader = new byte[] { 0x1, 0x2, 0xFF, 0xFE };
                //data.AddRange(waveHeader);
                //data.AddRange(args.Buffer);

                Data = args.Buffer;
            };
        }

        #endregion

        #region Public methods

        public override void Start()
        {
            Capture.StartRecording();

            base.Start();
        }

        public override void Stop()
        {
            Capture.StopRecording();
            
            base.Stop();
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            Capture?.Dispose();
            Capture = null;
        }

        #endregion
    }
}
