using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace WebServices.Helpers
{
    public static class SociatisTransactionOptions
    {
        public static TransactionOptions ReadUncommited
        {
            get
            {
                var _return = new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted
                };

                return _return;
            }
        }

        public static TransactionOptions Snapshot
        {
            get
            {
                var _return = new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.Snapshot
                };

                return _return;
            }
        }

        public static TransactionOptions RepeatableRead
        {
            get
            {
                var _return = new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.RepeatableRead
                };

                return _return;
            }
        }
        
    }
}
