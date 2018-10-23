namespace MishMashWebApp.Models
{
    public class ChannelTag
    {
        public int Id { get; set; }

        public int ChannelId { get; set; }

        public virtual Channel Channel { get; set; }

        public int TagId { get; set; }

        public virtual Tag Tag { get; set; }
    }
}