using Netwrok;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityFramework
{
    //管理整个库的生命周期
    public class UnityFrameworkManager : Singleton_CSharp<UnityFrameworkManager>
    {
        public void Init()
        {
        }

        public void Update()
        {
            NetworkManager.Update();
        }

        public void Connect()
        {
            NetworkManager.Connect("Main", "127.0.0.1", 8999, false);
        }
    }
}
