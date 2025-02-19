using Ionic.Zlib;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace zenith
{
    internal class netCatalog
    {
        const string catalogPrefix = "https://prod.cloud.rockstargames.com/titles/gta5/pcros/gamecatalog/";
        static string catalogName = "WkYi7fg8KlWWys5_1FFRHw";

        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                catalogName = args[0];
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var downloadUrl = catalogPrefix + catalogName + ".zip";
                    var filePath = Path.Combine(Environment.CurrentDirectory, $"{catalogName}.zip");
                    using (var response = await client.GetAsync(downloadUrl))
                    {
                        response.EnsureSuccessStatusCode();
                        using (var fileStream = File.Create(filePath))
                        {
                            await response.Content.CopyToAsync(fileStream);
                        }
                    }


                    string decompressedFilePath = Path.Combine(Environment.CurrentDirectory, $"{catalogName}.json");
                    await DecompressFile(filePath, decompressedFilePath);

                    Console.WriteLine($"Loaded file at {decompressedFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
            }
        }

        static async Task DecompressFile(string zipFilePath, string outputFilePath)
        {
            try
            {
                using (var zipStream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read))
                using (var decompressionStream = new DeflateStream(zipStream, CompressionMode.Decompress))
                using (var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {
                    await decompressionStream.CopyToAsync(outputStream);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error : {ex.Message}", ex);
            }
        }
    }
}
