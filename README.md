croppic-net
===========

Croppic Server-side .NET (C#) Implementation using ASP.NET MVC

Notes:
======

- This implementation abstracts the Storage provider, but provides a FileSystem implementation. You can create your own implementation of the ITemporaryStorage<TKey> interface to store the pictures to anywhere you want.
- To simplify the configuration, no dependency injector was used, but the code is prepared for one.
- You should provide a ILog implementation so you can log any errors cropping pictures.
- The only external dependency (outside of Microsoft stuff) is the SixPack library (used for Logging abstraction and ConfigurationSection deserialization in this case).

Quirks on Croppic.js
====================

- There is a bug in the original JS of croppic, that only accepts responses in text/html. There is config flag that you can use if you don't want to change the JS, but if you can, please make this change:

```
response = jQuery.parseJSON(data)
```

to

```
response = typeof data === 'string' ? jQuery.parseJSON(data) : data
```

This appears two times in the entire JS file.
