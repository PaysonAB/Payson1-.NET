using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaysonIntegration.Utils
{
    public enum PaymentUpdateAction
    {
        CancelOrder,
        ShipOrder,
        CreditOrder,
        Refund,
    }
}
