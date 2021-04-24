using System;
using ReactiveUI;
using Splat;
using WolvenManager.App.Attributes;

namespace WolvenManager.App.ViewModels.PageViewModels
{
     
    public abstract class PageViewModel : MainViewModel, IRoutableViewModel
    {
        public string UrlPathSegment { get; }

        public IScreen HostScreen { get; }

        protected PageViewModel(Type t, IScreen screen = null)
        {
            UrlPathSegment = GetAttributes<RoutingUrlAttribute>(t).ToString();


            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
        }


        private static Constants.Constants.RoutingIDs GetAttributes<T>(Type t) where T: Attribute
        {
            var attribute = (T)Attribute.GetCustomAttribute(t, typeof(T));

            switch (attribute)
            {
                case RoutingUrlAttribute routingUrlAttribute:
                    return routingUrlAttribute.RoutingId;
                default:
                    throw new NotImplementedException();
            }
        }


    }
}
