using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RbxStuUI.Services;
public class RbxStuService {
    public bool HasSetupBeenCompleted() => File.Exists("./_completed_setup");
    public bool IsWorkspaceCorrupted() => !HasSetupBeenCompleted() || !Directory.Exists("./workspace");
    public bool AreBinariesCorrupted() {
        if (!HasSetupBeenCompleted())
            return true;   // Installation is pending, corruption assumed.

        if (!Directory.Exists("./bin"))
            return true;   // Missing bins

        foreach (var file in Directory.EnumerateFiles("./bin")) {
            
        }
        return true;
    }

    public async Task DownloadBinariesAsync() {

    }
}
