using System;

namespace Adventure_Game_2._0
{
    internal class Fraction
    {
        public int numerator, denominator;
        private int GCD(int a, int b) => b == 0 ? a : GCD(b, a % b);
        public Fraction(int numerator, int denominator)
        {
            if (denominator == 0)
                throw new DivideByZeroException();
            if (denominator < 0)
            {
                numerator = -numerator;
                denominator = -denominator;
            }
            int gcd = GCD(Math.Abs(numerator), Math.Abs(denominator));
            numerator /= gcd;
            denominator /= gcd;
            this.numerator = numerator;
            this.denominator = denominator;
        }
        public static Fraction operator +(Fraction a, Fraction b) =>
            new Fraction(a.numerator * b.denominator + b.numerator * a.denominator, a.denominator * b.denominator);

        public static Fraction operator -(Fraction a, Fraction b) =>
            new Fraction(a.numerator * b.denominator - b.numerator * a.denominator, a.denominator * b.denominator);

        public static Fraction operator *(Fraction a, Fraction b) =>
            new Fraction(a.numerator * b.numerator, a.denominator * b.denominator);

        public static Fraction operator /(Fraction a, Fraction b) =>
            new Fraction(a.numerator * b.denominator, a.denominator * b.numerator);
        public string outputStrFrac() => denominator == 1 ? numerator.ToString() : $"{numerator}/{denominator}";
        public string outputStrMixFrac()
        {
            if (denominator == 1)
                return numerator.ToString();
            else if (Math.Abs(numerator) < denominator)
                return $"{numerator}/{denominator}";
            else
            {
                int wholePart = numerator / denominator;
                int newNumerator = Math.Abs(numerator) % denominator;
                return newNumerator == 0 ? wholePart.ToString() : $"{wholePart} {newNumerator}/{denominator}";
            }
        }
    }
}

