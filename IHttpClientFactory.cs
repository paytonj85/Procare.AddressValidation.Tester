﻿//-----------------------------------------------------------------------
// <copyright file="IHttpClientFactory.cs" company="Procare Software, LLC">
//     Copyright © 2021-2023 Procare Software, LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Procare.AddressValidation.Tester
{
    using System;
    using System.Net.Http;

    public interface IHttpClientFactory : IDisposable
    {
        HttpClient GetClient();

        HttpClient GetClient(HttpMessageHandler? handler, bool disposeHandler);
    }
}
