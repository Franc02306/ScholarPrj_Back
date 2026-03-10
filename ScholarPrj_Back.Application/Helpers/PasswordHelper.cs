using System.Security.Cryptography;
using System.Text;

namespace ScholarPrj_Back.Application.Helpers
{
    public class PasswordHelper
    {
        private const int DEFAULT_LENGTH = 12;
        private const int MAX_LENGTH = 255;

        private static readonly char[] _chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@$%*?".ToCharArray();

        /// <summary>
        /// Genera una contraseña aleatoria
        /// </summary>
        public static string GenerateRandom(int length = DEFAULT_LENGTH)
        {
            if (length <= 0 || length > MAX_LENGTH)
                throw new ArgumentException($"La longitud debe estar entre 1 y {MAX_LENGTH}");

            var data = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(data);

            var result = new StringBuilder(length);

            foreach (var b in data)
                result.Append(_chars[b % _chars.Length]);

            return result.ToString();
        }

        /// <summary>
        /// Genera hash BCrypt de la contraseña
        /// </summary>
        public static string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña no puede ser vacía");

            if (password.Length > MAX_LENGTH)
                throw new ArgumentException($"La contraseña no puede exceder {MAX_LENGTH} caracteres");

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifica contraseña contra hash
        /// </summary>
        public static bool Verify(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        /// <summary>
        /// Valida si la contrasñea cumple con la política de seguridad
        /// </summary>
        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < 8 || password.Length > MAX_LENGTH)
                return false;

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;
            bool hasSymbol = false;

            foreach (var c in password)
            {
                if (char.IsUpper(c))
                    hasUpper = true;
                else if (char.IsLower(c))
                    hasLower = true;
                else if (char.IsDigit(c))
                    hasDigit = true;
                else
                    hasSymbol = true;
            }

            return hasUpper && hasLower && hasDigit && hasSymbol;
        }
    }
}
