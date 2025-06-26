using System;
using System.Collections.Generic;

public partial class Certificado
{
    public int Id { get; set; }

    public Guid IdServidor { get; set; }

    public string? Thumbprint { get; set; }

    public string? IssuedTo { get; set; }

    public string? Subject { get; set; }

    public string? IssuedBy { get; set; }

    public string? Issuer { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public string? IntendedPurposes { get; set; }

    public string? FriendlyName { get; set; }

    public string? CertificateTemplate { get; set; }

    public DateTime FechaRecuperado { get; set; }

    public int Estado { get; set; }

}
