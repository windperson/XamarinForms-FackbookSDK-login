using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamarinFacebookDemo.Model;

namespace XamarinFacebookDemo.Service
{
    public interface IFBLoginService
    {
        void Login(Action<FBUser, string> onLoginComplete);
        void Logout();
    }
}
