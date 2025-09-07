## Sobre o Projeto

Este projeto simula o funcionamento de um micro-ondas digital com interface web, permitindo operações básicas de aquecimento, programas pré-definidos, cadastro de programas customizados e integração com Web API.

**Níveis Atendidos: 4/4 - Implementação Completa**

## Funcionalidades Principais

### Nível 1 - Operações Básicas
- Interface para configuração de tempo (1 segundo a 2 minutos) e potência (1-10)
- Função "Início Rápido" (30 segundos na potência 10)
- Acréscimo de 30 segundos durante aquecimento
- Animação visual do processo de aquecimento com caracteres dinâmicos
- Sistema de pausa/cancelamento com controle de estados
- Validações completas de entrada

### Nível 2 - Programas Pré-definidos
- **5 Programas Inclusos:**
  - Pipoca (3min, potência 7)
  - Leite (5min, potência 5)  
  - Carne de Boi (14min, potência 4)
  - Frango (8min, potência 7)
  - Feijão (8min, potência 9)
- Caracteres de aquecimento únicos para cada programa
- Instruções específicas para cada alimento
- Programas protegidos contra alteração/exclusão

### Nível 3 - Programas Customizados
- Cadastro de programas personalizados
- Persistência em arquivo JSON
- Caracteres de aquecimento personalizados (validação de unicidade)
- Interface diferenciada (fonte em itálico)
- Instruções opcionais

### Nível 4 - Web API
- **Endpoints RESTful** para todas as operações
- **Autenticação Bearer Token**
- Criptografia SHA-256 para senhas
- Suporte a SQL Server com connection string criptografada
- Tratamento robusto de exceções
- Interface de configuração de credenciais

### Padrões Implementados
- **Repository Pattern** - Abstração da camada de dados
- **Service Pattern** - Encapsulamento da lógica de negócio  
- **Factory Pattern** - Criação de programas de aquecimento
- **State Pattern** - Controle de estados do micro-ondas
- **Dependency Injection** - Inversão de dependência

## Tecnologias Utilizadas

- **.NET 9**
- **ASP.NET MVC**
- **ASP.NET Web API**
- **JSON.NET** (Serialização)

## Requisitos do Sistema

- .NET Framework 4.0 ou superior
- Navegador web moderno
- Visual Studio 2017+ ou VS Code

## Instalação e Execução

### 1. Clone o repositório
```bash
git clone https://github.com/debtil/WebMicroondas.git
cd webmicroondas
```

### 2. Restaure os pacotes NuGet
```bash
nuget restore
```

### 3. Execute o projeto
```bash  
dotnet run
```

## Testes

O projeto inclui uma suíte completa de testes unitários cobrindo:

- Validações de entrada (tempo e potência)
- Lógica de aquecimento e estados
- Programas pré-definidos e customizados
- Operações de pausa/cancelamento
- Persistência de dados
- Endpoints da API

```bash
# Executar todos os testes
dotnet test
```

## Configuração da API

### Autenticação
1. Acesse a seção "Configurações" na interface
2. Insira suas credenciais de API
3. O sistema validará automaticamente a conexão

### Credenciais Padrão
```json
{
  "usuario": "admin",
  "senha": "microwave123"
}
```

## Autor

**Matheus Debtil Souza**
- Email: matheus.debtilsouza@gmail.com

---