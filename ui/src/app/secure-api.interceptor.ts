import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { getCookie } from "./getCookie";

@Injectable()
export class SecureApiInterceptor implements HttpInterceptor {
  private secureRoutes = [ this.getApiUrl()];

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ) {

    if (!this.secureRoutes.find((x) => request.url.startsWith(x))) {
      return next.handle(request);
    }

    request = request.clone({
      headers: request.headers.set('X-XSRF-TOKEN', getCookie("XSRF-RequestToken")),
    });

    return next.handle(request);
  }

  public getApiUrl() {
    const backendHost = this.getCurrentHost();

    return `${backendHost}/api/`;
  }

  public getCurrentHost() {
    const host = window.location.host;
    const url = `${window.location.protocol}//${host}`;
    return url;
  }
}
