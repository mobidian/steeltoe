// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Steeltoe.Common.Security
{
#pragma warning disable S3881 // "IDisposable" should be implemented correctly
    public class CertificateRotationService : IDisposable, ICertificateRotationService
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
    {
        private readonly IOptionsMonitor<CertificateOptions> _optionsMonitor;
        private readonly IDisposable _subscription;
        private CertificateOptions _lastValue;
        private bool _isStarted = false;

        public CertificateRotationService(IOptionsMonitor<CertificateOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
            _subscription = _optionsMonitor.OnChange(RotateCert);
        }

        public void Start()
        {
            if (_isStarted)
            {
                return;
            }

            RotateCert(_optionsMonitor.CurrentValue);
            _lastValue = _optionsMonitor.CurrentValue;
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        private void RotateCert(CertificateOptions newCert)
        {
            if (newCert.Certificate == null)
            {
                return;
            }

            var personalCertStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            var authorityCertStore = new X509Store(StoreName.CertificateAuthority, StoreLocation.CurrentUser);
            personalCertStore.Open(OpenFlags.ReadWrite);
            authorityCertStore.Open(OpenFlags.ReadWrite);
            if (_lastValue != null)
            {
                personalCertStore.Certificates.Remove(_lastValue.Certificate);
                foreach (var cert in _lastValue.IssuerChain)
                {
                    personalCertStore.Certificates.Remove(cert);
                }
            }

            personalCertStore.Certificates.Add(newCert.Certificate);
            foreach (var cert in newCert.IssuerChain)
            {
                personalCertStore.Certificates.Add(cert);
            }

            personalCertStore.Close();
            authorityCertStore.Close();
        }
    }
}