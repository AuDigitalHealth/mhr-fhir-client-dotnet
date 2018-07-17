/*
* Copyright 2017 Australian Digital Health Agency (The Agency)
*
* Licensed under the AGENCY Open Source (Apache) License; you may not use this
* file except in compliance with the License. A copy of the License is in the
* 'license.txt' file, which should be provided with this work.
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
* WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
* License for the specific language governing permissions and limitations
* under the License.
*/

using System;
using System.Configuration;
using System.Net;
using System.Web;
using System.Windows;
using System.Windows.Navigation;
using DigitalHealth.MhrFhirClient;
using DigitalHealth.MhrFhirClient.Client;
using DigitalHealth.MhrFhirClient.Factory;
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Model.OAuth;
using DigitalHealth.MhrFhirClient.Rest;

namespace DigitalHealth.OAuthClient.Sample
{
    /// <summary>
    /// Demonstrates the usage of the Consumer OAuth Client to retrieve the Authorisation Code and Tokens 
    /// </summary>
    public partial class ConsumerAuthorisation
    {
        // The Consumer OAuth Client
        private IConsumerOAuthClient ConsumerOAuthClient { get; }

        // NOTE: PUT DUMMY DATA IN HERE ON RELEASE
        private readonly string _clientIdentifier = ConfigurationManager.AppSettings["ClientIdentifier"];
        private readonly string _clientSecret = ConfigurationManager.AppSettings["ClientSecret"];
        private readonly Uri _tokenEndPoint = new Uri(ConfigurationManager.AppSettings["TokenEndpointUrl"]);
        private readonly string _redirectUri = ConfigurationManager.AppSettings["RedirectUrl"];
        private readonly string _loginUri = ConfigurationManager.AppSettings["LoginUrl"];
        private readonly string _scopeUri = ConfigurationManager.AppSettings["ScopeUrl"];

        private string AuthorisationCode { get; set; }
        private string AccessToken { get; set; }
        private string RefreshToken { get; set; }

        public ConsumerAuthorisation()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

            InitializeComponent();
            Loaded += (sender, e) =>
            {
               LoadPage();
            };

            // Create Consumer OAuth Model
            var model = new ConsumerOAuthModel
            {
                ClientIdentifier = _clientIdentifier,
                ClientSecret = _clientSecret,
                TokenEndPointUrl = _tokenEndPoint,
                LoginUrl = _loginUri,
                RedirectUrl = _redirectUri,
                ScopeUrl = _scopeUri
            };

            // Create the Consumer Client
            ConsumerOAuthClient = ConsumerOAuthClientFactory.Create(model);
        }

        /// <summary>
        /// Catch the code redirect when the page navigates back
        /// </summary>
        private void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            // Get Code call back
            var code = HttpUtility.ParseQueryString(e.Uri.Query).Get("code");

            if (string.IsNullOrWhiteSpace(code)) return;
            AuthorisationCode = code;

            SetLabels();
            LoadPage();
        }

        // Load page
        private void LoadLoginPage(object sender, RoutedEventArgs e)
        {
            LoadPage();
        }

        private void LoadPage()
        {
            // Load page using Consumer OAuth Client GetLoginUri function
            var loginUrl = ConsumerOAuthClient.GetLoginUri().AbsoluteUri;
            WebBrowser.Navigate(loginUrl);
        }

        /// <summary>
        /// Demonstrates the retrieval of the token from the Consumer Client
        /// </summary>
        private async void GetToken(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await ConsumerOAuthClient.GetToken(AuthorisationCode);
                AccessToken = response.AccessToken;
                RefreshToken = response.RefreshToken;
                SetLabels();
            }
            catch (OAuthProviderClientException oAuthClientException)
            {
                MessageBox.Show(oAuthClientException.ErrorDescription);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            SetLabels();
        }

        /// <summary>
        /// Demonstrates the retrieval of the Refresh token from the Consumer Client
        /// </summary>
        private async void GetRefreshToken(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await ConsumerOAuthClient.GetRefreshToken(RefreshToken);
                AccessToken = response.AccessToken;
                RefreshToken = response.RefreshToken;

                SetLabels();
            }
            catch (OAuthProviderClientException oAuthClientException)
            {
                MessageBox.Show(oAuthClientException.ErrorDescription);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Set labels
        /// </summary>
        private void SetLabels()
        {
            AuthorisationCodeLabel.Content = AuthorisationCode;
            AccessTokenLabel.Text = AccessToken;
            RefreshTokenLabel.Content = RefreshToken;
        }
    }
}
