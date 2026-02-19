import 'leaflet';

declare module 'leaflet' {
  namespace Symbol {
    function arrowHead(options?: any): any;
  }

  function polylineDecorator(
    path: Polyline | Polyline[] | LatLngExpression[] | LatLngExpression[][],
    options?: any
  ): Layer;
}
