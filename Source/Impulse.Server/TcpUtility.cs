using System.Net.Sockets;

namespace Impulse.Server;

internal class TcpUtility
{
    public static int GetAvailablePort(int start, int end)
    {
        if (!TryGetAvailablePort(start, end, out var port))
        {
            throw new InvalidOperationException($"Could not find an available port between '{start}' and '{end}'");
        }

        return port;
    }

    public static bool TryGetAvailablePort(int start, int end, out int port)
    {
        var length = end - start;
        for (var i = 0; i <= length; ++i)
        {
            port = start + i;
            if (TryPort(port))
            {
                return true;
            }
        }

        port = -1;
        return false;
    }

    private static bool TryPort(int port)
    {
        var listener = TcpListener.Create(port);
        try
        {
            listener.ExclusiveAddressUse = true;
            listener.Start();
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
        finally
        {
            listener.Stop();
        }
    }
}