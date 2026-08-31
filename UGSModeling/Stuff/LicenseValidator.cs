using Standard.Licensing;
using Standard.Licensing.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Sl = Standard.Licensing;
using System.Security.Cryptography;

namespace UGSModeling.Stuff
{


    public class LicenseValidator
    {
        private const string PublicKey = "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEu0Gul7geaenmEqeXoF4k4CHnNIRkQYkRiFO+DxnfYsLoo2noDu57IrquJoO2AXHXUSV4e+KT66obQAQiOfpr7w==";

        private readonly string _licenseFilePath;
        private const string ActivationFlagKey = "app_activated";
        private const string ActivationHashKey = "activation_hash";
        public LicenseValidator()
        {
            _licenseFilePath = Path.Combine(FileSystem.AppDataDirectory,"license.lic");
        }

        public async Task<(bool IsValid, List<string> Errors)> ActivateAndSaveLicenseAsync(string licenseContent)
        {
            try
            {
                var tempLicense = Sl.License.Load(licenseContent);
                var validationResult = ValidateLicense(tempLicense);

                if (!validationResult.IsValid)
                    return validationResult;

                await File.WriteAllTextAsync(_licenseFilePath, licenseContent, Encoding.UTF8);

                var licenseHash = ComputeSha256Hash(licenseContent);
                await SecureStorage.SetAsync(ActivationHashKey, licenseHash);

                await SecureStorage.SetAsync(ActivationFlagKey, "true");

                return (true, new List<string>());
            }
            catch (Exception ex)
            {
                return (false, new List<string> { $"Ошибка активации: {ex.Message}" });
            }
        }



        //Собственно валидация
        public (bool IsValid, List<string> Errors) ValidateLicense(Sl.License license)
        {
            var validationFailures = license.Validate()
                                .ExpirationDate(systemDateTime: DateTime.Now)
                                .When(lic => lic.Type == LicenseType.Standard)
                                .And()
                                .Signature(PublicKey)
                                .AssertValidLicense()
                                .ToList();

            if (validationFailures.Any())
            {
                var errors = validationFailures.Select(f => f.Message).ToList();
                return (false, errors);
            }

            return (true, new List<string>());
        }

        public (bool IsValid, List<string> Errors) ValidateLicenseFromFile()
        {
            if (!File.Exists(_licenseFilePath))
                return (false, new List<string> { "Файл лицензии не найден" });

            var licenseContent = File.ReadAllText(_licenseFilePath);
            var license = Sl.License.Load(licenseContent);
            return ValidateLicense(license);
        }


        private string ComputeSha256Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        public async Task FullResetActivationAsync()
        {

            SecureStorage.Remove("ActivationHashKey");
            SecureStorage.Remove("ActivationFlagKey");

        }
    }
}
