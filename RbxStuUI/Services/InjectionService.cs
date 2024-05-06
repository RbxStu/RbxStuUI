using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RbxStuUI.Services;

[Flags]
public enum ProcessAccessRights : uint {
    PROCESS_TERMINATE = 0x0001,
    PROCESS_CREAATE_THREAD = 0x0002,
    PROCESS_VM_OPERATION = 0x0008,
    PROCESS_VM_READ = 0x0010,
    PROCESS_VM_WRITE = 0x0020,
    PROCESS_DUP_HANDLE = 0x0040,
    PROCESS_CREATE_PROCESS = 0x0080,
    PROCESS_SET_QUOTA = 0x0100,
    PROCESS_SET_INFORMATION = 0x0200,
    PROCESS_QUERY_INFORMATION = 0x0400,
    PROCESS_SUSPEND_RESUME = 0x0800,
    PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
    SYNCHRONIZE = 0x00100000,

    PROCESS_ALL_ACCESS = 0x000F0000 | SYNCHRONIZE | 0xFFFF,
}

public partial class InjectionService {
    #region Unmanaged

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleA", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr GetModuleHandleA(string szModuleName);

    [LibraryImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr GetProcAddress(IntPtr hModule, string szModuleName);

    [LibraryImport("kernel32.dll", EntryPoint = "OpenProcess", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr OpenProcess(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

    [LibraryImport("kernel32.dll", EntryPoint = "VirtualAllocEx", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, ulong dwSize, uint flAllocationType, uint flProtect);

    [LibraryImport("kernel32.dll", EntryPoint = "VirtualFreeEx", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, ulong dwSize, uint flFreeType);

    [LibraryImport("kernel32.dll", EntryPoint = "WriteProcessMemory", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

    [LibraryImport("kernel32.dll", EntryPoint = "CreateRemoteThread", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

    [LibraryImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial void CloseHandle(IntPtr lpHandle);

    [LibraryImport("kernel32.dll", EntryPoint = "WaitForSingleObject", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

    #endregion Unmanaged

    private readonly RbxStuService m_rbxStuService;

    public InjectionService(RbxStuService stuService) {
        m_rbxStuService = stuService;
    }

    public bool CanInject() => !m_rbxStuService.AreBinariesCorrupted();

    public async Task InjectModule() {
        if (!CanInject())
            return;
        var dwRobloxStudioPid = m_rbxStuService.GetRobloxStudioProcessId();
        var hKernel32Module = GetModuleHandleA("kernel32.dll");
        var rfpLoadLibraryA = GetProcAddress(hKernel32Module, "LoadLibraryA");
        var hRobloxStudio = OpenProcess((uint)ProcessAccessRights.PROCESS_ALL_ACCESS, false, (uint)dwRobloxStudioPid);
        var szDllPath = m_rbxStuService.GetModulePath();
        var szDllBytes = Encoding.ASCII.GetBytes( szDllPath);

        // 0x1000 | 0x2000 -> MEM_COMMIT | MEM_RESERVE
        var rpMemory = VirtualAllocEx(hRobloxStudio, 0, (uint) (szDllBytes .Length + 1), (0x1000 | 0x2000), 0x4); // 0x4 == PAGE_READ_WRITE
        WriteProcessMemory(hRobloxStudio, rpMemory, Encoding.ASCII.GetBytes(szDllPath), (uint) szDllBytes.Length, out _);

        var hThread = CreateRemoteThread(hRobloxStudio, IntPtr.Zero, 0, rfpLoadLibraryA, rpMemory, 0, IntPtr.Zero);

        while (WaitForSingleObject(hThread, 10) == 0x00000102) { // WAIT_TIMEOUT
            await Task.Delay(200);
        }   // We will be looping until we either hit -1 (0xffffffff aka WAIT_FAILED) or WAIT_OBJECT_O (Means it finished)

        // Clean up.
        VirtualFreeEx(hRobloxStudio, rpMemory, 0, 0x00008000); // MEM_RELEASE
        CloseHandle(hRobloxStudio);
        CloseHandle(hThread);

    }
}
