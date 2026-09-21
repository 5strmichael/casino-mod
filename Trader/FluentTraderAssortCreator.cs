using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace Michael.BookieTrader.Trader;

/// <summary>
/// Small fluent helper based on the official SPT dynamic trader example.
/// V0.1 only needs single-template offers.
/// </summary>
[Injectable(TypePriority = OnLoadOrder.TraderRegistration + 1)]
public class FluentTraderAssortCreator(
    TradersTable tradersTable,
    ISptLogger<FluentTraderAssortCreator> logger)
{
    private readonly List<Item> _itemsToSell = [];
    private readonly Dictionary<string, List<List<BarterScheme>>> _barterScheme = [];
    private readonly Dictionary<string, int> _loyaltyLevel = [];

    public FluentTraderAssortCreator CreateSingleAssortItem(MongoId itemTpl, MongoId? itemId = null)
    {
        var newItemToAdd = new Item
        {
            Id = itemId ?? new MongoId(),
            Template = itemTpl,
            ParentId = "hideout",
            SlotId = "hideout",
            Upd = new Upd
            {
                UnlimitedCount = false,
                StackObjectsCount = 100
            }
        };

        _itemsToSell.Add(newItemToAdd);
        return this;
    }

    public FluentTraderAssortCreator AddStackCount(int stackCount)
    {
        _itemsToSell[0].Upd!.StackObjectsCount = stackCount;
        return this;
    }

    public FluentTraderAssortCreator AddUnlimitedStackCount()
    {
        _itemsToSell[0].Upd!.StackObjectsCount = 999999;
        _itemsToSell[0].Upd.UnlimitedCount = true;
        return this;
    }

    public FluentTraderAssortCreator AddBuyRestriction(int maxBuyLimit)
    {
        _itemsToSell[0].Upd!.BuyRestrictionMax = maxBuyLimit;
        _itemsToSell[0].Upd.BuyRestrictionCurrent = 0;
        return this;
    }

    public FluentTraderAssortCreator AddLoyaltyLevel(int level)
    {
        _loyaltyLevel[_itemsToSell[0].Id] = level;
        return this;
    }

    public FluentTraderAssortCreator AddMoneyCost(string currencyType, int amount)
    {
        var dataToAdd = new BarterScheme
        {
            Count = amount,
            Template = currencyType
        };

        if (!_barterScheme.TryAdd(_itemsToSell[0].Id, [[dataToAdd]]))
        {
            logger.Warning($"[BookieTrader] Unable to add currency barter scheme: {currencyType}");
        }

        return this;
    }

    public FluentTraderAssortCreator AddBarterCost(MongoId itemTpl, int count)
    {
        var sellableItemId = _itemsToSell[0].Id;

        if (_barterScheme.Count == 0)
        {
            _barterScheme[sellableItemId] = [[new BarterScheme
            {
                Count = count,
                Template = itemTpl
            }]];
        }
        else
        {
            var existingData = _barterScheme[sellableItemId][0].FirstOrDefault(x => x.Template == itemTpl);
            if (existingData is not null)
            {
                existingData.Count += count;
            }
            else
            {
                _barterScheme[sellableItemId][0].Add(new BarterScheme
                {
                    Count = count,
                    Template = itemTpl
                });
            }
        }

        return this;
    }

    public FluentTraderAssortCreator? Export(string traderId)
    {
        var traderData = tradersTable.GetValueOrDefault(traderId);
        if (traderData is null || _itemsToSell.Count == 0)
        {
            logger.Error($"[BookieTrader] Could not export assort for trader {traderId}.");
            Reset();
            return null;
        }

        var rootItemAddedId = _itemsToSell[0].Id;
        if (traderData.Assort.Items.Exists(x => x.Id == rootItemAddedId))
        {
            logger.Error($"[BookieTrader] Assort item key {rootItemAddedId} is already in use.");
            Reset();
            return null;
        }

        traderData.Assort.Items.AddRange(_itemsToSell);
        traderData.Assort.BarterScheme[rootItemAddedId] = _barterScheme[rootItemAddedId];
        traderData.Assort.LoyalLevelItems[rootItemAddedId] = _loyaltyLevel[rootItemAddedId];
        Reset();
        return this;
    }

    private void Reset()
    {
        _itemsToSell.Clear();
        _barterScheme.Clear();
        _loyaltyLevel.Clear();
    }
}
