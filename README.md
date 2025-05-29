# MiniMedia Dataset Importer
A simple dataset import tool to import (for now) only MusicBrainz dataset

You can download the dataset from MusicBrainz at https://metabrainz.org/datasets/download

Using this tool to insert the entire dataset into postgres from MusicBrainz will take aprox ~10hours (depending on hardware of course)

# Features
1. Bulk-insert MusicBrainz dataset (from json-format)
2. Supports inserting the data for MusicBrainz artists, areas, labels, releases, release labels, release tracks, release track artists

# Usage example to import all artists from artist file
```
dotnet importmusicbrainz \
--connection-string "Host=192.168.1.1;Username=postgres;Password=postgres;Database=minimedia;Pooling=true;MinPoolSize=5;MaxPoolSize=100;" \
--file "~/dataset/artist"
```
