# Route Management System API with Neo4j

This project is a RESTful API built with .NET and Neo4j, designed to manage route-related data for a bus transportation system. The system supports managing routes, stops, bus operators, buses, drivers, and schedules with high scalability and complex relationship queries.

## Database Design with Neo4j

### Neo4j
- **Complex Relationship Management**: Neo4j excels in managing relationships between entities like routes, stops, and drivers.
- **Scalability**: Easily extend the schema without performance impact.
- **Graph Queries**: Perform complex queries like shortest paths or network analyses efficiently.

### Schema Overview
#### Relationships
- **HAS_ROUTE**: `City → Route` (startTime, endTime).
- **HAS_STOP**: `Route → Stop` (sequence).
- **OPERATES**: `BusOperator → Route`.
- **NEXT_STOP**: `Stop → Stop` (distance, expectedTravelTime).
- **ASSIGNED_TO**: `Bus → Schedule` (assignmentDate).
- **DRIVEN_BY**: `Driver → Schedule` (assignmentDate).
- **HAS_SCHEDULE**: `Route → Schedule` (activeFrom, activeUntil).

### Example Queries
- Find all routes between two cities.
- Retrieve all stops for a specific route in order.
- Identify the shortest path between two stops.

## Implementation Results
The system is fully implemented, leveraging Neo4j to handle complex relationships and ensure efficient data queries and scalability.

