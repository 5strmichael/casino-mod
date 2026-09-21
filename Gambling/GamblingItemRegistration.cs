using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Services.Modding.Custom;

namespace Michael.BookieTrader.Gambling;

/// <summary>
/// Registers the chips and openable gambling tickets before traders are registered.
/// A ticket is a cloned sealed crate whose random-loot table contains the wager outcomes.
/// </summary>
[Injectable(TypePriority = OnLoadOrder.Preload + 1)]
public class GamblingItemRegistration(
    ISptLogger<GamblingItemRegistration> logger,
    TemplateTable templateTable,
    InventoryConfig inventoryConfig,
    CustomItemService customItemService) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        CreateToken(BookieIds.Bust, "bookie_bust_slip", "BUST — House Wins", "BUST",
            "No payout. The Bookie keeps the wager. Better luck next time.", 1, false);

        CreateCashChip(BookieIds.Chip100K, "100k Winning Chip", "100K CHIP", 100_000);
        CreateCashChip(BookieIds.Chip200K, "200k Winning Chip", "200K CHIP", 200_000);
        CreateCashChip(BookieIds.Chip400K, "400k Winning Chip", "400K CHIP", 400_000);
        CreateCashChip(BookieIds.Chip500K, "500k Winning Chip", "500K CHIP", 500_000);
        CreateCashChip(BookieIds.Chip800K, "800k Winning Chip", "800K CHIP", 800_000);
        CreateCashChip(BookieIds.Chip1M, "1M Winning Chip", "1M CHIP", 1_000_000);
        CreateCashChip(BookieIds.Chip1Point6M, "1.6M Winning Chip", "1.6M CHIP", 1_600_000);
        CreateCashChip(BookieIds.Chip2M, "2M Winning Chip", "2M CHIP", 2_000_000);
        CreateCashChip(BookieIds.Chip5M, "5M Winning Chip", "5M CHIP", 5_000_000);

        CreateToken(BookieIds.Btc2Chip, "bookie_2btc_chip", "2 BTC Winning Chip", "2 BTC CHIP",
            "A winning Bitcoin-table voucher. Cash it out for two physical Bitcoins or let it ride for four.", 1, false);
        CreateToken(BookieIds.Btc4Chip, "bookie_4btc_chip", "4 BTC Winning Chip", "4 BTC CHIP",
            "The end of the current Bitcoin streak. Redeem it with the Bookie for four physical Bitcoins.", 1, false);

        CreateFiftyFiftyTicket(BookieIds.Bet50K, "bookie_bet_50k", "Double or Nothing — 50k", "50K FLIP", BookieIds.Chip100K, 50_000);
        CreateFiftyFiftyTicket(BookieIds.Bet100K, "bookie_bet_100k", "Double or Nothing — 100k", "100K FLIP", BookieIds.Chip200K, 100_000);
        CreateFiftyFiftyTicket(BookieIds.Bet250K, "bookie_bet_250k", "Double or Nothing — 250k", "250K FLIP", BookieIds.Chip500K, 250_000);
        CreateFiftyFiftyTicket(BookieIds.Bet500K, "bookie_bet_500k", "Double or Nothing — 500k", "500K FLIP", BookieIds.Chip1M, 500_000);
        CreateFiftyFiftyTicket(BookieIds.Bet1M, "bookie_bet_1m", "Double or Nothing — 1M", "1M FLIP", BookieIds.Chip2M, 1_000_000);

        CreateFiftyFiftyTicket(BookieIds.Ride200K, "bookie_ride_200k", "Let It Ride — 200k", "RIDE 200K", BookieIds.Chip400K, 200_000);
        CreateFiftyFiftyTicket(BookieIds.Ride400K, "bookie_ride_400k", "Let It Ride — 400k", "RIDE 400K", BookieIds.Chip800K, 400_000);
        CreateFiftyFiftyTicket(BookieIds.Ride800K, "bookie_ride_800k", "Let It Ride — 800k", "RIDE 800K", BookieIds.Chip1Point6M, 800_000);

        CreateFiftyFiftyTicket(BookieIds.Btc1Bet, "bookie_btc_bet_1", "Bitcoin Double or Nothing — 1 BTC", "1 BTC FLIP", BookieIds.Btc2Chip, 1);
        CreateFiftyFiftyTicket(BookieIds.Btc2Ride, "bookie_btc_ride_2", "Bitcoin Let It Ride — 2 BTC", "RIDE 2 BTC", BookieIds.Btc4Chip, 1);

        CreateTicket(BookieIds.Btc2Payout, "bookie_btc_payout_2", "Cash Out — 2 BTC", "2 BTC PAYOUT",
            "Redeem this payout pack to receive two physical Bitcoins.", 1, 2,
            new Dictionary<MongoId, double> { [new MongoId(BookieIds.PhysicalBitcoin)] = 1 });

        CreateTicket(BookieIds.Btc4Payout, "bookie_btc_payout_4", "Cash Out — 4 BTC", "4 BTC PAYOUT",
            "Redeem this payout pack to receive four physical Bitcoins.", 1, 4,
            new Dictionary<MongoId, double> { [new MongoId(BookieIds.PhysicalBitcoin)] = 1 });

        CreateTicket(BookieIds.Lucky7, "bookie_lucky_7", "Lucky 7", "LUCKY 7",
            "A 100k mystery ticket. Most lose, some break even, and a tiny few hit hard.", 100_000, 1,
            new Dictionary<MongoId, double>
            {
                [new MongoId(BookieIds.Bust)] = 60,
                [new MongoId(BookieIds.Chip100K)] = 25,
                [new MongoId(BookieIds.Chip200K)] = 10,
                [new MongoId(BookieIds.Chip500K)] = 4,
                [new MongoId(BookieIds.Chip2M)] = 1
            });

        CreateTicket(BookieIds.HighRoller, "bookie_high_roller", "High Roller", "HIGH ROLLER",
            "A 500k high-stakes mystery ticket with a five-million-rouble top hit.", 500_000, 1,
            new Dictionary<MongoId, double>
            {
                [new MongoId(BookieIds.Bust)] = 55,
                [new MongoId(BookieIds.Chip500K)] = 25,
                [new MongoId(BookieIds.Chip1M)] = 12,
                [new MongoId(BookieIds.Chip2M)] = 6,
                [new MongoId(BookieIds.Chip5M)] = 2
            });

        logger.Success("[BookieTrader] Registered gambling tickets and payout chips.");
        return Task.CompletedTask;
    }

    private void CreateCashChip(string id, string displayName, string shortName, int payout)
    {
        CreateToken(id, $"bookie_cash_chip_{payout}", displayName, shortName,
            $"Cash-out value: ₽{payout:N0}. Sell this chip back to the Bookie, or use it in a listed let-it-ride wager when available.",
            payout, true);
    }

    private void CreateToken(string id, string internalName, string displayName, string shortName,
        string description, int handbookValue, bool sellableForCash)
    {
        var cloneDetails = new NewItemFromCloneDetails
        {
            NewItemName = internalName,
            ItemTplToClone = new MongoId(BookieIds.GpCoin),
            ParentId = new MongoId(BookieIds.BarterItemParent),
            NewId = new MongoId(id),
            FleaPriceRoubles = handbookValue,
            HandbookPriceRoubles = handbookValue,
            HandbookParentId = BookieIds.BarterHandbookParent,
            Locales = EnglishLocale(displayName, shortName, description),
            OverrideProperties = new TemplateItemProperties
            {
                Name = displayName,
                ShortName = shortName,
                Description = description,
                Weight = 0.01
            }
        };

        var result = customItemService.CreateItemFromClone(cloneDetails);
        if (!result.Success)
        {
            logger.Error($"[BookieTrader] Failed to create token '{displayName}': {string.Join("; ", result.Errors)}");
        }
    }

    private void CreateFiftyFiftyTicket(string id, string internalName, string displayName,
        string shortName, string winningTpl, int displayValue)
    {
        CreateTicket(id, internalName, displayName, shortName,
            "A true 50/50 wager. Open it: one result pays the printed winning chip; the other is a bust.",
            displayValue, 1,
            new Dictionary<MongoId, double>
            {
                [new MongoId(BookieIds.Bust)] = 1,
                [new MongoId(winningTpl)] = 1
            });
    }

    private void CreateTicket(string id, string internalName, string displayName, string shortName,
        string description, int handbookValue, int rewardCount, Dictionary<MongoId, double> rewardPool)
    {
        var cloneDetails = new NewItemFromCloneDetails
        {
            NewItemName = internalName,
            ItemTplToClone = new MongoId(BookieIds.SealedWeaponCrate),
            ParentId = new MongoId(BookieIds.RandomLootContainerParent),
            NewId = new MongoId(id),
            FleaPriceRoubles = handbookValue,
            HandbookPriceRoubles = handbookValue,
            HandbookParentId = BookieIds.RandomLootContainerParent,
            Locales = EnglishLocale(displayName, shortName, description),
            OverrideProperties = new TemplateItemProperties
            {
                Name = displayName,
                ShortName = shortName,
                Description = description,
                Weight = 0.01
            }
        };

        var result = customItemService.CreateItemFromClone(cloneDetails);
        if (!result.Success)
        {
            logger.Error($"[BookieTrader] Failed to create ticket '{displayName}': {string.Join("; ", result.Errors)}");
            return;
        }

        var customItemInDb = templateTable.Items.GetValueOrDefault(new MongoId(id));
        if (customItemInDb is null)
        {
            logger.Error($"[BookieTrader] Ticket '{displayName}' was created but could not be found in the template table.");
            return;
        }

        customItemInDb.Name = internalName;
        inventoryConfig.RandomLootContainers[new MongoId(id)] = new RewardDetails
        {
            RewardCount = rewardCount,
            FoundInRaid = false,
            RewardTplPool = rewardPool
        };
    }

    private static Dictionary<string, LocaleDetails> EnglishLocale(string name, string shortName, string description)
    {
        return new Dictionary<string, LocaleDetails>
        {
            ["en"] = new LocaleDetails
            {
                Name = name,
                ShortName = shortName,
                Description = description
            }
        };
    }
}
