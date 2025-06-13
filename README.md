# Device Manager (.NET, Blazor, MAUI, MongoDB)

Este repositório contém a implementação de uma solução completa para gerenciamento de dispositivos, desenvolvida como parte de um desafio técnico para Desenvolvedor Pleno. A solução contempla três projetos interligados: uma API REST com ASP.NET Core e MongoDB, um aplicativo mobile com .NET MAUI e RealmDB (suporte offline), e uma aplicação web Blazor WebAssembly consumindo a API.

## 🔧 Estrutura da Solution

### 📌 Projeto 1: API REST (`DeviceManager.API`)
- **Tecnologia**: ASP.NET Core  
- **Banco de Dados**: MongoDB  
- **Recursos**:
  - CRUD completo para a entidade `Dispositivo`
  - Sincronização de dados recebidos do app mobile (inserções, atualizações e exclusões)
  - Validação de unicidade do campo `CodigoReferencia`

### 📱 Projeto 2: App Mobile (`DeviceManager.App`)
- **Tecnologia**: .NET MAUI (alternativa: Xamarin Forms)  
- **Banco de Dados Local**: RealmDB  
- **Funcionalidades**:
  - CRUD local com persistência offline
  - Indicação de itens pendentes de sincronização
  - Tela dedicada para sincronizar dados com a API
  - Logs básicos de sincronização
  - Utilização do padrão MVVM

### 💻 Projeto 3: Web App (`DeviceManager.Web`)
- **Tecnologia**: Blazor WebAssembly  
- **Funcionalidades**:
  - Interface web interativa para CRUD de dispositivos
  - Validações nos campos obrigatórios
  - Validação de unicidade para o `CodigoReferencia`
  - Componentização e uso de `HttpClient` com injeção de dependência
  - Totalmente conectado à API via chamadas assíncronas

## 🧪 Modelo da Entidade

```csharp
public class Dispositivo
{
    public string Id { get; set; } // Primary key
    public string Descricao { get; set; }
    public string CodigoReferencia { get; set; } // Unique
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public bool IsDeleted { get; set; } // para controle lógico de exclusão
}
