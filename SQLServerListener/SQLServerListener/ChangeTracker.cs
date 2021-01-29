using System;

namespace SQLServerListener
{
    public class ChangeTracker
    {
        public static void TraceChangeData<T>(T newVal, T oldVal) where T : class
        {
            var oType = oldVal.GetType();

            foreach (var oProperty in oType.GetProperties())
            {
                var oOldValue = oProperty.GetValue(oldVal, null);
                var oNewValue = oProperty.GetValue(newVal, null);
                if (!object.Equals(oOldValue, oNewValue))
                {
                    var sOldValue = oOldValue == null ? "null" : oOldValue.ToString();
                    var sNewValue = oNewValue == null ? "null" : oNewValue.ToString();

                    Console.WriteLine("{0} was {1}; is: {2}", oProperty.Name, sOldValue, sNewValue);
                }
            }
        }
    }
}