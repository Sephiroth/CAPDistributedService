namespace CAPService.Impl
{
    public interface IUserSubscriberService
    {
        System.Threading.Tasks.ValueTask InsertUser(Model.UserCompanyMap user);
    }
}