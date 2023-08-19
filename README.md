# Web Service Crasvia

O **Crasvia** é um projeto de serviço web que oferece funcionalidades para lidar com o envio de imagens e a sincronização de dados. Este README fornece uma visão geral dos principais componentes e métodos dentro do projeto.

## Índice

- [Introdução](#introdução)
- [Métodos](#métodos)
  - [EnviarFoto](#enviarfoto)
  - [Base64ParaImagem](#base64paraimagem)
  - [SincronizaDados](#sincronizadados)

## Introdução

O projeto **Crasvia** inclui uma classe de serviço web chamada `Service` que permite aos usuários fazer upload de imagens e sincronizar dados. O serviço é construído usando tecnologias ASP.NET e oferece vários métodos para realizar essas tarefas.

## Métodos

### EnviarFoto

```csharp
[WebMethod]
public string EnviarFoto(string FotosString, string NomeFotos, string nomePasta)
```

Este método permite que os usuários façam o upload de uma foto na forma de uma string codificada em base64. A imagem enviada é então salva em uma pasta especificada com o nome fornecido. Ele recebe como parâmetros a string da imagem codificada em base64, o nome desejado do arquivo e o nome da pasta.

### Base64ParaImagem

```csharp
public Image Base64ParaImagem(string base64String)
```

Este método converte uma string de imagem codificada em base64 em um objeto `System.Drawing.Image`. Ele é usado internamente no método `EnviarFoto` para converter a string base64 recebida de volta para uma imagem.

### SincronizaDados

```csharp
[WebMethod]
public string SincronizaDados(string dados)
```

O método `SincronizaDados` é usado para sincronização de dados. Ele recebe uma string contendo dados em um formato específico. O método processa esses dados e executa comandos SQL para sincronizar os dados recebidos com um banco de dados. Ele retorna "OK" após uma sincronização bem-sucedida ou uma string vazia em caso de erros.

## Configuração

O projeto utiliza uma string de conexão chamada "webConnectionString1" para se conectar ao banco de dados. Certifique-se de fornecer os detalhes apropriados da conexão do banco de dados na configuração.

## Uso

Para usar o serviço web **Crasvia**, você pode implantá-lo em um servidor web que suporte aplicativos ASP.NET. Depois de implantado, você pode chamar os métodos expostos por meio de requisições HTTP para realizar o envio de imagens e tarefas de sincronização de dados.

Lembre-se de lidar com a segurança e os casos de erro de maneira apropriada, com base no ambiente de implantação e no caso de uso.

## Aviso

Este README fornece uma visão geral básica do projeto **Crasvia** e de seus métodos. Para obter detalhes de implementação detalhados, tratamento de erros, considerações de segurança e diretrizes de implantação, são necessárias documentação adicional e testes.

Para dúvidas ou assistência, não hesite em entrar em contato com os mantenedores do projeto.
