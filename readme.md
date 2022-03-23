# ARCHIVED
This repository is archived and no longer maintained. Please use https://dev.azure.com/optenag/_git/Opten.Core instead.

# OPTEN Web Infrastructure

## Features

- Mailer
	- Simple Mailer
	- Razor View Mailer
	- Error Mailer
- Caching
	- Runtime
	- Varnish
- Active Directory
- Net

## Simple Mailer

Send a simple html or txt e-mail from web.config settings:

```
#!csharp
Opten.Web.Infrastructure.Mail.SimpleMailer mailer = new Opten.Web.Infrastructure.Mail.SimpleMailer(
	from: "test@opten.ch",
	displayName: "test@opten.ch",
	to: "test@opten.ch");

mailer.IsBodyHtml = false; // This is by default false

mailer.SendFromWebConfigSettings("Test", "Test");
```

## Razor View Mailer

Send a Razor View (.cshtml) with a custom model via e-mail from web.config settings.

> Install-Package **Opten.Web.Mvc** otherwise you can't use this!

```
#!csharp
Opten.Web.Infrastructure.Mail.RazorViewMailer mailer = new Opten.Web.Infrastructure.Mail.RazorViewMailer(
	from: "test@opten.ch",
	displayName: "test@opten.ch",
	to: "test@opten.ch");
	
mailer.SendFromWebConfigSettings(
	subject: "Test",
	viewName: "Member",
	model: new Member
	{
		FullName = "Calvin Frei",
		Age = 24
	});
```

Member.cshtml:

```
#!xml
@model Member
@{
    Layout = null;
}

<h1>
	@Model.FullName
</h1>

<p>
	Age: @Model.Age
</p>
```

> Default Search Location Path: "~/Views/MailTemplate/viewName.cshtml"

### TODO

- Test in ApiController
- Provide TempData
- Provide ViewData
- Provide @Url.AbsoluteAction extensions (from ActionMailer.Net)?
- Provide @Html.InlineImage extensions (from ActionMailer.Net)?
- Send Mail async (also for SimpleMailer)


## Error Mailer

You can set defaults in the web.config:

```
#!xml

<add key="OPTEN:mailer:error:from" value="info@domain.ch" />
<add key="OPTEN:mailer:error:displayName" value="DOMAIN NAME Error" />
<add key="OPTEN:mailer:error:to" value="notifications@opten.ch" />
<!-- optional -->
<add key="OPTEN:mailer:error:subject" value="Der Webserver meldet einen Fehler!" />
```

or in the constructor:

```
#!csharp

ErrorMailer mailer = new ErrorMailer(
	from: "info@domain.ch",
	displayName: "OPTEN Website Error",
	to: "notifications@opten.ch");

mailer.SendFromWebConfigSettings(...);
```

### TODO

- Test in ApiController 

## Caching

### Varnish

How to use with Umbraco (demo project): https://bitbucket.org/opten/opten.umbraco.plugins/src/master/src/Opten.Umbraco.Varnish.Integration/

VCL File: https://bitbucket.org/opten/opten.umbraco.plugins/src/master/src/Opten.Umbraco.Varnish.Integration/Resources/default.vcl

Blog: http://www.opten.ch/blog/2015/10/30/umbraco-plus-varnish-en/

Thanks to Snowflake (Typo 3): https://github.com/snowflakech/typo3-varnish

### Runtime

ADD DESCRIPTION HERE

## Active Directory

ADD DESCRIPTION HERE

## Net

### NoKeepAliveWebClient

Use this Web Client if you want to download a simple JSON. This client will close immediately after downloading it.

Safes Memory!
