# Official LinQ Unity SDK

🛠 In order for us to provide optimal support, we would kindly ask you to submit any issues to [support@galactica.games](mailto:support@galactica.games) or [create an issue](./issues/new).

> When submitting an issue please specify your game identifier (asset), production steps, logs, code snippets and any additional relevant information.

## Overview

This SDK and list of helpers inside allows you to implement native payments in your Unity game, using our infrastructure for anti-froud checks and tokenization cards. We incapsulate different libraries and external API inside and profide convinient and simple API for processing user payments.

Also, we provide additional services for quick checks of user location, or in which country they have account in the App Store for better tunning user experience. Set of such function will be adjusted with a time.

## Requirements

Current version written and tested on Unity version **2021.3** and release **26f1**. Later versions should be supported too, but [create an issue](./link-to-new-issue) if any difficuty appears.

Unity SDK uses [External Dependency Manager for Unity (EDM4U)](https://github.com/googlesamples/unity-jar-resolver) from Google for managing native libraries required for work, including CocoaPod binaries for iOS. This manager should be installed via Package Manager before SDK installed, as Unity can not automatically resolve dependencies from external sources except its own.

For work with JSON, that required for web-request to external APIs need to install special [JSON.NET package](https://www.newtonsoft.com/json) from Newtonsoft.

## Installation

Before installing SDK, be sure that you have installed required packages, using `Window > Package Manager`.

EDM4U can be installed via command line using promt `openupm add com.google.external-dependency-manager` or need to manually add [Scoped Registry](https://docs.unity3d.com/Manual/upm-scoped.html) in `Player Setting > Package Manager` (see screenshot).

img1

Or you can manually edit config in `Packages/manifest.json` using the next config:

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

To start using SDK need configure it and initialize. SDK sends requests to our backend under the hood which requires authentification for requests. We provide special Mobile SDK Secret Key for each game, that has to set up in the SDK config.

### Setup

Configuration stored in the special asset, that has to be placed by the path `Assets/Resources/Settings/LinqSettings.asset`. For quick setup we provide Editor menu, so to start configuring SDK you have to select `LinQ > Edit Settings` and set `Remote URL` and `Secret Key`.

img2

> Remote URL for stage is `https://services.stage.galactica.games` when for production the same, except `staging` word, i.e.: `https://services.galactica.games`.

To initializate SDK we provide special Initializer, which has to placed on the game scene. To add it to the scene use menu `LinQ > Create Initializer`.

### Usage

To use SDK need to prepare card data and billing address, that user provided via your own UX elements. We do not require any additional steps here, except the rule by that card data should not be transfered to any game backend service as it will brake PCI DCI policy.

Also, before processing payment you have to initiate payment intent, that can be done via backend API with method `putReplenishOrder`. Method will return `order.id`, that is intent identifier itself.

To proceed with SDK collect the data as it on the example, please:

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

> Note, that country has to be converted into 2-letters code according to [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2).

The latest step is to pass collected data into SDK, which will return the replenishment order with appropriate status in case payment was successfull.

```csharp
var order = await LinqSDK.StartPaymentProcessing(data.id, details, address);
Debug.Log("Order status: " + order.Status);
```

### Testing & Validation

For testing purpuses you can use the next crad credentials:

- Number: `4242424242424242`
- Expriration: `12/27`
- Holder Name: `CARD HOLDER`
- CVV Code: `123`

In some cases native modules, that are used under the hood, may not work. It is applied for situation when game is running on Android or from Unity Editor. To do not block the flow it is possible to skip anti-fraud checks providing word `NOFRAUD` in the field of card holder name.

## Samples

Examples and ready to use components.

Not yet ready, but we will implement it soon!

Check our [Roadmap](./VISION.md) for more details.
  
## Tutorials

- How to implement your own payment screen. (Soon!)

## Contributing

Pull requests are welcomed! For major changes, please open an issue first
to discuss what you would like to change.

## License

[MIT License](./LICENSE.md)
