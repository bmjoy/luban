using Luban.Job.Cfg.DataVisitors;
#if !LUBAN_LITE
using Luban.Job.Cfg.l10n;
#endif

namespace Luban.Job.Cfg.Datas
{
    public class DText : DType
    {
        public const string KEY_NAME = "key";
        public const string TEXT_NAME = "text";

        public string Key { get; }

        private readonly string _rawValue;

        public string RawValue => _rawValue;

        public override string TypeName => "text";

        public DText(string key, string x)
        {
            Key = key;
            _rawValue = x;
        }

#if !LUBAN_LITE
        public string GetText(TextTable stringTable, NotConvertTextSet notConvertKeys)
        {
            if (stringTable != null)
            {
                if (stringTable.TryGetText(Key, out var text))
                {
                    return text;
                }
                else if (notConvertKeys != null)
                {
                    notConvertKeys.Add(Key, _rawValue);
                }
            }
            return _rawValue;
        }
#endif

        public override void Apply<T>(IDataActionVisitor<T> visitor, T x)
        {
            visitor.Accept(this, x);
        }

        public override void Apply<T1, T2>(IDataActionVisitor<T1, T2> visitor, T1 x, T2 y)
        {
            visitor.Accept(this, x, y);
        }

        public override TR Apply<TR>(IDataFuncVisitor<TR> visitor)
        {
            return visitor.Accept(this);
        }

        public override TR Apply<T, TR>(IDataFuncVisitor<T, TR> visitor, T x)
        {
            return visitor.Accept(this, x);
        }

        public override bool Equals(object obj)
        {
            return obj is DText o && o._rawValue == this._rawValue && o.Key == this.Key;
        }

        public override int GetHashCode()
        {
            return _rawValue.GetHashCode();
        }
    }
}
