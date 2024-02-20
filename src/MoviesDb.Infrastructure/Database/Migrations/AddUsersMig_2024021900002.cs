using FluentMigrator;

namespace MoviesDb.Infrastructure.Database.Migrations;

[Migration(2024021900002)]
public class AddUsersMig_2024021900002 : Migration
{
    public override void Up()
    {
        Create.Table("Users")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Email").AsString(350).NotNullable()
            .WithColumn("PasswordHash").AsString(350).NotNullable()
            .WithColumn("IsAdmin").AsBoolean().NotNullable()
            .WithColumn("CreatedAt")
                .AsDateTimeOffset()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTimeOffset)
            .WithColumn("FirstName").AsString(150).NotNullable()
            .WithColumn("LastName").AsString(150).NotNullable();

        Create.ForeignKey("FK_Movies_Users")
            .FromTable("Movies").ForeignColumn("CreatedBy")
            .ToTable("Users").PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);
    }

    public override void Down()
    {
        Delete.ForeignKey("FK_Movies_Users");
        Delete.Table("Users");        
    }
}
