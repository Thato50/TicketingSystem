"use client";

import { PropsWithChildren, useEffect, Suspense } from "react";
import { Spin } from "antd";
import { useAuth } from "@shesha-io/reactjs";
import { usePathname, useRouter, useSearchParams } from "next/navigation";

const getReturnUrl = (pathname: string | null, searchParams: { toString: () => string }) => {
  const query = searchParams.toString();
  if (!pathname) return "/";
  return query ? `${pathname}?${query}` : pathname;
};

const ProtectedRouteInner = ({ children }: PropsWithChildren) => {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const { isLoggedIn, state } = useAuth();

  useEffect(() => {
    if (state.status === "ready" && !isLoggedIn) {
      const returnUrl = getReturnUrl(pathname, searchParams);
      router.replace(`/login?returnUrl=${encodeURIComponent(returnUrl)}`);
    }
  }, [isLoggedIn, pathname, router, searchParams, state.status]);

  if (state.status === "waiting" || state.status === "inprogress") {
    return (
      <div style={{ minHeight: "100vh", display: "grid", placeItems: "center" }}>
        <Spin size="large" />
      </div>
    );
  }

  if (!isLoggedIn) return null;

  return <>{children}</>;
};

export const ProtectedRoute = ({ children }: PropsWithChildren) => {
  return (
    <Suspense fallback={
      <div style={{ minHeight: "100vh", display: "grid", placeItems: "center" }}>
        <Spin size="large" />
      </div>
    }>
      <ProtectedRouteInner>{children}</ProtectedRouteInner>
    </Suspense>
  );
};