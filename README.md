# Kryptoteket-crypto-bot
Discord bot for retrieving crypto prices and tickers at [Kryptotekets discord](https://discord.gg/heWSa5n).

## Getting started

1. Create your own discord bot by following this [guide](https://discordpy.readthedocs.io/en/latest/discord.html).
2. Copy your bot token and past it in [AppSettings](https://github.com/loekensgard/kryptoteket-crypto-bot/blob/master/Kryptoteket.Bot/appsettings.json) under token.
3. Run the project in your favorite .net IDE.

## Discord commands

### Crypto

```
!ticker <pair> <mx / nbx / bitmynt>
!price <pair> <mx / nbx>
!gainers <top> <1h / 24h / 7d / 14d / 30d / 200d / 1y>
!losers <top> <1h / 24h / 7d / 14d / 30d / 200d / 1y>
!graph <currency>
```

### Covid-19

```
!covid <countrycode / countryName>
```

### Reflink roulette

```
!reflink             | get random reflink from pool
!getref <@user>      | get reflink for user
!getrefs             | get all reflinks
!addref <reflink>    | add reflink to pool
!updateref <reflink> | update reflink
!deleteref           | delete reflink

!approve <@user>     | Approve reflink for user
!reject <@user>      | Reject reflink for user
```

### Bets

```
!getbet <name>
!addbet <name> <date>
!deletebet <name>
!bet <name> <price>
```

### Info/Stats

```
!serverinfo
!userinfo
```

### Misc

```
!help
!support
```

These can be found under the Modules folder.

## Data
All crypto data is retrieved by public APIs at [MiraiEx](https://developers.miraiex.com/), [CoinGecko](https://www.coingecko.com/), [NBX](https://nbx.com/) and [Bitmynt](https://bitmynt.no/)\
Coronavirus Statistics is retrieved by public APIs at [Disease.sh](https://disease.sh/docs/)\
Graphs are made by sparklines from [CoinGecko](https://www.coingecko.com/) and plottend into [QuickChart](https://quickchart.io/)

## Contributing
Pull requests are welcome, please discuss changes via Issues. 
