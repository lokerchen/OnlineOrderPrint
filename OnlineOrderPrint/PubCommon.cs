using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetServerEmail;

namespace OnlineOrderPrint
{
    public class PubCommon
    {
        public static int GetRadioBtnValue(string radioValue)
        {
            switch (radioValue)
            {
                case "ONE":
                    return 1;
                case "TWO":
                    return 2;
                case "THREE":
                    return 3;
                default:
                    return 1;
            }
        }
    }
}
