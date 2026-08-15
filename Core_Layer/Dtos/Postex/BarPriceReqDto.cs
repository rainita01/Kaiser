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
public class ShippingQuotesResponseDto
{
    [JsonPropertyName("parcel_count")]
    public int ParcelCount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;

    [JsonPropertyName("pickup_price")]
    public double PickupPrice { get; set; }

    [JsonPropertyName("shipping_price")]
    public double ShippingPrice { get; set; }

    [JsonPropertyName("shipping_price_vat")]
    public double ShippingPriceVat { get; set; }

    [JsonPropertyName("value_added_service_price")]
    public double ValueAddedServicePrice { get; set; }

    [JsonPropertyName("total_cost")]
    public double TotalCost { get; set; }

    [JsonPropertyName("shipping_prices")]
    public List<ShippingPriceDto> ShippingPrices { get; set; } = new();

    [JsonPropertyName("excluded_services")]
    public Dictionary<string, object> ExcludedServices { get; set; } = new();

    [JsonPropertyName("shipping_price_discount")]
    public double ShippingPriceDiscount { get; set; }
}

public class ShippingPriceDto
{
    [JsonPropertyName("to_city_name")]
    public string ToCityName { get; set; } = null!;

    [JsonPropertyName("shipping_price")]
    public double ShippingPrice { get; set; }

    [JsonPropertyName("shipping_price_vat")]
    public double ShippingPriceVat { get; set; }

    [JsonPropertyName("total_shipping_price")]
    public double TotalShippingPrice { get; set; }

    [JsonPropertyName("estimated_delivery")]
    public string EstimatedDelivery { get; set; } = null!;

    [JsonPropertyName("service_price")]
    public List<ServicePriceDto> ServicePrice { get; set; } = new();

    [JsonPropertyName("value_added_service_price")]
    public Dictionary<string, object> ValueAddedServicePrice { get; set; } = new();
}

public class ServicePriceDto
{
    [JsonPropertyName("courierLogo")]
    public string CourierLogo { get; set; } = null!;

    [JsonPropertyName("courierName")]
    public string CourierName { get; set; } = null!;

    [JsonPropertyName("courierCode")]
    public string CourierCode { get; set; } = null!;

    [JsonPropertyName("courierNameAlias")]
    public string CourierNameAlias { get; set; } = null!;

    [JsonPropertyName("courierCodeAlias")]
    public string CourierCodeAlias { get; set; } = null!;

    [JsonPropertyName("serviceType")]
    public string ServiceType { get; set; } = null!;

    [JsonPropertyName("serviceName")]
    public string ServiceName { get; set; } = null!;

    [JsonPropertyName("slaDays")]
    public string SlaDays { get; set; } = null!;

    [JsonPropertyName("slaHours")]
    public int SlaHours { get; set; }

    [JsonPropertyName("vat")]
    public double Vat { get; set; }

    [JsonPropertyName("discountAmount")]
    public double DiscountAmount { get; set; }

    [JsonPropertyName("totalPrice")]
    public double TotalPrice { get; set; }

    [JsonPropertyName("initPrice")]
    public double InitPrice { get; set; }
}