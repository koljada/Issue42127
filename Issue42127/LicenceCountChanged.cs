using Models;

namespace Issue42127;

public class LicenceCountChangedWithException : EmailViewModel
{
    public LicenceCountChangedWithException()
    { }

    public LicenceCountChangedWithException(int count, string email)
    {
        Count = count;
        Email = email;
    }

    public int Count { get; set; }
}
