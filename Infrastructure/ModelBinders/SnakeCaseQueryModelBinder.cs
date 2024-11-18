using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DotNetService.Infrastructure.ModelBinder
{
    public partial class SnakeCaseQueryModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            var query = bindingContext.HttpContext.Request.Query;
            var model = Activator.CreateInstance(bindingContext.ModelType); // Create an instance of the DTO

            // Get all properties of the DTO
            var properties = bindingContext.ModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // Convert property name to snake_case
                var snakeCaseName = ToSnakeCase(property.Name);

                // Check if the query contains the expected snake_case name
                if (query.TryGetValue(snakeCaseName, out var value))
                {
                    var propertyType = property.PropertyType;

                    // Check if the property is an enum or a nullable enum
                    if (propertyType.IsEnum || Nullable.GetUnderlyingType(propertyType)?.IsEnum == true)
                    {
                        var enumType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

                        try
                        {
                            var enumValue = Enum.Parse(enumType, value.First(), true); // case-insensitive
                            property.SetValue(model, enumValue);
                        }
                        catch (ArgumentException)
                        {
                            bindingContext.ModelState.AddModelError(property.Name, $"Invalid value for enum {enumType.Name}");
                        }
                    }
                    // Handle Guid and nullable Guid types
                    else if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                    {
                        if (Guid.TryParse(value.First(), out var guidValue))
                        {
                            property.SetValue(model, guidValue);
                        }
                        else
                        {
                            bindingContext.ModelState.AddModelError(property.Name, "Invalid Guid format");
                        }
                    }
                    // Handle boolean and nullable boolean types
                    else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                    {
                        var firstValue = value.First().Trim();

                        if (bool.TryParse(firstValue, out var boolValue) ||
                            firstValue == "1" ||
                            firstValue.Equals("true", StringComparison.OrdinalIgnoreCase))
                        {
                            property.SetValue(model, true);
                        }
                        else if (firstValue == "0" ||
                                 firstValue.Equals("false", StringComparison.OrdinalIgnoreCase))
                        {
                            property.SetValue(model, false);
                        }
                        else
                        {
                            bindingContext.ModelState.AddModelError(property.Name, "Invalid boolean format");
                        }
                    }

                    else
                    {
                        // Convert the value and set the property value in the DTO instance
                        var convertedValue = Convert.ChangeType(value.First(), property.PropertyType);
                        property.SetValue(model, convertedValue);
                    }
                }
            }

            // Set the binding result
            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }

        private static string ToSnakeCase(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            // Convert to snake_case (from PascalCase or camelCase)
            var regex = MyRegex();
            return regex.Replace(str, "$1_$2").ToLower();
        }

        [GeneratedRegex("([a-z])([A-Z])")]
        private static partial Regex MyRegex();
    }

    public class SnakeCaseQueryModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            ArgumentNullException.ThrowIfNull(nameof(context));

            // Apply the SnakeCaseQueryModelBinder to all query parameters
            if (context.BindingInfo.BindingSource == BindingSource.Query)
            {
                return new SnakeCaseQueryModelBinder();
            }

            return null;
        }
    }
}
