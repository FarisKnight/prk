prk
===

A Parallel Rabin Karp Method for Plagiarism Detection in Indonesian Language

This repo included a class that writed on c#.

1. CRUD.cs, an entity class that contains method to create, read, update and delete to database.

2. Document.cs, a controller class for document handling,

3. Preprocess.cs, a controller class that contains preprocessing phase Tokenize, Casefold, Stopword and inherits
that methods to ECSStemmer.cs

4. ECSStemm.cs, a controller class inherited by Preprocess using Enhanced Confix Stripping Stemmer

5. Kgram.cs, a controller class to tokenize using kgram approach

6. PRabinKarp.cs, a controller class for string match, using Parallel Rabin Karp (Brodanac et al, 2013)

7. ISimilarity, interface for count similarity between two documents

8. IVerify, interface for verify documents (additional)
