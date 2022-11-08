namespace RegisterUser.InterFaces
{
    public interface IRabbitMqDeleteService
    {
        public Task Connect(string userName);
    }
}
