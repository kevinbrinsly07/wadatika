"use client";

import type { ReactNode } from "react";
import { useEffect } from "react";
import { useRouter } from "next/navigation";

import { Sidebar } from "@/components/layout/sidebar";
import { useAuth } from "@/features/auth/auth-context";
import { useOrganization } from "@/features/organizations/organization-context";

type WorkspaceLayoutProps = {
  children: ReactNode;
};

export function WorkspaceLayout({ children }: WorkspaceLayoutProps) {
  const router = useRouter();
  const { user, status, logout } = useAuth();
  const { selectedOrganization, isLoading: isLoadingOrganizations } =
    useOrganization();

  useEffect(() => {
    if (status === "unauthenticated") {
      router.replace("/");
    }
  }, [status, router]);

  async function handleLogout() {
    try {
      await logout();
    } finally {
      router.replace("/");
    }
  }

  if (status === "loading" || !user) {
    return (
      <main className="flex min-h-screen items-center justify-center bg-[#1B211A] text-sm text-[#628141]">
        Loading your workspace...
      </main>
    );
  }

  return (
    <div className="flex min-h-screen bg-[#1B211A] text-[#628141]">
      <div className="hidden md:block">
        <Sidebar email={user.email} onSignOut={handleLogout} />
      </div>

      <div className="min-w-0 flex-1">
        <header className="flex h-16 items-center justify-between px-5 sm:px-8">
          <div className="flex items-center gap-3">
            <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-[#628141] text-sm font-bold text-[#1B211A] md:hidden">
              W
            </div>

            <span className="text-sm font-medium text-[#628141]">
              {isLoadingOrganizations
                ? "Loading organization..."
                : selectedOrganization?.name ?? "No organization selected"}
            </span>
          </div>

          <button
            type="button"
            onClick={handleLogout}
            className="rounded-lg px-3 py-2 text-sm text-[#628141] hover:bg-[#8BAE66] md:hidden"
          >
            Sign out
          </button>

          <div className="hidden text-sm text-[#628141] md:block">
            {user.email}
          </div>
        </header>

        {children}
      </div>
    </div>
  );
}