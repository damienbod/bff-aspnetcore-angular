import { HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { getCookie } from './getCookie';

export function secureApiInterceptor(
  request: HttpRequest<unknown>,
  next: HttpHandlerFn
) {
  const secureRoutes = [getApiUrl()];

  if (!secureRoutes.find((x) => request.url.startsWith(x))) {
    return next(request);
  }

  request = request.clone({
    headers: request.headers.set(
      'X-XSRF-TOKEN',
      getCookie('XSRF-RequestToken')
    ),
  });

  return next(request);
}

function getApiUrl() {
  const backendHost = getCurrentHost();

  return `${backendHost}/api/`;
}

function getCurrentHost() {
  const host = globalThis.location.host;
  const url = `${globalThis.location.protocol}//${host}`;
  return url;
}
