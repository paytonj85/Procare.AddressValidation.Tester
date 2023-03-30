//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Procare Software, LLC">
//     Copyright © 2021-2023 Procare Software, LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Procare.AddressValidation.Tester
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    [SuppressMessage(
        "Design",
        "CA1031:Do not catch general exception types",
        Justification = "Need to guarantee there's no uncaught exceptions for the loop")]
    [SuppressMessage(
        "Globalization",
        "CA1303:Do not pass literals as localized parameters",
        Justification = "any string literals are just test console output. Not worth a resource table")]
    internal static class Program
    {
        private static async Task Main()
        {
            var addressValidationBaseUrl = new Uri("https://addresses.dev-procarepay.com");

            using var factory = new HttpClientFactory();
            using var addressService = new AddressValidationService(factory, false, addressValidationBaseUrl);
            addressService.LogEvent += (_, args) => Console.WriteLine(args.Message);

            // var request = new AddressValidationRequest { Line1 = "1 W Main", City = "Medford", StateCode = "OR", ZipCodeLeading5 = "97501" };
            // var request = new AddressValidationRequest();
            var request = new AddressValidationRequest { Line1 = "1125 17th St Ste 1800", City = "Denver", StateCode = "CO", ZipCodeLeading5 = "80202" };

            while (true)
            {
                try
                {
                    var response = await addressService.GetAddressesAsync(request).ConfigureAwait(false);
                    Console.WriteLine(response);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                const string x = " Q to quit, any other key to resend request.";
                Console.Write(x);
                if (Console.ReadKey().Key == ConsoleKey.Q)
                {
                    return;
                }

                Console.WriteLine();
            }
        }
    }
}
