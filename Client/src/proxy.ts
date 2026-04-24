import env from "@/services/EnvService";
import { NextRequest, NextResponse } from 'next/server';

async function isAuthenticated(request: NextRequest): Promise<boolean> {
  try {
    const response = await fetch(`${env.getInternalWebApiBaseUrl()}/api/auth/me`, {
      method: 'GET',
      headers: {
        cookie: request.headers.get('cookie') ?? '',
      },
      cache: 'no-store',
    });

    return response.ok;
  } catch (е) {
    console.error(е);
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
