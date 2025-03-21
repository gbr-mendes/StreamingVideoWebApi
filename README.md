# StreamingVideoWebApi

API criada para a aplicação de streaming.
O objetivo é consultar os dados populados pelo [microsserviço](https://github.com/gbr-mendes/StreamingVideoIndexer) do banco de dados e servir para a aplicação frontend

TODO:
- Cachear no redis a url gerada para s3 com base no proprio lifetime deinido para a expiração da url
- Implementar funcionalidade de streaming
