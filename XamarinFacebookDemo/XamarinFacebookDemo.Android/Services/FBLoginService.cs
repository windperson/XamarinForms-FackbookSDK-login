using System;
using System.Collections.Generic;
using Android.App;
using Org.Json;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using XamarinFacebookDemo.Model;
using XamarinFacebookDemo.Service;

[assembly: Xamarin.Forms.Dependency(typeof(XamarinFacebookDemo.Droid.Services.FBLoginService))]
namespace XamarinFacebookDemo.Droid.Services
{
    public class FBLoginService : Java.Lang.Object, IFBLoginService, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback
    {
        private static FBLoginService _instance;
        public static FBLoginService Instance => _instance;

        private Action<FBUser, string> _onLoginComplete;
        public ICallbackManager FBCallbackManager;

        public FBLoginService()
        {
            FBCallbackManager = CallbackManagerFactory.Create();
            LoginManager.Instance.RegisterCallback(FBCallbackManager, this);
            FBLoginService._instance = this;
        }


        #region IFBLoginService implementation

        public void Login(Action<FBUser, string> onLoginComplete)
        {
            _onLoginComplete = onLoginComplete;
            LoginManager.Instance.SetLoginBehavior(LoginBehavior.NativeWithFallback);
            var activity = Xamarin.Forms.Forms.Context as Activity;
            LoginManager.Instance.LogInWithReadPermissions(activity, new List<string>{"public_profile","email"});
        }

        public void Logout()
        {
            LoginManager.Instance.LogOut();
        }

        #endregion

        #region IFaceookCallback implementation

        public void OnCancel()
        {
            _onLoginComplete?.Invoke(null, "Canceled!");
        }

        public void OnError(FacebookException error)
        {
            _onLoginComplete?.Invoke(null, error.Message);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            if (result is LoginResult loginResult)
            {
                var request = GraphRequest.NewMeRequest(loginResult.AccessToken, this);
                var bundle = new Android.OS.Bundle();
                bundle.PutString("fields", "id, first_name, email, last_name, picture.width(500).height(500)");
                request.Parameters = bundle;
                request.ExecuteAsync();
            }
        }

        #endregion


        public void OnCompleted(JSONObject jsonObject, GraphResponse response)
        {
            var id = string.Empty;
            var first_name = string.Empty;
            var last_name = string.Empty;
            var email = string.Empty;
            var pictureUrl = string.Empty;

            if (jsonObject.Has("id"))
            {
                id = jsonObject.GetString("id");
            }

            if (jsonObject.Has("first_name"))
            {
                first_name = jsonObject.GetString("first_name");
            }

            if (jsonObject.Has("last_name"))
            {
                last_name = jsonObject.GetString("last_name");
            }

            if (jsonObject.Has("email"))
            {
                email = jsonObject.GetString("email");
            }

            if (jsonObject.Has("picture"))
            {
                var picture = jsonObject.GetJSONObject("picture");
                if (picture.Has("data"))
                {
                    var data = picture.GetJSONObject("data");
                    if (data.Has("url"))
                    {
                        pictureUrl = data.GetString("url");
                    }
                }
            }

            var fbUser = new FBUser()
            {
                Id = id,
                Email = email,
                FirstName = first_name,
                LastName = last_name,
                PicUrl = pictureUrl,
                Token = AccessToken.CurrentAccessToken.Token
            };

            _onLoginComplete?.Invoke(fbUser,string.Empty);
        }
    }
}
