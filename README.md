# Official LinQ Unity SDK

🛠 In order for us to provide optimal support, we would kindly ask you to submit any issues to [support@galactica.games](mailto:support@galactica.games) or [create an issue](https://github.com/linqgg/unity-sdk/issues/new).

> When submitting an issue please specify your game identifier (asset), production steps, logs, code snippets, and any additional relevant information.

## Overview

This SDK and list of helpers inside allow you to implement native payments in your Unity game, using our infrastructure for anti-fraud checks and tokenization cards. We encapsulate different libraries and external APIs inside and provide convenient and simple APIs for processing user payments.

Also, we provide additional services for quick checks of user location, or in which country they have an account in the App Store for better tunning user experience. The set of such functions will be adjusted with time.

## Requirements

The current version is written and tested on Unity version **2021.3** and release **26f1**. Later versions should be supported too, but [create an issue](./link-to-new-issue) if any difficulty appears.

Unity SDK uses [External Dependency Manager for Unity (EDM4U)](https://github.com/googlesamples/unity-jar-resolver) from Google for managing native libraries required for work, including CocoaPod binaries for iOS. This manager should be installed via Package Manager before SDK is installed, as Unity can not automatically resolve dependencies from external sources except its own.

For work with JSON, which is required for web-request to external APIs need to install a special [JSON.NET package](https://www.newtonsoft.com/json) from Newtonsoft.

## Installation

Before installing SDK, be sure that you have installed the required packages, using `Window > Package Manager`.

EDM4U can be installed via the command line using the prompt `openupm add com.google.external-dependency-manager` or need to manually add [Scoped Registry](https://docs.unity3d.com/Manual/upm-scoped.html) in `Player Setting > Package Manager` (see screenshot).

![linq-unity-sdk-setup-1](https://github.com/linqgg/unity-sdk/assets/303498/81ed49b5-95cf-4b1c-83cc-ee0615ff24d7)

Or you can manually edit the config in `Packages/manifest.json` using the next config:

```json
"scopedRegistries": [
  {
    "name": "package.openupm.com",
    "url": "https://package.openupm.com",
    "scopes": [
      "com.google.external-dependency-manager"
    ]
  }
]
```

## Getting Started

To start using SDK need to configure it and initialize it. SDK sends requests to our backend under the hood which requires authentification for requests. We provide a special Mobile SDK Secret Key for each game, that has to be set up in the SDK config.

### Setup

Configuration stored in the special asset, that has to be placed by the path `Assets/Resources/Settings/LinqSettings.asset`. For quick setup we provide an Editor menu, so to start configuring SDK you have to select `LinQ > Edit Settings` and set `Remote URL` and `Secret Key`.

![linq-unity-sdk-setup-2](https://github.com/linqgg/unity-sdk/assets/303498/77d1fbf5-6cb3-4f90-85b8-c2158d87bc16)

> Remote URL for the stage is `https://services.stage.galactica.games` when for production the same, except `staging` word, i.e.: `https://services.galactica.games`.

To initialize SDK we provide a special Initializer, which has to be placed on the game scene. To add it to the scene use menu `LinQ > Create Initializer`.

In case you are using your system for configuring the game, you may initialize the SDK manually, providing proper details.

```scharp
LinqSDK.InitSDK("https://services.stage.galactica.games", "secret-key");
```

### Usage

To use SDK need to prepare card data and billing address, that the user provided via your own UX elements. We do not require any additional steps here, except the rule that card data should not be transferred to any game backend service as it will break PCI DCI policy.

Also, before processing payment you have to initiate payment intent, which can be done via backend API with the method `putReplenishOrder`. The method will return `order.id`, that is intent identifier itself.

To proceed with SDK collect the data as in the example, please:

```csharp
// Card Details
var details = new PaymentDetails()
{
  CardNumber = "4242424242424242",
  Expiration = "12/27",
  HolderName = "Kevon Chang",
  Protection = "123",
};

// Billing Address
var address = new BillingAddress()
{
  Country = "US", // 2-letter code
  Region = "Iowa",
  City = "Iowa City",
  Street = "109 S Johnson St",
  Zip = "52240"
};
```

> Note, that country has to be converted into a 2-letter code according to [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2).

The latest step is to pass collected data into SDK, which will return the replenishment order with the appropriate status in case payment was successful.

```csharp
var order = await LinqSDK.StartPaymentProcessing(data.id, details, address);
Debug.Log("Order status: " + order.Status);
```

### Testing & Validation

For testing purposes, you can use the following card credentials:

- Number: `4242424242424242`
- Expriration: `12/27`
- Holder Name: `CARD HOLDER`
- CVV Code: `123`

In some cases, native modules, that are used under the hood, may not work. It is applied for situations when the game is running on Android or from Unity Editor. To not block the flow it is possible to skip anti-fraud checks by providing the word `NOFRAUD` in the field of the cardholder name.

## Samples

Examples and ready-to-use components.

Not yet ready, but we will implement it soon!

Check our [Roadmap](./VISION.md) for more details.
  
## Tutorials

- How to implement your payment screen. (Soon!)

## Contributing

Pull requests are welcomed! For major changes, please open an issue first
to discuss what you would like to change.

## License

[MIT License](./LICENSE.md)
