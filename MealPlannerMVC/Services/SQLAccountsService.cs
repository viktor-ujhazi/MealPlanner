using MealPlannerMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlannerMVC.Services
{
    public class SQLAccountsService : IAccountsService
    {
        private readonly IDbConnection _connection;

        public SQLAccountsService(IDbConnection connection)
        {
            _connection = connection;
        }
        private static Account ToAccount(IDataReader reader)
        {
            return new Account
            {
                UserID = (int)reader["user_id"],
                Username = (string)reader["username"],
                Email = (string)reader["email"],
                Role = (string)reader["user_role"]
            };
        }
        public void Register(RegisterModel register)
        {
            using var command = _connection.CreateCommand();

            var usernameParam = command.CreateParameter();
            usernameParam.ParameterName = "username";
            usernameParam.Value = register.Username;

            var emailParam = command.CreateParameter();
            emailParam.ParameterName = "email";
            emailParam.Value = register.Email;

            var passwordParam = command.CreateParameter();
            passwordParam.ParameterName = "password";
            passwordParam.Value = register.Password;

            command.CommandText = @"INSERT INTO users (username, password, email) VALUES (@username, @password, @email)";
            command.Parameters.Add(usernameParam);
            command.Parameters.Add(passwordParam);
            command.Parameters.Add(emailParam);

            command.ExecuteNonQuery();
        }

        public Account Login(string email, string password)
        {
            using var command = _connection.CreateCommand();

            var emailParam = command.CreateParameter();
            emailParam.ParameterName = "email";
            emailParam.Value = email;

            var passwordParam = command.CreateParameter();
            passwordParam.ParameterName = "password";
            passwordParam.Value = password;

            command.CommandText = @"SELECT * FROM users WHERE email = @email AND password = @password";
            command.Parameters.Add(emailParam);
            command.Parameters.Add(passwordParam);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return ToAccount(reader);
            }
            return null;
        }
        public int GetUserId(string email)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = $"SELECT user_id FROM users WHERE email = {email}";

            using var reader = command.ExecuteReader();

            reader.Read();
            int userId = Convert.ToInt32(reader["user_id"]);
            return userId;
        }
    }
}
