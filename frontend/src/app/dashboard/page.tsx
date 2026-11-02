"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { WorkspaceLayout } from "@/components/layout/workspace-layout";
import { useAuth } from "@/features/auth/auth-context";
import { OrganizationManager } from "@/features/organizations/components/organization-manager";

export default function DashboardPage() {
  const router = useRouter();
  const { user, status } = useAuth();

  useEffect(() => {
    if (status === "unauthenticated") {
      router.replace("/");
    }
  }, [status, router]);

  if (status === "loading" || !user) {
    return (
      <main className="flex min-h-screen items-center justify-center bg-[#1B211A] text-sm text-[#628141]">
        Loading your workspace...
      </main>
    );
  }

  return (
    <WorkspaceLayout>
    <main className="mx-auto max-w-6xl p-5 sm:p-8">
      <OrganizationManager />
    </main>
  </WorkspaceLayout>
  );
}
