namespace TalkWithAyodeji.Repository.Seeder
{
    public interface IAdminSeed
    {
        Task<int> AddDefaultAdmin();
    }
}
