//-----------------------------------------------------------------------
// <copyright file="LogEventArgs.cs" company="Procare Software, LLC">
//     Copyright © 2021-2023 Procare Software, LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Procare.AddressValidation.Tester
{
    using System;

    public class LogEventArgs : EventArgs
    {
        public LogEventArgs(string message)
        {
            this.Message = message;
        }

        public string Message { get; }
    }
}
