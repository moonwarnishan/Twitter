namespace RegisterUser.InterFaces
{
    public interface IRabbitMQService
    {
        IConnection CreateChannel();
    }
}
