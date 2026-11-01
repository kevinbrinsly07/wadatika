"use client";

import { useRouter } from "next/navigation";

import { useAuth } from "@/features/auth/auth-context";

export default function DashboardPage() {
  const router = useRouter();
  const { user, status, logout } = useAuth();

  async function handleLogout() {
    await logout();
    router.replace("/");
  }

  if (status === "loading") {
    return <main className="p-8">Loading your workspace...</main>;
  }

  if (status === "unauthenticated" || !user) {
    router.replace("/");
    return null;
  }

  return (
    <main className="min-h-screen bg-slate-950 p-8 text-white">
      <div className="mx-auto max-w-5xl">
        <header className="flex items-center justify-between border-b border-slate-800 pb-6">
          <h1 className="text-2xl font-bold">
            Wadatika
          </h1>

          <button
            onClick={handleLogout}
            className="rounded-lg border border-slate-700 px-4 py-2 text-sm hover:bg-slate-800"
          >
            Sign out
          </button>
        </header>

        <section className="mt-12">
          <p className="text-sm text-indigo-400">
            YOUR WORKSPACE
          </p>

          <h2 className="mt-3 text-3xl font-bold">
            Welcome, {user.email}
          </h2>

          <p className="mt-3 text-slate-400">
            Your Wadatika workspace is ready.
          </p>
        </section>
      </div>
    </main>
  );
}