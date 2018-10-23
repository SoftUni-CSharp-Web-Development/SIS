using System;
using System.Collections.Generic;
using System.Text;

namespace MishMashWebApp.ViewModels.Channels
{
    public class FollowedChannelsViewModel
    {
        public IEnumerable<BaseChannelViewModel> FollowedChannels { get; set; }
    }
}

