namespace Wxck.AdminTemplate.Domain.Attributes {

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ExcludeOnUpdateAttribute : Attribute {
    }
}