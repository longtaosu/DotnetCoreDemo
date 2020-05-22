using Coravel.Invocable;
using Coravel.Mailer.Mail;
using Coravel.Mailer.Mail.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDemo.Invocables
{
    public class SendNightlyReportsEmailJob : IInvocable
    {
        private IMailer _mailer;
        public SendNightlyReportsEmailJob(IMailer mailer)
        {
            this._mailer = mailer;
        }

        public async Task Invoke()
        {
            Console.WriteLine("NightlyReportMailable Started....");
            await Task.Delay(10000);

            // You could grab multiple users from a DB query ;)
            var mailable = new NightlyReportMailable(new UserModel
            {
                Name = "Coravel is awesome!",
                Email = "test@test.com"
            });
            await this._mailer.SendAsync(mailable);
            Console.WriteLine($"NightlyReportMailable was sent at {DateTime.UtcNow}.");
        }
    }

    public class NightlyReportMailable : Mailable<UserModel>
    {
        private UserModel _user;

        public NightlyReportMailable(UserModel user) => this._user = user;

        public override void Build()
        {
            this.To(this._user)
                .From("from@test.com")
                .View("~/Views/Mail/NewUser.cshtml", this._user);
        }
    }

    public class UserModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
