namespace RegisterUser.Validators
{
    public class MinimumAgeValidator : ValidationAttribute
    {
        int _minimumAge;

        public MinimumAgeValidator(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        public override bool IsValid(object value)
        {
            DateTime date;
            if (DateTime.TryParse(value.ToString(), out date))
            {
                return date.AddYears(_minimumAge) < DateTime.Now;
            }

            return false;
        }
    }
}
