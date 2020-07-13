namespace GetServerEmail
{
    public static class HtmlTextPath
    {
        //邮箱链接超时时长
        public static int EMAIL_TIME_OUT = 20000;

        //链接邮箱端口
        public static int EMAIL_PORT = 110;

        //Order ID:#XXXXXX
        public static string HEAD_ORDER_ID = @"//html/body/div[1]/div[1]/strong[1]/h3[1]";

        //COLLECTION ORDER
        public static string HEAD_ORDER_TYPE = @"//html/body/div[1]/div[1]/strong[1]/h3[2]";

        //Name:XXX
        public static string BODY_NAME = @"//html[1]/body[1]/div[1]/p[1]/strong[1]";

        //Phone:XXXXXXXX
        public static string BODY_PHONE = @"//html[1]/body[1]/div[1]/p[2]/strong[1]";

        //Order Time:&nbsp;05/04/2018 - 22:01 
        public static string BODY_COLLECTION_ORDER_TIME = @"//html[1]/body[1]/div[1]/p[3]/strong[1]";

        public static string BODY_DELIVER_ORDER_TIME = @"//html[1]/body[1]/div[1]/p[7]/strong[1]";

        //Order Type
        public static string ORDER_TYPE_DELIVERY = @"DELIVERY";

        public static string ORDER_TYPE_COLLECTION = @"COLLECTION";

        //Code
        public static string BODY_TABLE_CODE = @"//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/thead[1]/tr[1]/td[1]/strong[1]";

        //Qty
        public static string BODY_TABLE_QTY = @"//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/thead[1]/tr[1]/td[2]/strong[1]";

        //Name
        public static string BODY_TABLE_NAME = @"//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/thead[1]/tr[1]/td[3]/strong[1]";

        //Price
        public static string BODY_TABLE_PRICE = @"//html[1]/body[1]/div[1]/strong[1]/strong[1]/center[1]/table[1]/thead[1]/tr[1]/td[4]/strong[1]";

        //打印字体大小
        public static float PRT_FONT_SIZE = 11.0f;

        //打印字体
        public static string PRT_FONT = @"Arial";

        //小写字母个数
        public static int PRT_WORD_LOWER_NUM = 40;
        //大写字母个数
        public static int PRT_WORD_TOPPER_NUM = 26;
        //数字个数
        public static int PRT_NUM_NUM = 32;
        //汉字个数
        public static int PRT_HANZI_NUM = 16;
        //MenuItem个数
        public static int PRT_MENUITEM_NUM = 24; //36(PRT_WORD_LOWER_NUM) - 6(Code+2个空格) - 5(Qty+2个空格) - 5(Price） - 4(Name)
        //Code
        public static int PRT_CODE = 9;
        //Qty
        public static int PRT_QTY = 5;
        //偏移量
        public static int PRT_OFFSET = 5;

        //打印次数
        public static string PRT_COUNT_ONE = @"ONE";
        public static string PRT_COUNT_TWO = @"TWO";
        public static string PRT_COUNT_THREE = @"THREE";
    }
}