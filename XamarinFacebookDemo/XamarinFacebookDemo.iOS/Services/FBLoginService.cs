using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Foundation;
using UIKit;
using XamarinFacebookDemo.Model;
using XamarinFacebookDemo.Service;

[assembly: Xamarin.Forms.Dependency(typeof(XamarinFacebookDemo.iOS.Services.FBLoginService))]
namespace XamarinFacebookDemo.iOS.Services
{
    public class FBLoginService : IFBLoginService
    {
        private Action<FBUser, string> _onLoginComplete;

        public void Login(Action<FBUser, string> onLoginComplete)
        {
            _onLoginComplete = onLoginComplete;

            var viewController = GetPresetedViewController();

            var tcs = new TaskCompletionSource<FBUser>();

            var loginManager = new LoginManager();
            loginManager.LogOut();
            loginManager.LoginBehavior = LoginBehavior.SystemAccount;
            loginManager.LogInWithReadPermissions(new[] { "public_profile", "email" }, viewController,
                (result, error) =>
                {
                    if (error != null || result == null || result.IsCancelled)
                    {
                        if (error != null)
                        {
                            _onLoginComplete?.Invoke(null, error.LocalizedDescription);
                        }
                        if (result.IsCancelled)
                        {
                            _onLoginComplete?.Invoke(null, "user cancelled.");
                        }
                    }
                    else
                    {
                        var request = new GraphRequest("me",
                            new NSDictionary("fields",
                                "id, first_name, email, last_name, picture.width(1000).height(1000)"));

                        request.Start(
                            (conn, fetched, fetchErr) =>
                            {
                                if (fetchErr != null || fetched == null)
                                {
                                    Debug.WriteLine(fetchErr.LocalizedDescription);
                                    tcs.TrySetResult(null);
                                }
                                else
                                {
                                    var id = string.Empty;
                                    var first_name = string.Empty;
                                    var email = string.Empty;
                                    var last_name = string.Empty;
                                    var url = string.Empty;

                                    try
                                    {
                                        id = fetched.ValueForKey(new NSString("id"))?.ToString();
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.WriteLine(e.Message);
                                    }

                                    try
                                    {
                                        first_name = fetched.ValueForKey(new NSString("first_name"))?.ToString();
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.WriteLine(e.Message);
                                    }

                                    try
                                    {
                                        last_name = fetched.ValueForKey(new NSString("last_name"))?.ToString();
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.WriteLine(e.Message);
                                    }

                                    try
                                    {
                                        email = fetched.ValueForKey(new NSString("email"))?.ToString();
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.WriteLine(e.Message);
                                    }


                                    try
                                    {
                                        url = ((fetched.ValueForKey(new NSString("picture")) as NSDictionary)?.ValueForKey(new NSString("data")) as NSDictionary)?.ValueForKey(new NSString("url")).ToString();
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.WriteLine(e.Message);
                                    }
                                    var fbUser = new FBUser()
                                    {
                                        Id = id,
                                        Email = email,
                                        FirstName = first_name,
                                        LastName = last_name,
                                        PicUrl = url,
                                        Token = result.Token.TokenString
                                    };
                                    tcs.TrySetResult(fbUser);
                                    _onLoginComplete?.Invoke(fbUser, string.Empty);

                                }
                            }
                        );
                    }


                });
        }

        private UIViewController GetPresetedViewController()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }
            return vc;
        }

        public void Logout()
        {
            var loginManager = new LoginManager();
            loginManager.LogOut();
        }
    }
}