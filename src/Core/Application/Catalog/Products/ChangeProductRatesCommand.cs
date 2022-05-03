using FSH.WebApi.Application.Common.Commands;

namespace FSH.WebApi.Application.Catalog.Products
{
    public class ChangeProductRatesCommand : CommandRequest 
    {

        public double Percent { get; set; }
        public bool RevertChange { get; set; }
        public Guid EntityId { get; set; }

        public ChangeProductRatesCommand(double percentage, Guid brandId, string userId, string userName = "", bool revertChange = false)
            : base(userId, userName)
        {
            Percent = percentage;
            EntityId = brandId;
            RevertChange = revertChange;
        }


    }
}

