namespace Models;

public class LicenceCountChanged : EmailViewModel
{
    public LicenceCountChanged()
    { }

    public LicenceCountChanged(int count, string email)
    {
        Count = count;
        Email = email;
    }

    public int Count { get; set; }
}
