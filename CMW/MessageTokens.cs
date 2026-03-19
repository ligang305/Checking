namespace CMW
{
    /// <summary>
    /// Messenger 传递消息时的 token
    /// </summary>
    public static class MessageTokens
    {
        // 启动自检
        public static string StartAutoDetect = "StartAutoDetect";

        // 停止自检
        public static string StopAutoDetect = "StopAutoDetect";

        // 重置自检ViewModel的各状态
        public static string ResetAutoDetect = "ResetAutoDetect";

        // 导出自检结果
        public static string ExportAutoDetectResult = "ExportAutoDetectResult";

        // 自检结束
        public static string AutoDetectOver = "AutoDetectOver";
        //sjw添加 清除通道
        public static string MaterialCalClearPassageway = "MaterialCalClearPassageway";
        //sjw添加 清除通道
        public static string MaterialCalTips = "MaterialCalTips";



        // 将自检View的滚动条滚到底部
        public static string AutoDetectScrollToBottom = "AutoDetectScrollToBottom";

        // 主界面启动DR图像处理
        public static string MainWindowDataProcess = "MainWindowDataProcess";

        // 保持 状态栏 传送带 左移右移 按钮 宽度一致
        public static string BottomBarFormatConveyorMoveBtnWidth = "BottomBarFormatConveyorMoveBtnWidth";
    }
}
