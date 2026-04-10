using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
namespace FoodBook_SS.Desktop.Core
{
    public static class SessionManager
    {
        private const string TokenFile       = "fb_token.dat";
        private const string UserFile         = "fb_user.dat";
        private const string RolFile          = "fb_rol.dat";
        private const string RestauranteFile  = "fb_rest.dat";
        private const string UserIdFile       = "fb_uid.dat";
        public static void Save(string token, string nombreUsuario, string rol,
                                int usuarioId = 0, int restauranteId = 0)
        {
            Write(TokenFile, token);
            Write(UserFile,  nombreUsuario);
            Write(RolFile,   rol);
            Write(UserIdFile, usuarioId.ToString());
            Write(RestauranteFile, restauranteId.ToString());
        }
        public static string? GetToken()       => Read(TokenFile);
        public static string? GetUsuario()     => Read(UserFile);
        public static string? GetRol()         => Read(RolFile);
        public static bool    IsLoggedIn()     => !string.IsNullOrEmpty(GetToken());
        public static int     GetUserId()      => int.TryParse(Read(UserIdFile), out var id) ? id : 0;
        public static int     GetRestauranteId() => int.TryParse(Read(RestauranteFile), out var id) ? id : 0;
        public static void Clear()
        {
            Delete(TokenFile);
            Delete(UserFile);
            Delete(RolFile);
            Delete(UserIdFile);
            Delete(RestauranteFile);
        }
        private static void Write(string file, string content)
        {
            using var store = IsolatedStorageFile.GetUserStoreForAssembly();
            using var stream = new IsolatedStorageFileStream(file, FileMode.Create, store);
            using var writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(content);
        }
        private static string? Read(string file)
        {
            try
            {
                using var store = IsolatedStorageFile.GetUserStoreForAssembly();
                if (!store.FileExists(file)) return null;
                using var stream = new IsolatedStorageFileStream(file, FileMode.Open, store);
                using var reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
            catch { return null; }
        }
        private static void Delete(string file)
        {
            try
            {
                using var store = IsolatedStorageFile.GetUserStoreForAssembly();
                if (store.FileExists(file)) store.DeleteFile(file);
            }
            catch {  }
        }
    }
}

