namespace BGCommunication
{
    public class BaseSocketProtocol : ICommonProtocol
    {
        public BaseSocketProtocol()
        {
            ProtocolName = "控制站-基础协议";
        }
        protected override void PrepareSend(byte[] SendBuf)
        {
            ConnIntf.SendedBuf = null;
            CorrectRet.Clear();
            NeedFeedBack = false;
            ExeResult = false;
        }
        
    }
}
