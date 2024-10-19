namespace Identity.WebAPI.Utilities
{
    public class PasswordGenerator
    {
        public static string GeneratePassword(int passwordLength)
        {
            Random random = new Random();
            string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            char[] password = new char[passwordLength];
            for (int i = 0; i < passwordLength; i++)
            {
                password[i] = characters[random.Next(characters.Length)];

            }
            return new string(password);
        }
    }
}
