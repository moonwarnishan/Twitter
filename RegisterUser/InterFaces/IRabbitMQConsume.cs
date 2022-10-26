namespace RegisterUser.InterFaces
{
    public interface IRabbitMQConsume
    {
        public Task Connect(string userName);
    }
}
