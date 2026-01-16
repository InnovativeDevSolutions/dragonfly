---
seo:
  title: ArmaDragonflyClient - Ultra-Fast In-Memory Database for Arma 3
  description: Experience next-level persistence in Arma 3 with ArmaDragonflyClient - an ultra-fast in-memory data store powered by C# .NET 8 and DragonflyDB. Redis-like API with unparalleled performance.
---

::u-page-hero
#title
ArmaDragonflyClient

#description
Experience next-level persistence in Arma 3 with our mod powered by C# .NET 8 and DragonflyDB.

This ultra-fast in-memory data store offers unparalleled performance and scalability for your Arma 3 gameplay data management needs.

#links
  :::u-button
  ---
  color: primary
  size: xl
  to: /getting-started/installation
  trailing-icon: i-lucide-arrow-right
  ---
  Get started
  :::

  :::u-button
  ---
  color: neutral
  icon: simple-icons-github
  size: xl
  to: https://github.com/jschmidt92/dragonfly
  variant: outline
  ---
  Star on GitHub
  :::
::

::u-page-section
#title
Powerful features for Arma 3 persistence

#features
  :::u-page-feature
  ---
  icon: i-lucide-zap
  ---
  #title
  [Blazing Fast]{.text-primary} Performance
  
  #description
  Powered by DragonflyDB, an ultra-fast Redis alternative. Experience minimal latency and maximum throughput for all database operations in your Arma 3 missions.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-database
  ---
  #title
  [Redis-Like]{.text-primary} API
  
  #description
  Familiar Redis-style commands for key-value, hash tables, and lists. Easy to learn, powerful to use with operations like SET, GET, HSET, LPUSH, and more.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-save
  ---
  #title
  [Persistent Storage]{.text-primary}
  
  #description
  Save your data to disk when needed with the SAVE command. DragonflyDB handles persistence efficiently with snapshots and AOF logs.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-shield-check
  ---
  #title
  [Production Ready]{.text-primary}
  
  #description
  Built with C# .NET 8 and DragonflyDB's proven architecture. Reliable, stable, and battle-tested for mission-critical Arma 3 deployments.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-split
  ---
  #title
  [Smart Chunking]{.text-primary}
  
  #description
  Automatic data chunking for large payloads exceeding the 20KB buffer. Seamlessly handles large arrays, loadouts, and complex game data.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-users
  ---
  #title
  [Context Awareness]{.text-primary}
  
  #description
  Player Steam ID integration with special placeholders for single-player and multiplayer modes. Easy per-player data management without manual key tracking.
  :::
::
