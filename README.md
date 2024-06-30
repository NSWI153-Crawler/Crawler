# Crawler
[https://webik.ms.mff.cuni.cz/nswi153/seminar-project-webcrawler.html](https://webik.ms.mff.cuni.cz/nswi153/seminar-project-webcrawler.html)

## TODO List

**Frontend**
- [ ] Website records list: pages, filter, sort
- [ ] Executions list (for all records / for single record)
- [ ] Crawler visualization
- [ ] Design, css

**Backend**
- [ ] Routing, Serving
- [ ] Crawler execution management (multi thread)
- [ ] OpenAPI/Swagger

**Database**
- [ ] Create database (Graph QL)

**Deployment**
- [ ] Linter, Prettier
- [ ] .env
- [ ] Dockerfiles
- [ ] Docker Compose

**Other**
- [ ] Tests?

## Deployment
After cloning the repository, run the following command to deploy the application:
```docker compose up --build```
The ```--build``` option is necessary only during the first deployment. 


## Services

### Backend
The Backend services are running on port **8080**. 

### GraphQL
Click the following [link](http://localhost:8080/playground) to visit the GraphQL playground.

###Swagger
Swagger is accesible at the following [link]( http://localhost:8080/swagger/index.html).

### Frontend
The frontend service is running on port **8083**. 
Click the following [link](http://localhost:8083/) to visit.

### Database
The database service is running on port **3306**. 

### MyPHP
The MyPHP dashboard ia accesible on port **8082**. 
Click the following [link](http://localhost:8082/) to visit.
