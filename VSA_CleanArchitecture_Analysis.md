# Kvantitativa mätetal

För att jämföra Clean Architecture (CA) och Vertical Slice Architecture (VSA) implementerades samma funktioner, `CreateOrder` och `DeleteOrder`, med fokus på hur många filer som behövde skapas eller ändras.

## Antal kodfiler och berörda filer

Om med "kod" menas kodfiler/ändringsytor, blir den korta sammanfattningen:

| Funktion | CA: kodfiler | CA: berörda filer |
|---|---:|---:|---:|
| CreateOrder | 27 |
| DeleteOrder | 3-4 |

## Clean Architecture

I CA sprids ansvaren över flera lager. Det ger tydlig separation, men också fler berörda filer per funktion.

| Funktion | Nya filer | Modifierade filer | Totalt |
|---|---:|---:|---:|
| CreateOrder | 19 | 8 | 27 |
| GetOrder | 2 | 1 | 3 |
| DeleteOrder | 2 | 1-2 | 3-4 |
| **Totalt** | **23** | **10-11** | **33-34** |

För `DeleteOrder` innebär CA i praktiken att ändringen hamnar i flera lager:
- Domain: `IOrderRepository.DeleteAsync`
- Application: `DeleteOrderCommand` + handler
- Infrastructure: `OrderRepository.DeleteAsync`
- API: `OrderEndpoints.MapDelete`

Det motsvarar ungefär **6-7 filer fördelat**, även om själva funktionaliteten är relativt liten.

## Vertical Slice Architecture

I VSA samlas samma funktion i en enda slice. För `DeleteOrder` skulle det typiskt vara ungefär:
- `DeleteOrderCommand.cs`
- `DeleteOrderHandler.cs`
- `DeleteOrderDbAccess.cs`
- `DeleteOrderTests.cs`

Det ger ungefär **4 filer samlat** för samma funktionalitet.

## Tolkning

Resultatet visar att CA har högre initial ceremony och fler berörda filer när en funktion spänner över flera lager. VSA ger däremot en tätare och mer lokal kodstruktur per feature. För små, isolerade CRUD-flöden blir skillnaden tydlig: CA prioriterar separation och långsiktig struktur, medan VSA prioriterar snabbhet och låg filspridning.

Samtidigt visar siffrorna att CA:s kostnad per ny operation efter initial setup sjunker kraftigt. När grundstrukturen finns på plats behöver nya operationer ofta bara **2-4 filer**.