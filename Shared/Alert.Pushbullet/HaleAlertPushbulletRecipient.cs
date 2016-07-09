using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Hale.Alert.Pushbullet
{
    public class HaleAlertPushbulletRecipient: IHaleAlertRecipient
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Target { get; set; }
        public PushbulletPushTarget TargetType { get; set; }
        public string AccessToken { get; set; }
    }

    public enum PushbulletPushTarget
    {
        Device, Email, Channel, Client
    }
}
