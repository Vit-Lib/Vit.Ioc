namespace Demo.Controllers
{
    public interface IUser
    {
        string GetInfo();
    }
    public class UserA : IUser
    {
        public string GetInfo()
        {
            return "UserA";
        }
    }

    public class UserB : IUser
    {
        public string fieldValue;
        public string propertyValue { get; set; } = "default";
        public int age = 0;
        public void SetAge(int age)
        {
            this.age = age;
        }
        public string GetInfo()
        {
            return $"UserB fieldValue[{ fieldValue }]  propertyValue[{propertyValue}]  age[{age}]";
        }
    }
}
