DotNetServer is like a J2EE server or OSGI environment.
I wish to provide a way to "deploy" easily applications, resolving dependencies between "bundle" to consume or provide services.
The next step will be to provide a "ASP.Net" service, to easily deploy WebApplication without the needs of IIS/Apache (they could be used as proxy to ensure security, SSL etc.).
So you will be able to deploy a new WebApplication as easily as in Tomcat with a "admin" page.
You will be able also to start/stop/restart WebApplications or "bundle services".

Some services will be provided by default:
- Inter-App communication, using DBus
- ASP.Net hosting, using Xsp
- Distributed cache, using NMemcached
- Queue, using ???
- Database connection pool
- Simple database, using SQLite-Net ?
- Scheduler/Jobs container, using Quartz.net

Some "applications" will be provided by default:
- WebApp Administration portal, to see others applications, and to manage lifecycles (start/stop/etc.)
- ssh Administration portal? to manage remotely with command line? or provide a client with WCF/.NetRemoting/DBus communication?

Some API will be provided by default:
- IOC (Spring.net? with Spring.Fluent?)
- ORM (NHibernate? with NHibernate.Fluent and NHibernate.Linq)

The future step will be to provide a abstract layer to these API, to be able to switch between different implementations (kind of JSR)
