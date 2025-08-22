using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoTSimulator.Subscriber.Migrations
{
    /// <inheritdoc />
    public partial class AddSensorDataUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SensorData",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SensorData",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Rooms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Rooms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "IoTDevices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "IoTDevices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Houses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Houses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_SensorData_CreatedAt",
                table: "SensorData",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SensorData_UpdatedAt",
                table: "SensorData",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Room_CreatedAt",
                table: "Rooms",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Room_UpdatedAt",
                table: "Rooms",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IoTDevice_CreatedAt",
                table: "IoTDevices",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IoTDevice_UpdatedAt",
                table: "IoTDevices",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_House_CreatedAt",
                table: "Houses",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_House_UpdatedAt",
                table: "Houses",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SensorData_CreatedAt",
                table: "SensorData");

            migrationBuilder.DropIndex(
                name: "IX_SensorData_UpdatedAt",
                table: "SensorData");

            migrationBuilder.DropIndex(
                name: "IX_Room_CreatedAt",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Room_UpdatedAt",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_IoTDevice_CreatedAt",
                table: "IoTDevices");

            migrationBuilder.DropIndex(
                name: "IX_IoTDevice_UpdatedAt",
                table: "IoTDevices");

            migrationBuilder.DropIndex(
                name: "IX_House_CreatedAt",
                table: "Houses");

            migrationBuilder.DropIndex(
                name: "IX_House_UpdatedAt",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SensorData");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SensorData",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Rooms",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Rooms",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "IoTDevices",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "IoTDevices",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Houses",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Houses",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");
        }
    }
}
