# Trimble Mock Server

This is a mock server made in ASP.NET, made to simulate messages from Trimble Mobile Manager.

## Build & Run
With .NET installed, run:
```dotnet build && dotnet run```

This should start the server on `localhost:9635`

You can use something like [this website](https://piehost.com/websocket-tester) to test the websocket.

## Use with device

Change the following line in Properties/launchSettings.json:

`"applicationUrl": "http://localhost:9635"`

to the IP of your machine, i.e.

`"applicationUrl": "http://10.42.20.132:9635",`

On macOS, you can use the following command to find your IP: `ipconfig getifaddr en0`

Both machines need to be on the same network, unless you configure your network otherwise.
