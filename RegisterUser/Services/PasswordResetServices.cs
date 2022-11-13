namespace RegisterUser.Services
{
    public class PasswordResetServices : IPasswordResetServices
    {
        private readonly IMongoCollection<PasswordReset> _passwordResetCollection;
        private readonly UserServices _userServices;
        private readonly EmailConfiguration _emailConfig;

        public PasswordResetServices(IOptions<DatabaseSetting.DatabaseSetting> dbSetting,
            UserServices userServices,
            EmailConfiguration emailConfig
            )
        {
            var client = new MongoClient(dbSetting.Value.connectionString);
            var db = client.GetDatabase(dbSetting.Value.databaseName);
            _passwordResetCollection = db.GetCollection<PasswordReset>(dbSetting.Value.passwordResetCollection);
            _userServices = userServices;
            _emailConfig = emailConfig;
        }
        
        public string RequestPasswordReset(string userEmail)
        {
            var user =_userServices.FindByEmail(userEmail);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var token = new PasswordReset
            {
                userId = user.userId,
                token = GenerateOTP()
            };
            if (_passwordResetCollection.Find(x => x.userId == user.userId).FirstOrDefault() != null)
            {
                 _passwordResetCollection.ReplaceOneAsync(x => x.userId == user.userId, token);
            }
            else
            {
                _passwordResetCollection.InsertOne(token);
            }
            var message = new Message(new string[] { user.email }, "Dopamine password Reset Key", "OTP is valid for only 1 hour. Your password change OTP is : " + token.token);
            SendEmailAsync(message);
            return user.userId.ToString();
        }

        //reset password
        public async Task ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var token = await _passwordResetCollection.Find(x => x.userId == resetPasswordDto.userId).FirstOrDefaultAsync();
            if (token == null)
            {
                throw new Exception("Token not found");
            }
            if (token.token != resetPasswordDto.token)
            {
                throw new Exception("Invalid token");
            }
            if (Convert.ToDateTime(token.expiryDate) < DateTime.Now)
            {
                throw new Exception("Token expired");
            }
            var user = await _userServices.GetUserByIdAsync(resetPasswordDto.userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            user.password = PasswordHash.HashPassword(resetPasswordDto.password);
            await _userServices.UpdateAsync(user);
            await _passwordResetCollection.DeleteOneAsync(x => x.userId == resetPasswordDto.userId);
        }


        //6 digit otp generator
        private string GenerateOTP()
        {
            int min = 100000;
            int max = 999999;
            Random random = new Random();
            return random.Next(min, max).ToString();
        }

        private void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private async Task SendEmailAsync(Message message)
        {
            var mailMessage = CreateEmailMessage(message);

            await SendAsync(mailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(string.Empty, _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };



            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                    client.Send(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception, or both.
                    throw new NotImplementedException();

                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception, or both.
                    throw new NotImplementedException();

                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }


    }
}
