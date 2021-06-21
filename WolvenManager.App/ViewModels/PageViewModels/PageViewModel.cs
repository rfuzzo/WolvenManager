using System;
using ReactiveUI;
using Splat;
using WolvenManager.App.Attributes;
using WolvenManager.App.Services;

namespace WolvenManager.App.ViewModels.PageViewModels
{
     
    public abstract class PageViewModel : MainViewModel, IRoutableViewModel
    {
        public string UrlPathSegment { get; }

        public IScreen HostScreen { get; }

        public readonly ISettingsService _settingsService;
        protected readonly IInteractionService _interactionService;
        protected readonly INotificationService _notificationService;


        protected PageViewModel(Type t, IScreen screen = null)
        {
            _settingsService = Locator.Current.GetService<ISettingsService>();
            _interactionService = Locator.Current.GetService<IInteractionService>();
            _notificationService = Locator.Current.GetService<INotificationService>();

            UrlPathSegment = GetAttributes<RoutingUrlAttribute>(t).ToString();


            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
        }


        private static Constants.RoutingIDs GetAttributes<T>(Type t) where T: Attribute
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
