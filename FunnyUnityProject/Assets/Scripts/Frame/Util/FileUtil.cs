using System.IO;

namespace GFrame
{
    public class FileUtil
    {
        private static void CopyFolder(string srcFolderPath, string destFolderPath)
        {
            if (!Directory.Exists(srcFolderPath)) return;
            if (!Directory.Exists(destFolderPath)) Directory.CreateDirectory(destFolderPath);
            foreach (string dirPath in Directory.GetDirectories(srcFolderPath, "*", SearchOption.AllDirectories))
            {
                string destPath = dirPath.Replace(srcFolderPath, destFolderPath);
                if (!Directory.Exists(destPath))
                    Directory.CreateDirectory(destPath);
            }

            foreach (string newPath in Directory.GetFiles(srcFolderPath, "*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(srcFolderPath, destFolderPath), true);
        }
    }
}