namespace GetServerEmail
{
    public static class HtmlTextPath
    {
        //Order ID:#XXXXXX
        public static string HEAD_ORDER_ID = @"//html/body/div[1]/div[1]/strong[1]/h3[1]";

        //COLLECTION ORDER
        public static string HEAD_ORDER_TYPE = @"//html/body/div[1]/div[1]/strong[1]/h3[2]";

        //Name:XXX
        public static string BODY_NAME = @"//html[1]/body[1]/div[1]/p[1]/strong[1]";

        //Phone:XXXXXXXX
        public static string BODY_PHONE = @"//html[1]/body[1]/div[1]/p[2]/strong[1]";

        //Order Time:&nbsp;05/04/2018 - 22:01 
        public static string BODY_ORDER_TIME = @"//html[1]/body[1]/div[1]/p[3]/strong[1]";

        //Code
        public static string BODY_TABLE_CODE = @"//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/thead[1]/tr[1]/td[1]/strong[1]";

        //Qty
        public static string BODY_TABLE_QTY = @"//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/thead[1]/tr[1]/td[2]/strong[1]";

        //Name
        public static string BODY_TABLE_NAME = @"//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/thead[1]/tr[1]/td[3]/strong[1]";

        //Price
        public static string BODY_TABLE_PRICE = @"//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/thead[1]/tr[1]/td[4]/strong[1]";
    }
}