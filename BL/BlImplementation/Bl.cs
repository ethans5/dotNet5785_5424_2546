
namespace BlImplementation;
using BlApi;
internal class Bl : IBl
{
    public IAdmin Admin => new AdminImplementation();

    public ICall Call => new CallImplementation();

    public IVolunteer Volunteer => new VolunteerImplementation();
}
