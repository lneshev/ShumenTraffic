import env from "@/services/EnvService";
import { NextRequest, NextResponse } from 'next/server';

async function isAuthenticated(request: NextRequest): Promise<boolean> {
  try {
    const response = await fetch(`${env.getPublicWebApiBaseUrl()}/api/auth/me`, {
      method: 'GET',
      headers: {
        cookie: request.headers.get('cookie') ?? '',
      },
      cache: 'no-store',
    });
    console.log(response);
    const text = await response.text();
    console.log(text);
    return response.ok;
  } catch (e) {
    console.log(e);
    return false;
  }
}

export async function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;
  const authenticated = await isAuthenticated(request);

  if (pathname === '/admin/login' || pathname.startsWith('/admin/login/')) {
    if (authenticated) {
      return NextResponse.redirect(new URL('/admin', request.url));
    }
    return NextResponse.next();
  }

  if (!authenticated) {
    return NextResponse.redirect(new URL('/admin/login', request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ['/admin/:path*'],
};
