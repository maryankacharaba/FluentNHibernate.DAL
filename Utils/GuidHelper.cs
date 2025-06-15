namespace FluentNHibernate.DAL.Utils;

public class GuidHelper
{
    // https://stackoverflow.com/questions/1383030/how-to-combine-two-guid-values
    public static Guid Combine(Guid x, Guid y)
    {
        byte[] a = x.ToByteArray();
        byte[] b = y.ToByteArray();

        return new Guid(BitConverter.GetBytes(BitConverter.ToUInt64(a, 0) ^ BitConverter.ToUInt64(b, 8))
            .Concat(BitConverter.GetBytes(BitConverter.ToUInt64(a, 8) ^ BitConverter.ToUInt64(b, 0))).ToArray());
    }
}