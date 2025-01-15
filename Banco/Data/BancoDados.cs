using Banco.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Data
{
    public static class Banco_de_Dados
    {
        private static string connectionString = "Data Source=contas.db;Version=3;";

        static void BancoDeDados()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "CREATE TABLE IF NOT EXISTS Contas (Numero INTEGER PRIMARY KEY, Titular TEXT, Saldo REAL)";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void AdicionarConta(DadosConta conta)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Contas (Numero, Titular, Saldo) VALUES (@Numero, @Titular, @Saldo)";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Numero", conta.Numero);
                    command.Parameters.AddWithValue("@Titular", conta.Titular);
                    command.Parameters.AddWithValue("@Saldo", conta.Saldo);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static DadosConta ObterConta(int numero)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Contas WHERE Numero = @Numero";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Numero", numero);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string titular = reader["Titular"].ToString();
                            double saldo = Convert.ToDouble(reader["Saldo"]);
                            return new DadosConta(numero, titular, saldo);
                        }
                    }
                }
            }
            return null;
        }

        public static void AtualizarConta(DadosConta conta)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE Contas SET Titular = @Titular, Saldo = @Saldo WHERE Numero = @Numero";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Numero", conta.Numero);
                    command.Parameters.AddWithValue("@Titular", conta.Titular);
                    command.Parameters.AddWithValue("@Saldo", conta.Saldo);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
