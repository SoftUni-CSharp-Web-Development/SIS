namespace IRunesWebApp.Models
{
    public abstract class BaseEntity<TKeyIdentifier>
    {
        public TKeyIdentifier Id { get; set; }
    }
}
