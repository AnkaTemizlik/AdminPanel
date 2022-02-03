using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Domain.Models {
    public static class Validator {

        public static bool IsTaxNumber(string val, bool acceptIfAll1 = false, bool acceptIfAll2 = false) {
            return IsVKN(val, acceptIfAll1, acceptIfAll2) || IsTCKN(val, acceptIfAll1, acceptIfAll2);
        }

        public static bool IsVKN(string val, bool acceptIfAll1 = false, bool acceptIfAll2 = false) {
            int l = 10;

            if (string.IsNullOrWhiteSpace(val))
                return false;

            if (val.Length != l)
                return false;

            var vkn = val.ToCharArray();
            if (!vkn.All(n => char.IsNumber(n)))
                return false;

            if (acceptIfAll1)
                if (val == new string('1', l))
                    return true;

            if (acceptIfAll2)
                if (val == new string('2', l))
                    return true;

            var lastDigit = Convert.ToInt32(vkn[9].ToString());
            int total = 0;
            for (int i = 9; i >= 1; i--) {
                int digit = Convert.ToInt32(vkn[9 - i].ToString());
                var v1 = ((digit + i) % 10);
                int v11 = (int)(v1 * Math.Pow(2, i)) % 9;
                if (v1 != 0 && v11 == 0)
                    v11 = 9;
                total += v11;
            }

            total = (total % 10 == 0) ? 0 : (10 - (total % 10));
            return (total == lastDigit);
        }

        public static bool IsTCKN(string val, bool acceptIfAll1 = false, bool acceptIfAll2 = false) {
            int l = 11;

            if (val == null)
                return false;
            
            if (string.IsNullOrWhiteSpace(val))
                return false;

            if (val.Length != l)
                return false;

            if (acceptIfAll1)
                if (val == new string('1', l))
                    return true;     
            
            if (acceptIfAll2)
                if (val == new string('2', l))
                    return true;

            var ok = long.TryParse(val, out long valLong);

            if (!ok)
                return false;

            long a, b;
            long d1, d2, d3, d4, d5, d6, d7, d8, d9, q1, q2;

            a = valLong / 100;
            b = valLong / 100;

            d1 = a % 10;
            a /= 10;
            d2 = a % 10;
            a /= 10;
            d3 = a % 10;
            a /= 10;
            d4 = a % 10;
            a /= 10;
            d5 = a % 10;
            a /= 10;
            d6 = a % 10;
            a /= 10;
            d7 = a % 10;
            a /= 10;
            d8 = a % 10;
            a /= 10;
            d9 = a % 10;

            q1 = (10 - ((((d1 + d3 + d5 + d7 + d9) * 3) + (d2 + d4 + d6 + d8)) % 10)) % 10;
            q2 = (10 - (((((d2 + d4 + d6 + d8) + q1) * 3) + (d1 + d3 + d5 + d7 + d9)) % 10)) % 10;

            return ((b * 100) + (q1 * 10) + q2 == valLong);
        }
    }
}
