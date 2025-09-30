namespace Plataforma.Helpers
{
    public class HttpErrorMessages
    {
        public const string NO_RECORDS_FOUND = "No record founds";
        public const string INVALID_LOGIN_INFO = "Incorrect user or password";
        public const string INVALID_OR_NULL_REFERENCE = "Invalid or null reference";
        public const string CONFIG_NOT_FOUND_OR_INVALID = "Configuration is not valid or is empty";
        public const string MISSING_VALUES = "One or more missing values";
        public const string DUPLICATED_PRIMARY_KEY = "Conflict when try to insert primary key";
        public const string ACTION_NOT_ALLOWED = "Conflict when try to insert primary key";
        public const string INTERNAL_ERROR = "Internal unidentified error";
        public const string FOREING_KEY_NOT_FOUND = "Foreing key is wrong or null";
        public const string INVALID_FILE_TYPE = "Invalid file type";
        public const string USER_WITHOUT_MAIL_OR_PHONE = "This user dont have associated mail or phone";
        public const string OUT_OF_REGISTER_AREA = "Out of the register area";
        public const string INVALID_DATE = "La fecha no tiene el formato correcto (yyyy-MM-dd).";
    }
}
