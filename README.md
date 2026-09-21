# BookieTrader V0.1

A server-side gambling trader prototype for SPT 4.1.x.

## Features

- Rouble double-or-nothing bets
- Let It Ride streaks
- Bitcoin gambling table
- Lucky 7 mystery tickets
- High Roller mystery tickets
- Cash-out chips
- Custom Bookie trader

## Requirements

- SPT 4.1.x
- .NET 10 SDK only if building from source

## Build

```powershell
dotnet restore
dotnet build BookieTrader.csproj -c Release
```

The project creates:

`ReleaseZip\Michael-BookieTrader-0.1.0.zip`

## Source

This repository contains the source code for BookieTrader.

## License

MIT. See `LICENSE`.
