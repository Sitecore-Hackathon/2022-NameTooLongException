namespace NTLE.Project.DemoSite.Rendering.Configuration
{
    /// <summary>
    /// Boxever Configuration
    /// </summary>
    public class BoxeverOptions
    {
        public string ClientKey { get; set; }
        public string ClientSecret { get; set; }
        public BoxeverSettings Configuration { get; set; }
    }
    public class BoxeverSettings
    {
        public string Channel { get; set; }
        public string CurrencyCode { get; set; }
        public string FriendlyId { get; set; }
        public string Language { get; set; }
        public string PointOfSale { get; set; }
    }

}
