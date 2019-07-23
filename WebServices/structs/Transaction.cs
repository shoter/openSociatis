using Entities;
using Entities.enums;

namespace WebServices.structs
{
    public class Transaction
    {
        public int? SourceEntityID { get; set; }
        /// <summary>
        /// Null indicated that money will go into black hole
        /// </summary>
        public int? DestinationEntityID { get; set; } 
        public Money Money { get; set; }
        public TransactionTypeEnum  TransactionType { get; set; }
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }

        public bool TakeMoneyFromSource { get; set; } = true;

        public int? SourceWalletID { get; set; }
        public int? DestinationWalletID { get; set; }

    }
}
