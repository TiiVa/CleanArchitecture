using Microsoft.JSInterop;

namespace CleanArchitecture.Client;

public static class IjsRunTimeExtensionMethod
{
    public static async ValueTask DownloadFile(this IJSRuntime js, string filename, byte[] data)
    {
        try
        {
            await js.InvokeVoidAsync("downLoadFile", filename, Convert.ToBase64String(data));
        }
        catch (Exception ex)
        {
            throw new ApplicationException(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
        }
    }

    public static async ValueTask SaveAs(this IJSRuntime js, string filename, byte[] data)
    {
        try
        {
            await js.InvokeVoidAsync("saveAsFile", filename, Convert.ToBase64String(data));
        }
        catch (Exception ex)
        {
            throw new ApplicationException(ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
        }
    }
}