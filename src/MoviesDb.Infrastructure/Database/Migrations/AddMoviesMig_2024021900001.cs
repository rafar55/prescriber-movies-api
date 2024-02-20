using FluentMigrator;
using FluentMigrator.SqlServer;

namespace MoviesDb.Infrastructure.Database.Migrations;

[Migration(2024021900001)]
public class AddMoviesMig_2024021900001 : Migration
{
    public override void Up()
    {
        Create.Table("Movies")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Slug").AsString(350).NotNullable()
            .WithColumn("Title").AsString(250).NotNullable()
            .WithColumn("YearOfRelease").AsInt32().NotNullable()
            .WithColumn("CreatedAt")
                .AsDateTimeOffset()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTimeOffset)
            .WithColumn("CreatedBy").AsGuid().NotNullable();

        Create.UniqueConstraint("UC_Movies_Slug")
            .OnTable("Movies")
            .Column("Slug");

        Create.Table("Genres")
            .WithColumn("Id").AsInt32().Identity(1,1).PrimaryKey()
            .WithColumn("MovieId").AsGuid().NotNullable()
            .WithColumn("Name").AsString(250).NotNullable();
           
       Create.ForeignKey("FK_Genres_Movies")
            .FromTable("Genres").ForeignColumn("MovieId")
            .ToTable("Movies").PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);
    }

    public override void Down()
    {
        Delete.Table("Genres");
        Delete.Table("Movies");       
    }
}
