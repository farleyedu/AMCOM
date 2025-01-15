using Banco.Models;
using System;
using System.Data.SQLite;
using System.Globalization;

class Program
{

    static DadosConta DadosIniciais()
    {
        Console.Write("Entre o número da conta: ");
        int numero = int.Parse(Console.ReadLine()); 
         
        DadosConta conta = Banco.Data.Banco_de_Dados.ObterConta(numero);
        if (conta != null)
        { 
            Console.WriteLine("Conta existente encontrada:");
            Console.WriteLine(conta);

            Console.Write("Deseja alterar o nome do titular (s/n)? ");
            char alterarNome = char.Parse(Console.ReadLine());

            if (alterarNome == 's' || alterarNome == 'S')
            {
                Console.Write("Entre o novo nome do titular: ");
                string novoTitular = Console.ReadLine();
                conta.Titular = novoTitular;
                Banco.Data.Banco_de_Dados.AtualizarConta(conta);
            }
        }
        else
        {
            Console.Write("Entre o titular da conta: ");
            string titular = Console.ReadLine();

            Console.Write("Haverá depósito inicial (s/n)? ");
            char resp = char.Parse(Console.ReadLine());

            if (resp == 's' || resp == 'S')
            {
                Console.Write("Entre com o valor de depósito inicial: ");
                double depositoInicial = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                conta = new DadosConta(numero, titular, depositoInicial);
            }
            else
            {
                conta = new DadosConta(numero, titular);
            }

            Banco.Data.Banco_de_Dados.AdicionarConta(conta);
        }

        Console.WriteLine("\nDados da conta:");
        Console.WriteLine(conta);

        Console.Write("\nEntre um valor para depósito: ");
        double quantia = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
        conta.Depositar(quantia);

        Banco.Data.Banco_de_Dados.AtualizarConta(conta);

        Console.WriteLine("\nDados da conta atualizados:");
        Console.WriteLine(conta);

        Console.Write("\nEntre um valor para saque: ");
        quantia = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
        conta.Sacar(quantia);

        Banco.Data.Banco_de_Dados.AtualizarConta(conta);

        Console.WriteLine("\nDados da conta atualizados:");
        Console.WriteLine(conta);
        return conta;
    }

    static void Main(string[] args)
    {
        bool continuar = true;

        while (continuar)
        {
            var dadosCliente = DadosIniciais();

            Console.Write("Procurar outra conta (s/n)? ");
            char resp1 = char.Parse(Console.ReadLine());

            if (resp1 != 's' && resp1 != 'S')
            {
                continuar = false;
                Console.WriteLine("Saindo do programa...");
            }
        }
    }
}

