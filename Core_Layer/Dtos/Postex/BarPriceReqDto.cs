using System.Text.Json.Serialization;

namespace Busines_Layer.Dtos.Postex;

public class GetShippingQuotesQueryDto
{
    [JsonPropertyName("from_city_code")]
    public int FromCityCode { get; set; }

    [JsonPropertyName("collection_type")]
    public string CollectionType { get; set; } = null!;

    public Courier Courier { get; set; }
    [JsonPropertyName("value_added_service")]
    public OptionalServices ValueAddedServices { get; set; }

    [JsonPropertyName("parcels")]
    public List<GetShippingQuotesQueryParcels> Parcels { get; set; } = new();
}

public class Courier
{
    [JsonPropertyName("courier_code")]
    public string? CourierCode { get; set; }
    [JsonPropertyName("service_type")]
    public string? ServiceType { get; set; }
}

public class GetShippingQuotesQueryParcels
{
    [JsonPropertyName("to_city_code")]
    public int ToCityCode { get; set; }

    [JsonPropertyName("payment_type")]
    public string? PaymentType { get; set; }

    [JsonPropertyName("parcel_properties")]
    public ParcelPropertyDto ParcelProperties { get; set; } = new();

    
}
public class OptionalServices
{
    public bool RequestLabel { get; set; } = false;

    public bool RequestPackaging { get; set; }= false;

    public bool RequestSmsNotification { get; set; } = false;
}

public class ParcelPropertyDto
{
    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("total_weight")]
    public long TotalWeight { get; set; }

    [JsonPropertyName("box_type_id")]
    public int BoxTypeId { get; set; }

    [JsonPropertyName("total_value")]
    public double TotalValue { get; set; }
}

public class ShippingQuotesResponse
{
    public List<ShippingPrice> ShippingPrices { get; set; } = new();
}

public class ShippingPrice
{
    public List<ServicePrice> ServicePrice { get; set; } = new();
}

public class ServicePrice
{
    public double TotalPrice { get; set; }
}