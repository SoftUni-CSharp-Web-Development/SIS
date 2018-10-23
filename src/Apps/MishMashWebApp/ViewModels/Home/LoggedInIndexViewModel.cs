using System;
using System.Collections.Generic;
using System.Text;
using MishMashWebApp.ViewModels.Channels;

namespace MishMashWebApp.ViewModels.Home
{
    public class LoggedInIndexViewModel
    {
        public string UserRole { get; set; }

        public IEnumerable<BaseChannelViewModel> YourChannels { get; set; }

        public IEnumerable<BaseChannelViewModel> SuggestedChannels { get; set; }

        public IEnumerable<BaseChannelViewModel> SeeOtherChannels { get; set; }
    }
}
