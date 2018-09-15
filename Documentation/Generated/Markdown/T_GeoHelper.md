# GeoHelper
## Base Class
- Object
## Fields
Flags|Type|Name
-|-|-
*static*|Point|GERMANY_CENTERPOINT
## Methods
Flags|Result|Name|Parameters
-|-|-|-
*static*|IGeometry|StringToGeometry|( String strWKBAsString )
*static*|String|GeometryToString|( IGeometry geo )
*static*|Polygon|CreatePolygon|( Point[] points )
*static*|PointLatLng|CoordinateToPointLatLng|( Coordinate c )