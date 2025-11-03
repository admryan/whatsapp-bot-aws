# 🤖 Chatbot WhatsApp com AWS Lambda e DynamoDB

[![C#](https://img.shields.io/badge/Code-C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://dotnet.microsoft.com/)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![AWS Lambda](https://img.shields.io/badge/AWS-Lambda-FF9900?style=for-the-badge&logo=awslambda&logoColor=white)](https://aws.amazon.com/lambda/)
[![DynamoDB](https://img.shields.io/badge/AWS-DynamoDB-4053D6?style=for-the-badge&logo=amazondynamodb&logoColor=white)](https://aws.amazon.com/dynamodb/)
[![Twilio](https://img.shields.io/badge/Twilio-WhatsApp-25D366?style=for-the-badge&logo=whatsapp&logoColor=white)](https://www.twilio.com/whatsapp)
[![GitHub](https://img.shields.io/badge/Project-Public-181717?style=for-the-badge&logo=github)](https://github.com/admryan/whatsapp-bot-aws)

---

## 💡 Sobre o Projeto

Este projeto implementa um **chatbot para WhatsApp** desenvolvido em **C#** e hospedado de forma **serverless** na **AWS Lambda**, com armazenamento de dados no **Amazon DynamoDB**.  
O bot gerencia conversas automatizadas, simulando o atendimento de uma **Livraria Digital**, oferecendo opções como consulta de catálogo, pedidos e contato com atendente.

O código foi **gerado com o auxílio de IA (ChatGPT)**, mas **toda a configuração, integração com AWS, criação do DynamoDB e deploy foram feitos manualmente por mim**.

---

## ⚙️ Principais Funcionalidades

- Integração com o **Twilio WhatsApp API**  
- Fluxo de conversas baseado em **estados armazenados no DynamoDB**  
- Mensagens formatadas em **TwiML (XML)** para compatibilidade com o Twilio  
- Deploy **serverless** com **AWS Lambda** e **API Gateway**  
- Estrutura limpa, comentada e facilmente expansível  

---

## 🧠 Conceito de Funcionamento

O chatbot armazena o número do cliente e o **estado atual da conversa** no DynamoDB, permitindo continuar o atendimento de onde o usuário parou.  
Cada interação é processada na Lambda e devolvida ao Twilio em formato XML.

