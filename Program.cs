using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;

// Assemblies necessários para serialização do Lambda
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ProjectDarkin
{
    public class Function
    {
        // Cria o cliente do DynamoDB
        private static readonly AmazonDynamoDBClient _client = new AmazonDynamoDBClient();

        // Referência para a tabela DynamoDB
        [Obsolete]
        private static readonly Table _tabela = (Table)Table.LoadTable(_client, "LivrariaPedidos");

        /// <summary>
        /// Função principal do AWS Lambda.
        /// É acionada via API Gateway ao receber uma requisição do Twilio (WhatsApp).
        /// </summary>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation("🚀 [INÍCIO] Execução da função Project Darkin.");
            context.Logger.LogInformation($"🕓 Horário (UTC): {DateTime.UtcNow}");
            context.Logger.LogInformation($"🔎 ID da Requisição: {context.AwsRequestId}");

            try
            {
                // Validação básica da requisição
                if (string.IsNullOrEmpty(request.Body))
                {
                    context.Logger.LogInformation("⚠️ Requisição sem corpo recebida.");
                    return ResponderTwiML("Desculpe, não entendi sua mensagem. Pode repetir?");
                }

                // Extrai os parâmetros vindos do Twilio (form-urlencoded)
                var parametros = HttpUtility.ParseQueryString(request.Body);
                string mensagem = parametros["Body"];
                string numero = parametros["From"];

                context.Logger.LogInformation($"📩 Mensagem recebida de {numero}: {mensagem}");

                // Consulta o estado atual do cliente no DynamoDB
                var documento = await _tabela.GetItemAsync(numero);
                string estado = documento?["Estado"] ?? "";

                context.Logger.LogInformation($"📊 Estado atual do cliente: {estado}");

                string resposta = "";

                // ------------------------------
                // 🤖 LÓGICA PRINCIPAL DO ATENDIMENTO
                // ------------------------------

                if (string.IsNullOrEmpty(estado))
                {
                    resposta =
                        "Olá! 👋 Bem-vindo à *Livraria Digital Darkin*!\n\n" +
                        "Aqui você encontra qualidade, confiança e rapidez em cada atendimento. 📚✨\n\n" +
                        "Digite o número da opção desejada:\n\n" +
                        "1️⃣ Ver catálogo de livros\n" +
                        "2️⃣ Fazer um pedido\n" +
                        "3️⃣ Falar com um atendente";
                    await SalvarEstado(numero, "aguardando_opcao");
                    context.Logger.LogInformation("🎉 Saudação inicial enviada.");
                }
                else if (estado == "aguardando_opcao")
                {
                    switch (mensagem)
                    {
                        case "1":
                            resposta = "📚 Nosso catálogo está sendo atualizado!\nDigite o nome do livro que deseja procurar:";
                            await SalvarEstado(numero, "aguardando_livro");
                            break;

                        case "2":
                            resposta = "🛒 Perfeito! Me diga o nome do livro que você deseja comprar:";
                            await SalvarEstado(numero, "aguardando_pedido");
                            break;

                        case "3":
                            resposta = "📞 Um atendente entrará em contato em instantes. Obrigado pela preferência!";
                            await SalvarEstado(numero, "aguardando_atendente");
                            break;

                        default:
                            resposta = "⚠️ Opção inválida! Por favor, digite 1, 2 ou 3.";
                            break;
                    }
                }
                else if (estado == "aguardando_livro")
                {
                    resposta = $"🔍 Procurando o livro *{mensagem}*... Aguarde um momento enquanto verificamos a disponibilidade. 📖";
                    await SalvarEstado(numero, "livro_consultado");
                }
                else if (estado == "aguardando_pedido")
                {
                    resposta = $"✅ Pedido registrado para o livro *{mensagem}*!\nEm breve entraremos em contato para confirmar o pagamento.";
                    await SalvarEstado(numero, "pedido_finalizado");
                }
                else
                {
                    resposta = "📖 Deseja iniciar um novo atendimento? Digite *menu* para começar novamente.";
                    await SalvarEstado(numero, "");
                }

                context.Logger.LogInformation("🏁 [FIM] Execução concluída com sucesso.");
                return ResponderTwiML(resposta);
            }
            catch (Exception ex)
            {
                // Log detalhado do erro (com stack trace)
                context.Logger.LogError($"❌ ERRO INTERNO: {ex}");
                return ResponderTwiML("⚠️ Tivemos um problema interno. Nossa equipe já foi notificada e estamos resolvendo! 🙏");
            }
        }

        // ------------------------------
        // 🔸 FUNÇÕES AUXILIARES
        // ------------------------------

        /// <summary>
        /// Salva o estado atual do usuário no DynamoDB.
        /// </summary>
        private static async Task SalvarEstado(string numero, string estado)
        {
            var item = new Document
            {
                ["PedidoID"] = numero, // Chave primária
                ["Estado"] = estado,
                ["UltimaAtualizacao"] = DateTime.UtcNow.ToString("o")
            };

            await _tabela.PutItemAsync(item);
        }

        /// <summary>
        /// Retorna uma resposta TwiML (XML) para o Twilio.
        /// </summary>
        private static APIGatewayProxyResponse ResponderTwiML(string mensagem)
        {
            var twiml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Response>
    <Message>{mensagem}</Message>
</Response>";

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/xml" } },
                Body = twiml
            };
        }
    }
}
