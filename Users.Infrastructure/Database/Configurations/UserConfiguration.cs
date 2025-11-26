using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Entities;
using Users.Domain.ValueConverter;

namespace Users.Infrastructure.Database.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.UserId);
            builder.Property(u => u.UserId).HasConversion(userId => userId.Value, value => UserId.Create(value));
            builder.Property(u => u.UserName).HasConversion(userName => userName.Value, value => UserName.Create(value)).IsRequired().HasMaxLength(50);
            builder.Property(u => u.UserLastName).HasConversion(userLastName => userLastName.Value, value => UserLastName.Create(value)).IsRequired().HasMaxLength(50);
            builder.Property(u => u.UserEmail).HasConversion(userEmail => userEmail.Value, value => UserEmail.Create(value)).IsRequired().HasMaxLength(50);
            builder.Property(u => u.UserPhoneNumber).HasConversion(userPhoneNumber => userPhoneNumber.Value, value => UserPhoneNumber.Create(value)).IsRequired().HasMaxLength(25);
            builder.Property(u => u.UserDirection).HasConversion(userDirection => userDirection.Value, value => UserDirection.Create(value)).IsRequired().HasMaxLength(200);
            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.UserType).HasConversion<string>().IsRequired();

            builder.ToTable( tb =>{
                tb.HasCheckConstraint(
                    "Check_UserType",
                    "\"UserType\" IN ('Administrador', 'Usuario', 'Organizador', 'Soporte')");
            });
        }
    }
}