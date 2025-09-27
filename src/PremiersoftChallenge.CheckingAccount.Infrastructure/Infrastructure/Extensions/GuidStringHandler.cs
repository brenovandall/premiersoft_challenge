using Dapper;
using System.Data;

namespace Infrastructure.Extensions
{
    public class GuidStringHandler : SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value)
        {
            return value is string s ? Guid.Parse(s) : Guid.Empty;
        }

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value.ToString();
        }
    }
}
