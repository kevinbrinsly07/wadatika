
"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";

import { Sidebar } from "@/components/layout/sidebar";
import { useAuth } from "@/features/auth/auth-context";

export default function DashboardPage() {
  const router = useRouter();
  const { user, status, logout } = useAuth();

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
      <main className="flex min-h-screen items-center justify-center bg-slate-950 text-sm text-slate-400">
        Loading your workspace...
      </main>
    );
  }

  return (
    <div className="flex min-h-screen bg-slate-950 text-white">
      <div className="hidden md:block">
        <Sidebar email={user.email} onSignOut={handleLogout} />
      </div>

      <main className="min-w-0 flex-1">
        <header className="flex h-16 items-center justify-between border-b border-slate-800 px-5 sm:px-8">
          <div className="flex items-center gap-3">
            <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-indigo-500 text-sm font-bold md:hidden">
              W
            </div>
            <span className="text-sm font-medium text-slate-300">
              Personal workspace
            </span>
          </div>

          <button
            type="button"
            onClick={handleLogout}
            className="rounded-lg border border-slate-700 px-3 py-2 text-sm text-slate-300 transition-colors hover:bg-slate-800 md:hidden"
          >
            Sign out
          </button>

          <div className="hidden text-sm text-slate-400 md:block">
            {user.email}
          </div>
        </header>

        <div className="mx-auto max-w-6xl p-5 sm:p-8">
          <section>
            <p className="text-sm font-medium text-indigo-400">
              WORKSPACE OVERVIEW
            </p>

            <h1 className="mt-3 text-3xl font-bold tracking-tight sm:text-4xl">
              Welcome to Wadatika
            </h1>

            <p className="mt-3 max-w-2xl text-sm leading-6 text-slate-400 sm:text-base">
              Your workspace is ready. This is where you&apos;ll manage projects,
              organize tasks, and collaborate with your team.
            </p>
          </section>

          <section className="mt-10 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            <div className="rounded-xl border border-slate-800 bg-slate-900/50 p-5">
              <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-indigo-500/10 text-indigo-300">
                <span className="text-lg">▦</span>
              </div>
              <h2 className="mt-4 font-semibold text-white">Projects</h2>
              <p className="mt-2 text-sm leading-6 text-slate-400">
                Create projects and organize your team&apos;s work in one place.
              </p>
              <p className="mt-4 text-xs font-medium text-slate-500">
                Coming soon
              </p>
            </div>

            <div className="rounded-xl border border-slate-800 bg-slate-900/50 p-5">
              <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-emerald-500/10 text-emerald-300">
                <span className="text-lg">✓</span>
              </div>
              <h2 className="mt-4 font-semibold text-white">Tasks</h2>
              <p className="mt-2 text-sm leading-6 text-slate-400">
                Track progress, set priorities, and stay on top of deadlines.
              </p>
              <p className="mt-4 text-xs font-medium text-slate-500">
                Coming soon
              </p>
            </div>

            <div className="rounded-xl border border-slate-800 bg-slate-900/50 p-5 sm:col-span-2 lg:col-span-1">
              <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-amber-500/10 text-amber-300">
                <span className="text-lg">◎</span>
              </div>
              <h2 className="mt-4 font-semibold text-white">Team collaboration</h2>
              <p className="mt-2 text-sm leading-6 text-slate-400">
                Invite teammates and work together across your organization.
              </p>
              <p className="mt-4 text-xs font-medium text-slate-500">
                Coming soon
              </p>
            </div>
          </section>

          <section className="mt-8 rounded-xl border border-dashed border-slate-700 p-6 sm:p-8">
            <h2 className="font-semibold text-white">
              Your workspace starts here
            </h2>
            <p className="mt-2 max-w-2xl text-sm leading-6 text-slate-400">
              Once we implement organizations and projects, your recent
              activity and project progress will appear here.
            </p>
          </section>
        </div>
      </main>
    </div>
  );
}
