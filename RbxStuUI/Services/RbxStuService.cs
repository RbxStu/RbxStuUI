using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RbxStuUI.Services;
public class RbxStuService {
    readonly IHttpClientFactory m_httpClientFactory;
    public RbxStuService(IHttpClientFactory httpClientFactory) {
        m_httpClientFactory = httpClientFactory;
    }

    public bool HasSetupBeenCompleted() => File.Exists("./_completed_setup");
    public bool IsWorkspaceCorrupted() => !HasSetupBeenCompleted() || !Directory.Exists("./workspace");
    public bool AreBinariesCorrupted() {
        if (!HasSetupBeenCompleted())
            return true;   // Installation is pending, corruption assumed.

        if (!Directory.Exists("./bin"))
            return true;   // Missing bins

        var fileDictionary = new Dictionary<string, bool> {
            ["Module.dll"] = false,
            ["injector.exe"] = false
        };

        foreach (var file in Directory.EnumerateFiles("./bin")) {
            if (fileDictionary.ContainsKey(file))
                fileDictionary[file] = true;
        }

        return fileDictionary.All(x => x.Value); // Must evaluate all to true, else not good.
    }

    public async Task DownloadBinariesAsync() {
        var client = m_httpClientFactory.CreateClient() ?? throw new Exception("Failed to obtain an HttpClient from the HttpClientFactory!");
    }
}
