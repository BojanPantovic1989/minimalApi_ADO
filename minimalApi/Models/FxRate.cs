using System.ComponentModel.DataAnnotations.Schema;

namespace minimalApi.Models;

[Keyless]
[Table("v_currencies")]
public class FxRate
{
    [Column("currency_code")]
    public string CurrencyCode { get; set; } = string.Empty;
    public string Rate { get; set; } = string.Empty;
}

