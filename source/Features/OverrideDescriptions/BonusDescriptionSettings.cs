using Localize;

namespace MechEngineer.Features.OverrideDescriptions
{
    internal class BonusDescriptionSettings
    {
        private string FShort;
        private string FLong;
        private string FFull;
#pragma warning disable 649
        public string Bonus;
        public string Short { get { return new Text(FShort).ToString(); } set { FShort = value; } }
        public string Long { get { return new Text(FLong).ToString(); } set { FLong = value; } }
        public string Full { get { return new Text(FFull).ToString(); } set { FFull = value; } }
#pragma warning restore 649
    }
}
