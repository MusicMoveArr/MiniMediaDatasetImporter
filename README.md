# MiniMedia Dataset Importer
A simple dataset import tool to import MusicBrainz, Discogs datasets

You can download the dataset from MusicBrainz at https://metabrainz.org/datasets/download

Download the discogs dadaset here https://discogs-data-dumps.s3.us-west-2.amazonaws.com/index.html

Using this tool to insert the entire dataset into postgres from MusicBrainz will take aprox ~10hours (depending on hardware of course)

# Features
1. Bulk-insert MusicBrainz dataset (from json-format)
2. Supports inserting the data for MusicBrainz artists, areas, labels, releases, release labels, release tracks, release track artists

# Usage example to import all artists from artist file
```
dotnet MiniMediaDatasetImporter.dll importmusicbrainz \
--connection-string "Host=192.168.1.1;Username=postgres;Password=postgres;Database=minimedia;Pooling=true;MinPoolSize=5;MaxPoolSize=100;" \
--file "~/dataset/artist"
```
