using System;
using System.ComponentModel.DataAnnotations;

namespace Grains
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Validates an object instance using a <see cref="Validator"/>.
        /// </summary>
        /// <param name="instance">The object instance to validate.</param>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static void EnsureValid(this object instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            Validator.ValidateObject(instance, new ValidationContext(instance));
        }
    }
}
