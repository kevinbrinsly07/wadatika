"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState, type FormEvent } from "react";

import { useAuth } from "@/features/auth/auth-context";

export default function Home() {
  const router = useRouter();
  const { login, status } = useAuth();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    setError("");
    setIsSubmitting(true);

    try {
      await login({ email, password });
      router.push("/dashboard");
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Unable to sign in. Please try again.",
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  if (status === "loading") {
    return (
      <main className="flex min-h-screen items-center justify-center bg-slate-950 text-slate-300">
        Restoring your session...
      </main>
    );
  }

  return (
    <main className="flex min-h-screen items-center justify-center bg-[#1B211A] px-4 py-12 text-white">
      <div className="w-full max-w-md">
        <div className="mb-8 text-center">
          <Link href="/" className="text-2xl font-bold tracking-tight">
            <span className="text-[#EBD5AB]">W</span>adatika
          </Link>

          <h1 className="mt-3 text-3xl font-bold tracking-tight">
            Welcome back
          </h1>

          <p className="text-xs text-white/70">
            Sign in to continue to your workspace.
          </p>
        </div>

        <form
          onSubmit={handleSubmit}
          className="space-y-5 rounded-2xl bg-[#628141] p-8 shadow-xl"
        >
          {error && (
            <div
              role="alert"
              className="rounded-lg border border-red-900 bg-red-950/50 px-4 py-3 text-sm text-red-300"
            >
              {error}
            </div>
          )}

          <div>
            <label
              htmlFor="email"
              className="mb-2 block text-sm font-semibold text-black"
            >
              Email address
            </label>

            <input
              id="email"
              type="email"
              autoComplete="email"
              required
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              placeholder="you@example.com"
              className="w-full rounded-lg bg-[#1B211A] px-4 py-3 text-sm text-white outline-none transition placeholder:text-[#628141] focus:border-[#EBD5AB] focus:ring-2 focus:ring-[#EBD5AB]"
            />
          </div>

          <div>
            <label
              htmlFor="password"
              className="mb-2 block text-sm font-semibold text-black"
            >
              Password
            </label>

            <input
              id="password"
              type="password"
              autoComplete="current-password"
              required
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              placeholder="Enter your password"
              className="w-full rounded-lg bg-[#1B211A] px-4 py-3 text-sm text-white outline-none transition placeholder:text-[#628141] focus:border-[#EBD5AB] focus:ring-2 focus:ring-[#EBD5AB]"
            />
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full rounded-lg bg-[#1B211A] px-4 cursor-pointer py-3 text-sm font-semibold text-white transition hover:bg-[#EBD5AB] hover:text-black focus:outline-none focus:ring-2 focus:ring-[#EBD5AB] focus:ring-offset-2 focus:ring-offset-[#EBD5AB] disabled:cursor-not-allowed disabled:opacity-60"
          >
            {isSubmitting ? "Signing in..." : "Sign in"}
          </button>

          <p className="text-center text-sm text-black/70">
            Don&apos;t have an account?{" "}
            <Link
              href="/register"
              className="font-medium text-black hover:text-white"
            >
              Create an account
            </Link>
          </p>
        </form>

        <p className="mt-6 text-center text-[10px] text-white/70">
          Project management for modern teams.
        </p>
      </div>
    </main>
  );
}
