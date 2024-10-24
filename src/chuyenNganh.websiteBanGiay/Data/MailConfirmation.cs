using System;
using System.Collections.Generic;

namespace chuyenNganh.websiteBanGiay.Data;

public partial class MailConfirmation
{
    public int MailId { get; set; }

    public int? OrderId { get; set; }

    public bool? EmailSent { get; set; }

    public DateTime? SentDate { get; set; }

    public virtual Order? Order { get; set; }
}
