namespace TestCrypto.APIs
{
    class CoinDataApi
    {
        public int Rank { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal Supply { get; set; }
        public decimal PriceUsd { get; set; }
        public decimal ChangePercent24Hr { get; set; }
        public string Explorer { get; set; }
    }
}