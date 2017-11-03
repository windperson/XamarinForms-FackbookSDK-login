using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinFacebookDemo.Service;
using XamarinFacebookDemo.ViewModel;

namespace XamarinFacebookDemo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var fbService = DependencyService.Get<IFBLoginService>();
            BindingContext = new MainPageViewModel(fbService);
        }
    }
}
