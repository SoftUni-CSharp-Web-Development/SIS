namespace SIS.MvcFramework
{
    public class MvcUserInfo
    {
        private const string Separator = "_____________________";

        public MvcUserInfo()
        {
        }

        public MvcUserInfo(string serializedInfo)
        {
            var infoParts = serializedInfo.Split(Separator);
            this.Username = infoParts[0];
            this.Role = infoParts[1];
            this.Info = infoParts[2];
        }

        public string Username { get; set; }

        public string Role { get; set; }

        public string Info { get; set; }

        public bool IsLoggedIn => this.Username != null;

        public override string ToString()
        {
            return $"{this.Username}{Separator}{this.Role}{Separator}{this.Info}";
        }
    }
}
