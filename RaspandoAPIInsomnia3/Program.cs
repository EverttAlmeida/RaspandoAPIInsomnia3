using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;

namespace RaspandoAPIInsomnia3
{
    //Classe de contexto do banco de dados
    public class LogContexto : DbContext
    {
        public DbSet<Log> Logs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = PC03LAB2519\\SENAI;Database= WebScrapingDb2;User Id=sa; Password=senai.123;");
        }
    }
    public class Log
    {
        [Key]
        public int IdLog { get; set; }
        public string CodRob { get; set; }
        public string UsuRob { get; set; }
        public DateTime DateLog { get; set; }
        public string Processo { get; set; }
        public string InfLog { get; set; }
        public int IdProd { get; set; }
    }

    class Program
    {
        //Lista para armazenar os produtos já verificados
        static List<Produto> produtosVerificados = new List<Produto>();
        static void Main(string[] args)
        {
            //Definir o intervalo de tempo para 5 minutos (300.000 milisegundos)
            int intervalo = 300000;

            //Criar um temporisador que dispara a cada 5 minutos
            Timer timer = new Timer(VerificarNovoProduto, null, 0, intervalo);

            //Manter a aplicação rodando
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadLine();


        }
        static async void VerificarNovoProduto(Object state)
        {
            string username = "11164448";
            string senha = "60-dayfreetrial";
            string url = "http://regymatrix-001-site1.ktempurl.com/api/v1/produto/getall";

            try
            {
                //Criar um objeto HttpClient
                using (HttpClient client = new HttpClient())
                {
                    //Adicionar as credenciais de autentificação básica
                    var byteArray = Encoding.ASCII.GetBytes($"{username}:{senha}");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    //Fazer a requsição GET à API
                    HttpResponseMessage response = await client.GetAsync(url);

                    //Verificar sea requisição foi bem sucedida (Códico de status 200)
                    if (response.IsSuccessStatusCode)
                    {
                        //Ler o conteúdo da respoda com uma string
                        string responseData = await response.Content.ReadAsStringAsync();

                        //Processar os dados da resposta
                        List<Produto> novosProdutos = ObterNovosProdutos(responseData);
                        foreach (Produto produto in novosProdutos)
                        {
                            if (!produtosVerificados.Exists(p => p.Id == produto.Id))
                            {
                                //Se é um novo Produto, faça algo com ele
                                Console.WriteLine($"Novo produto encontrado: ID {produto.Id}, Nome: {produto.Nome}");

                                //Adicionar o produto à lista de produtos verificados
                                produtosVerificados.Add(produto);

                                //Registrar um log no banco de dados
                                RegistrarLog("0001", "foxmatrix", DateTime.Now, "API CONSULTA - InfomacaoLog", "Sucesso", produto.Id);


                            }
                        }
                    }
                    else
                    {
                        //Imprimir mensagem de erro caso falhe a requisição falhe
                        Console.WriteLine($"Erro: {response.StatusCode}");

                    }
                }
            }
            catch (Exception ex)
            {
                //Imprimir mensagem de erro caso ocorra uma exceção
                Console.WriteLine($"Erro ao fazer a requisição: {ex.Message}");

            }

            //Método para processar os dados da resposta e obter produtos
            static List<Produto> ObterNovosProdutos(string responseData)
            {
                //Desserialixar os dados da resposta para uma lista de produtos
                List<Produto> produtos = JsonConvert.DeserializeObject<List<Produto>>(responseData);
                return produtos;
            }

        }
        static void RegistrarLog(string codRob, string usuRob, DateTime dateLog, string processo, string infLog, int idProd)
        {
            using (var context = new LogContexto())
            {
                var log = new Log
                {
                    CodRob = codRob,
                    UsuRob = usuRob,
                    DateLog = dateLog,
                    Processo = processo,
                    InfLog = infLog,
                    IdLog = idProd
                };
                context.Logs.Add(log);
                context.SaveChanges();
            }
        }

        //Classe para representar um produto
        public class Produto
        {
            public int Id { get; set; }
            public string Nome { get; set; }
        }
    }
}

