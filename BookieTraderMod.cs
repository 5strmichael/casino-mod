using System.Reflection;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Helpers.Server;
using Michael.BookieTrader.Trader;
using Path = System.IO.Path;

namespace Michael.BookieTrader;

[Injectable(TypePriority = OnLoadOrder.TraderRegistration + 1)]
public class BookieTraderMod(
    ISptLogger<BookieTraderMod> logger,
    ModHelper modHelper,
    ImageRouter imageRouter,
    TraderConfig traderConfig,
    RagfairConfig ragfairConfig,
    TimeUtil timeUtil,
    FluentTraderAssortCreator assortCreator,
    AddCustomTraderHelper traderHelper) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        var traderImagePath = Path.Combine(pathToMod, "db/bookie.jpg");
        var traderBase = modHelper.GetJsonDataFromFile<TraderBase>(pathToMod, "db/base.json");

        imageRouter.AddRoute(traderBase.Avatar.Replace(".jpg", ""), traderImagePath);
        traderHelper.SetTraderUpdateTime(
            traderConfig,
            traderBase,
            timeUtil.GetHoursAsSeconds(1),
            timeUtil.GetHoursAsSeconds(2));

        ragfairConfig.Traders.TryAdd(traderBase.Id, true);
        traderHelper.AddTraderWithEmptyAssortToDb(traderBase);
        traderHelper.AddTraderToLocales(
            traderBase,
            "Bookie",
            "A back-room bookmaker who turns roubles, Bitcoin, and bad judgment into a business model. The odds are posted. The decision is yours.");

        AddRoubleBet(BookieIds.Bet50K, 50_000, 10);
        AddRoubleBet(BookieIds.Bet100K, 100_000, 10);
        AddRoubleBet(BookieIds.Bet250K, 250_000, 8);
        AddRoubleBet(BookieIds.Bet500K, 500_000, 5);
        AddRoubleBet(BookieIds.Bet1M, 1_000_000, 3);

        AddRoubleBet(BookieIds.Lucky7, 100_000, 10);
        AddRoubleBet(BookieIds.HighRoller, 500_000, 5);

        AddChipBarter(BookieIds.Ride200K, BookieIds.Chip200K, 10);
        AddChipBarter(BookieIds.Ride400K, BookieIds.Chip400K, 10);
        AddChipBarter(BookieIds.Ride800K, BookieIds.Chip800K, 10);

        AddBarterOffer(BookieIds.Btc1Bet, BookieIds.PhysicalBitcoin, 1, 3);
        AddBarterOffer(BookieIds.Btc2Payout, BookieIds.Btc2Chip, 1, 10);
        AddBarterOffer(BookieIds.Btc2Ride, BookieIds.Btc2Chip, 1, 10);
        AddBarterOffer(BookieIds.Btc4Payout, BookieIds.Btc4Chip, 1, 10);

        logger.Success("[BookieTrader] The Bookie is open for business.");
        return Task.CompletedTask;
    }

    private void AddRoubleBet(string ticketTpl, int price, int buyLimit)
    {
        assortCreator
            .CreateSingleAssortItem(new MongoId(ticketTpl))
            .AddStackCount(Math.Max(buyLimit * 10, 100))
            .AddBuyRestriction(buyLimit)
            .AddMoneyCost(Money.ROUBLES, price)
            .AddLoyaltyLevel(1)
            .Export(BookieIds.Trader);
    }

    private void AddChipBarter(string ticketTpl, string chipTpl, int stock)
    {
        AddBarterOffer(ticketTpl, chipTpl, 1, stock);
    }

    private void AddBarterOffer(string resultTpl, string costTpl, int costCount, int stock)
    {
        assortCreator
            .CreateSingleAssortItem(new MongoId(resultTpl))
            .AddStackCount(stock)
            .AddBarterCost(new MongoId(costTpl), costCount)
            .AddLoyaltyLevel(1)
            .Export(BookieIds.Trader);
    }
}
