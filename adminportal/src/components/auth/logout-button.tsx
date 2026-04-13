"use client";

import { LogoutOutlined } from "@ant-design/icons";
import { Alert, Button, Space, Typography } from "antd";
import { useAuth } from "@shesha-io/reactjs";
import { useRouter } from "next/navigation";
import { useState } from "react";

export const LogoutButton = () => {
  const router = useRouter();
  const { loginInfo, logoutUser } = useAuth();
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleLogout = async () => {
    try {
      setError(null);
      setIsLoggingOut(true);
      await logoutUser();
      router.replace("/login");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Could not log out. Please try again.");
    } finally {
      setIsLoggingOut(false);
    }
  };

  return (
    <Space direction="vertical" size="small" style={{ alignItems: "flex-end" }}>
      {loginInfo?.fullName ? (
        <Typography.Text style={{ background: "#fff", border: "1px solid #e5e7eb", borderRadius: 999, padding: "4px 12px", fontSize: 13 }}>
          {loginInfo.fullName}
        </Typography.Text>
      ) : null}
      <Button icon={<LogoutOutlined />} onClick={handleLogout} loading={isLoggingOut} style={{ borderRadius: 999 }}>
        Log out
      </Button>
      {error ? <Alert type="error" showIcon message={error} style={{ maxWidth: 320 }} /> : null}
    </Space>
  );
};