using dotNetty_kcp.thread;

namespace dotNetty_kcp
{
    public class CloseTask:ITask
    {
        private Ukcp _ukcp;

        public CloseTask(Ukcp ukcp)
        {
            _ukcp = ukcp;
        }

        public void execute()
        {
            _ukcp.internalClose();
        }
    }
}