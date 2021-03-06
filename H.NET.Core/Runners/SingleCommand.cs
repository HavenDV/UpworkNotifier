﻿namespace H.NET.Core.Runners
{
    public class SingleCommand : TextObject
    {
        #region Constructors

        public SingleCommand()
        {
        }

        public SingleCommand(string text) : base(text)
        {
        }

        #endregion

        #region ICloneable

        public object Clone() => new SingleCommand(Text);

        #endregion
    }
}
