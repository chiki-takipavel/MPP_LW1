using System;
using System.Globalization;
using System.Xml.Serialization;

namespace LR1_MPP
{
    public class Price : IComparable<Price>
    {
        [XmlIgnore]
        public CultureInfo Culture { get; set; }

        public decimal Value { get; set; }

        public string CultureName
        {
            get => Culture.Name;
            set => Culture = new CultureInfo(value);
        }

        public Price()
        {
            Culture = null;
            Value = 0;
        }

        public Price(decimal value, CultureInfo culture)
        {
            Culture = culture;
            Value = value;
        }

        public int CompareTo(Price other)
        {
            var result = CompareByValue(this, other);
            if (result == 0) 
                result = CompareByCulture(this, other);
            return result;
        }

        public override string ToString()
        {
            return Value.ToString(new CultureInfo("fr")) +
                   (Culture != null ? " " + Culture.NumberFormat.CurrencySymbol : "");
        }

        private static int CompareByValue(Price priceX, Price priceY)
        {
            if (priceX.Value > priceY.Value)
                return 1;
            if (priceX.Value < priceY.Value)
                return -1;
            return 0;
        }

        private static int CompareByCulture(Price priceX, Price priceY)
        {
            if (priceX.Culture != null && priceY.Culture != null)
                return string.CompareOrdinal(priceX.Culture.Name, priceY.Culture.Name);
            if (priceX.Culture != null && priceY.Culture == null)
                return 1;
            return -1;
        }
    }
}
