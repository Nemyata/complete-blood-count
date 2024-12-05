using FluentMigrator;

namespace BloodCount.Migrator.Migrations;

[Migration(202412032134, "Создание таблиц с результатами")]
public class Migration_202412032134 : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("LLM").InSchema("dbo")
            .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("FileId").AsGuid().NotNullable()
            .WithColumn("Gender").AsString(20).NotNullable()
            .WithColumn("Hemoglobin").AsInt32().NotNullable()
            .WithColumn("RedBloodCells").AsInt32().NotNullable()
            .WithColumn("Platelets").AsInt32().NotNullable()
            .WithColumn("Leukocytes").AsInt32().NotNullable()
            .WithColumn("ResultML").AsString(int.MaxValue).NotNullable();

        Create.Table("Files").InSchema("dbo")
            .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .ReferencedBy("FK_Files_LLM_FilesId", "dbo", "LLM", "FileId")
            .WithColumn("FileName").AsString(int.MaxValue).NotNullable()
            .WithColumn("OCR").AsString(int.MaxValue).Nullable();
    }
}