# PMS.Tests Projesi (xUnit)

Bu proje, PMS (Proje Yönetim Sistemi) mikroservis mimarisini test etmek için xUnit test framework'ünü kullanır.

## Proje Yapısı

- **UserApiTests:** UserApi servisine ait unit testler
- **TaskApiTests:** TaskApi servisine ait unit testler
- **ProjectApiTests:** ProjectApi servisine ait unit testler
- **IntegrationTests:** API'ların entegrasyon testleri
- **ModelDiscoveryTests.cs:** Model keşif testleri

## Test Türleri

### 1. Unit Testler

Controller ve mapper sınıflarının davranışlarını izole bir şekilde test etmek için unit testler yazılmıştır. Bu testlerde, harici bağımlılıklar mock nesnelerle değiştirilmiştir.

Örnek:
```csharp
[Fact]
public async Task GetUserById_WithValidId_ReturnsUser()
{
    // Arrange
    var userId = Guid.NewGuid();
    var user = new User { /* ... */ };
    
    _mockMapper.Setup(m => m.GetUserById(userId))
        .ReturnsAsync(user);

    // Act
    var result = await _controller.GetUserById(userId);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var returnValue = Assert.IsType<User>(okResult.Value);
    Assert.Equal(userId, returnValue.Id);
}
```

### 2. Entegrasyon Testler

Gerçek HTTP istekleri yaparak API'ların bütünsel davranışını test etmek için entegrasyon testleri yazılmıştır. Bu testler için `WebApplicationFactory` kullanılmaktadır.

Örnek:
```csharp
[Fact]
public async Task Register_WithValidData_ReturnsCreated()
{
    // Arrange
    var userDto = new DtoUserUI { /* ... */ };

    // Act
    var response = await _client.PostAsJsonAsync("/api/user/register", userDto);

    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
}
```

## Test Çalıştırma

Testleri çalıştırmak için aşağıdaki yöntemlerden birini kullanabilirsiniz:

### Visual Studio'da:
1. Test Explorer'ı açın (Test > Test Explorer)
2. "Run All Tests" düğmesine tıklayın veya belirli testleri seçin

### Komut Satırında:
```
dotnet test PMS.Tests/PMS.Tests.csproj
```

## xUnit Kullanım Prensipleri

- **[Fact]:** Parametre almayan basit testler için kullanılır
- **[Theory]:** Veri odaklı, parametreli testler için kullanılır, genellikle [InlineData] ile birlikte
- **Constructor:** NUnit'teki [SetUp] metodu yerine constructor kullanılır
- **IDisposable.Dispose():** NUnit'teki [TearDown] metodu yerine Dispose metodu kullanılır
- **IClassFixture<T>:** Test sınıfları arasında paylaşılan kaynaklar için kullanılır
- **ITestOutputHelper:** Test çıktılarını kaydetmek için kullanılır

## Test Prensipleri

1. **Arrange-Act-Assert:** Her test, bu üç aşamayı içermelidir
2. **Bir test, bir senaryo:** Her test yalnızca bir davranışı test etmelidir
3. **İzolasyon:** Testler birbirinden ve dış sistemlerden izole olmalıdır
4. **Okunabilirlik:** Test isimleri test edilen davranışı açıkça belirtmelidir (örn. Method_Condition_ExpectedResult)
5. **Hız:** Testler hızlı çalışmalıdır, entegrasyon testlerini minimumda tutun

## Daha Fazla Test Eklemek

Bu proje, temel test yapısını oluşturmak için hazırlanmıştır. Yeni testler eklerken mevcut örnekleri takip edebilirsiniz. Daha fazla test türü için:

- **İntegration Testleri:** Her API için eksiksiz entegrasyon testleri ekleyin
- **Repository Testleri:** Veritabanı işlemlerini test edin (InMemoryDatabase kullanarak)
- **Controller Testleri:** Tüm API uç noktaları için testler ekleyin
- **Servis Testleri:** Servis katmanı sınıfları için testler ekleyin
- **Validation Testleri:** Input doğrulama mantığını test edin
