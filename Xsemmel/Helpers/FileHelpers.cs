using System.IO;

namespace XSemmel.Helpers
{
    public static class FileHelper
    {

        public static bool FileExists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            try
            {
                return File.Exists(filePath);
            }
            catch
            {
                return false;
            }
        }

    }


}
