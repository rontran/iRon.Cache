# iRon.Cache

iRon.Cache is a Redis repository service for .net

## License 
This is free for you to use whatever.  No warrenty.  Use at your own risk.

## Setup

Edit startup.cs and add
```
   services.UseCache(this.Configuration);
```
Edit appsettings.json and add

```
 "iRon.Cache": {
    "ConnectionString": "192.168.0.227",
    "Prefix": "TEST_",
    "Enabled": true,
    "Duration": {
      "NONE": 0,
      "LOW": 30,
      "NORMAL": 360,
      "HIGH": 3600,
      "FOREVER": 360000
    }
  }
```
