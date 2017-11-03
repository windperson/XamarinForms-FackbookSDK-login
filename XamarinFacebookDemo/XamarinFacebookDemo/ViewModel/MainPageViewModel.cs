using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinFacebookDemo.Annotations;
using XamarinFacebookDemo.Model;
using XamarinFacebookDemo.Service;

namespace XamarinFacebookDemo.ViewModel
{
    class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly IFBLoginService _fbLoginService;

        #region UIState

        public FBUser FacebookUser { get; private set; }

        public bool IsLogin { get; private set; }

        #endregion

        #region ViewModelCommand

        public ICommand LoginCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        #endregion

        public MainPageViewModel(IFBLoginService fbLoginService)
        {
            _fbLoginService = fbLoginService;

            WireUpCmd();
        }

        private void WireUpCmd()
        {
            LoginCommand = new Command(FB_login);
            LogoutCommand = new Command(FB_logout);
        }
        private void FB_login()
        {
            _fbLoginService?.Login(OnLoginComplete);
        }

        private void FB_logout()
        {
            _fbLoginService?.Logout();
            IsLogin = false;
            FacebookUser = null;
            OnPropertyChanged("IsLogin");
            OnPropertyChanged("FacebookUser");
        }


        private void OnLoginComplete(FBUser facebookUser, string message)
        {
            if (facebookUser != null)
            {
                FacebookUser = facebookUser;
                IsLogin = true;
                OnPropertyChanged("IsLogin");
                OnPropertyChanged("FacebookUser");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("login failed, reason=" + message);

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
