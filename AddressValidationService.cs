//-----------------------------------------------------------------------
// <copyright file="AddressValidationService.cs" company="Procare Software, LLC">
//     Copyright © 2021-2023 Procare Software, LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Procare.AddressValidation.Tester
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class AddressValidationService : BaseHttpService
    {
        // Request timeout in milliseconds
        private const int TIMEOUT = 700;
        private const int RETRIES = 2;

        public AddressValidationService(IHttpClientFactory httpClientFactory, bool disposeFactory, Uri baseUrl)
            : this(httpClientFactory, disposeFactory, baseUrl, null, false)
        {
        }

        private AddressValidationService(IHttpClientFactory httpClientFactory, bool disposeFactory, Uri baseUrl, HttpMessageHandler? httpMessageHandler, bool disposeHandler)
            : base(httpClientFactory, disposeFactory, baseUrl, httpMessageHandler, disposeHandler)
        {
        }

        public event EventHandler<LogEventArgs>? LogEvent;

        public async Task<string> GetAddressesAsync(AddressValidationRequest request, CancellationToken token = default)
        {
            // Since the client factory keeps references to its clients we don't want to dispose it here.
            var client = this.GetClient();

            var retries = RETRIES;
            var queryString = request.ToQueryString();
            do
            {
                using var timeoutCts = new CancellationTokenSource();
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token);

                try
                {
                    using var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(this.BaseUrl, queryString));

                    // Doing the timeout here so we don't need to recreate the client for every retry.
                    timeoutCts.CancelAfter(TIMEOUT);

                    using var response = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, linkedCts.Token).ConfigureAwait(false);

                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync(linkedCts.Token).ConfigureAwait(false);
                }

                // Only attempt a retry on codes in the 5xx range. everything else should skip this catch and get thrown
                catch (HttpRequestException ex) when (retries > 0 && (int?)ex.StatusCode is >= 500 and <= 599)
                {
                    this.Log($"Status code:{ex.StatusCode} | attempts remaining: {retries} | retrying...");
                }

                // Only attempt a retry if the timeout token was cancelled. if the external token is cancelled, just throw.
                catch (OperationCanceledException) when (retries > 0 && timeoutCts.IsCancellationRequested && !token.IsCancellationRequested)
                {
                    this.Log($"Timed out after {TIMEOUT} milliseconds | attempts remaining: {retries} | retrying...");
                }
            }
            while (retries-- > 0);

            // Should be impossible to get here but the compiler doesn't know that.
            throw new InvalidOperationException("Unknown error");
        }

        private void Log(string message)
        {
            this.LogEvent?.Invoke(this, new LogEventArgs(message));
        }
    }
}
