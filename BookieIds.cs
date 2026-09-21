namespace Michael.BookieTrader;

/// <summary>
/// All custom template/trader ids used by Bookie Trader V0.1.
/// Keep these stable after a public release so existing profiles do not break.
/// </summary>
public static class BookieIds
{
    public const string Trader = "1fef29b536eff3a271b03137";

    // Vanilla templates / parents.
    public const string Roubles = "5449016a4bdc2d6f028b456f";
    public const string PhysicalBitcoin = "59faff1d86f7746c51718c9c";
    public const string GpCoin = "5d235b4d86f7742e017bc88a";
    public const string SealedWeaponCrate = "6489b2b131a2135f0d7d0fcb";
    public const string RandomLootContainerParent = "62f109593b54472778797866";
    public const string BarterItemParent = "5448eb774bdc2d0a728b4567";
    public const string BarterHandbookParent = "5b47574386f77428ca22b33f";

    // Result items.
    public const string Bust = "d0fa36ac75109161809b2323";
    public const string Chip100K = "7b80abe742d98bfa7cc63fd1";
    public const string Chip200K = "0e7eefd2ad6321ed7150f964";
    public const string Chip400K = "242b06d1ec5ac486d14f86cf";
    public const string Chip500K = "5ec8149e2030bb70ae785329";
    public const string Chip800K = "0b2e595a35ea3f39346c3fd2";
    public const string Chip1M = "8727c4ed91154d07c44b2497";
    public const string Chip1Point6M = "0d3624033c9ce7d98a847133";
    public const string Chip2M = "e206dfcdf04d9a46eb6b4c05";
    public const string Chip5M = "41d5e1b89196684aa323c9c6";
    public const string Btc2Chip = "533b7d29f49988e2a3695bae";
    public const string Btc4Chip = "3698f809bf6e53c86f92229d";

    // Straight bets.
    public const string Bet50K = "034e05d4c2b1b8f96a5c1e13";
    public const string Bet100K = "3ba2ca79f17f43e0ee557a9a";
    public const string Bet250K = "67f5eeab1bea7caa79ab7b68";
    public const string Bet500K = "bfea905883f44593b978ff5d";
    public const string Bet1M = "846f48a251c539fb745f8566";

    // Let-it-ride streak tickets.
    public const string Ride200K = "cb21bdcf25504832536203ee";
    public const string Ride400K = "6ca6849bbe0634dd5e70f26e";
    public const string Ride800K = "006dcb26c7323dcec6c64ef9";

    // Bitcoin table.
    public const string Btc1Bet = "9e62b8ab907e9f55bfa4baf4";
    public const string Btc2Ride = "03a2a5642c38982732ba0500";
    public const string Btc2Payout = "931d038c85add8aad14f1f22";
    public const string Btc4Payout = "5e70a30e32c8ddd846d340a6";

    // Mystery money tables.
    public const string Lucky7 = "efd78dc7590e79a2f81bdef3";
    public const string HighRoller = "54caea6ebc1f072f41b9c3ee";

    public static readonly string[] CashChipIds =
    [
        Chip100K,
        Chip200K,
        Chip400K,
        Chip500K,
        Chip800K,
        Chip1M,
        Chip1Point6M,
        Chip2M,
        Chip5M
    ];
}
