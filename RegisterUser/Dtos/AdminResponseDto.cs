namespace RegisterUser.Dtos
{
    public class AdminResponseDto
    {
        public string name { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string adminStat { get; set; }
        public bool blockStat { get; set; }

        public string dateOfBirth { get; set; }
    }
}
