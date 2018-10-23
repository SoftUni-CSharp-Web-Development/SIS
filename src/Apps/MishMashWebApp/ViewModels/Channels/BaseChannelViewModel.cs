using MishMashWebApp.Models;

namespace MishMashWebApp.ViewModels.Channels
{
    public class BaseChannelViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ChannelType Type { get; set; }

        public int FollowersCount { get; set; }
    }
}
