# MHR FHIR Client for .NET

MHR FHIR Client is a .NET library for accessing the My Health Record FHIR gateway. It contains consumer and provider APIs for authenticating using OAuth and accessing 
health information through the FHIR gateway. The library can be used within desktop and mobile applications.

## Getting started

Put your clientId and clientSecret codes in the App.config files.

For Provider Models
\DigitalHealth.MhrFhirClient.Sample\App.config

For Consumer Models
\DigitalHealth.MhrFhirClient.Sample.Consumer\App.config


### Provider

For provider access the simplest way to get started is to use the `ProviderOAuthClientFactory` to create a `IProviderOAuthClient` and get an access token:

```csharp

ProviderOAuthModel providerOAuthDetails new ProviderOAuthModel
{
    Certificate = _certificate,
    TokenProviderEndpointUrl = _tokenEndPoint,
    ClientIdentifier = _providerClientId,
    ClientSecret = _providerClientSecret,
    RedirectUrl = _callback,
    Hpio = _hpio,
    OrganisationName = _organisationName,
    DeviceModel = _deviceModel,
    DeviceIdentifier = _deviceIdentifier,
    DeviceMake = _deviceMake
};

IProviderOAuthClient providerOAuthClient = ProviderOAuthClientFactory.Create(providerOAuthDetails);

OAuthResponse oAuthResponse = await providerOAuthClient.GetProviderToken(userIdentifier, userName);

string accessToken = oAuthResponse.AccessToken;

```

Once the token has been retrieved the `MhrFhirProviderClientFactory` can be used to create a `IMhrFhirProviderClient` which is used to make calls to the MHR FHIR gateway:

```csharp

IMhrFhirProviderClient mhrFhirPoviderClient = MhrFhirProviderClientFactory.Create(_providerEndPoint, accessToken, _clientId, _appVersion, providerOAuthDetails.Certificate);

mhrFhirProviderClient.GetPrescriptions(_patientId, _startDate);

```

### Consumer

For consumer access a consumer must first log into [my.gov.au](https://my.gov.au) and grant access to the application. This is done by redirecting to the [my.gov.au](https://my.gov.au) login. Once the consumer has logged and granted access the token can be retrieved by using the `ConsumerOAuthClientFactory` to create a `IConsumerOAuthClient` and passing the authorisation code.

```csharp

ConsumerOAuthModel consumerOAuthDetails = new ConsumerOAuthModel
{
    ClientIdentifier = _clientIdentifier,
    ClientSecret = _clientSecret,
    TokenEndPointUrl = _tokenEndPoint,
    LoginUrl = _loginUri,
    RedirectUrl = _redirectUri,
    ScopeUrl = _scopeUri
};

IConsumerOAuthClient consumerOAuthClient = ConsumerOAuthClientFactory.Create(consumerOAuthDetails);

OAuthResponse oAuthResponse = await consumerOAuthClient.GetToken(_authorisationCode);

string accessToken = oAuthResponse.AccessToken;

```

Once the token has been retrieved the `MhrFhirConsumerClientFactory` can be used to create a `IMhrFhirConsumerClient` which is used to make calls to the MHR FHIR gateway:

```csharp

IMhrFhirProviderClient mhrFhirPoviderClient = MhrFhirConsumerClientFactory.Create(_providerEndPoint, accessToken, _clientId, _appVersion);

mhrFhirProviderClient.GetPrescriptions(_patientId, _startDate);

```