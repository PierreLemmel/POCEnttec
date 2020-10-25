namespace POCEnttec.OpenDmx
{
    public static class OpenDmxExtensions
    {
        public static void CopyData(this IOpenDmxInterface openDmx, int channelOffset, byte[] data)
            => openDmx.CopyData(channelOffset, data, data.Length);
    }
}