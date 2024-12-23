namespace BlApi;

internal class Factory
{
    public static IBl Get() => new BlImplementation.Bl();
}
