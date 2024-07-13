# UrlShortener
This is a basic implmenetation of how a url shortnener behaves under the hood.
The main idea here is to generate a unique code for any given url.
## Improvements Notes
- Make sure there are no collisions while generating the unique code.
    The shorter the code, the more the likelihood a collision might occur.
- Use any caching mechanism (local, redis, memcahed) to improve the full url retrieval.
