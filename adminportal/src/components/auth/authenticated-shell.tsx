"use client";

import { PropsWithChildren } from "react";
import { LogoutButton } from "./logout-button";

export const AuthenticatedShell = ({ children }: PropsWithChildren) => {
  return (
    <div style={{ minHeight: "100vh" }}>
      <div style={{ position: "fixed", top: 16, right: 16, zIndex: 1000 }}>
        <LogoutButton />
      </div>
      {children}
    </div>
  );
};