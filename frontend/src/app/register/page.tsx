
"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useState, type FormEvent } from "react";

import { useAuth } from "@/features/auth/auth-context";

export default function RegisterPage() {
  const router = useRouter();
  const { register, status } = useAuth();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isRegistered, setIsRegistered] = useState(false);

  useEffect(() => {
    if (status === "authenticated") {
      router.replace("/dashboard");
    }
  }, [status, router]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");

    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    const passwordIsValid =
      password.length >= 8 &&
      /[A-Z]/.test(password) &&
      /[a-z]/.test(password) &&
      /[0-9]/.test(password) &&
      /[^a-zA-Z0-9]/.test(password);

    if (!passwordIsValid) {
      setError(
        "Your password must meet all the requirements listed below.",
      );
      return;
    }

    setIsSubmitting(true);

    try {
      await register({
        email,
        password,
        confirmPassword,
      });

      setIsRegistered(true);
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Unable to create your account. Please try again.",
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  if (status === "loading" || status === "authenticated") {
    return (
      <main className="flex min-h-screen items-center justify-center bg-slate-950 text-slate-300">
        Loading...
      </main>
    );
  }

  if (isRegistered) {
    return (
      <main className="flex min-h-screen items-center justify-center bg-[#628141] px-4 py-12 text-white">
        <div className="w-full max-w-md rounded-2xl border border-slate-800 bg-slate-900 p-8 text-center shadow-xl">
          <div className="mx-auto flex size-14 items-center justify-center rounded-full bg-green-500/10 text-2xl text-green-400">
            ✓
          </div>

          <h1 className="mt-6 text-2xl font-bold">
            Account created!
          </h1>

          <p className="mt-3 text-sm text-slate-400">
            Your Wadatika account has been created successfully.
            You can now sign in.
          </p>

          <Link
            href="/"
            className="mt-8 block w-full rounded-lg bg-indigo-600 px-4 py-3 text-sm font-semibold text-white transition hover:bg-indigo-500"
          >
            Continue to sign in
          </Link>
        </div>
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

          <h1 className="mt-2 text-3xl font-bold tracking-tight">
            Create your account
          </h1>

          <p className=" text-xs text-white/80">
            Start managing your projects with Wadatika.
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
              maxLength={256}
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              placeholder="you@example.com"
              className="w-full rounded-lg bg-[#1B211A] px-4 py-3 text-xs text-white outline-none transition placeholder:text-[#628141] focus:ring-offset-[#EBD5AB] focus:ring-2 focus:ring-[#EBD5AB]"
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
              autoComplete="new-password"
              required
              minLength={8}
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              placeholder="Create a password"
              className="w-full rounded-lg bg-[#1B211A] px-4 py-3 text-xs text-white outline-none transition placeholder:text-[#628141] focus:ring-offset-[#EBD5AB] focus:ring-2 focus:ring-[#EBD5AB]"
            />

            <p className="mt-2 text-xs leading-4 text-black/70">
              At least 8 characters, including uppercase, lowercase,
              a number and a special character.
            </p>
          </div>

          <div>
            <label
              htmlFor="confirmPassword"
              className="mb-2 block text-sm font-semibold text-black"
            >
              Confirm password
            </label>

            <input
              id="confirmPassword"
              type="password"
              autoComplete="new-password"
              required
              value={confirmPassword}
              onChange={(event) => setConfirmPassword(event.target.value)}
              placeholder="Enter your password again"
              className="w-full rounded-lg bg-[#1B211A] px-4 py-3 text-xs text-white outline-none transition placeholder:text-[#628141] focus:ring-offset-[#EBD5AB] focus:ring-2 focus:ring-[#EBD5AB]"
            />
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full rounded-lg bg-[#1B211A] cursor-pointer px-4 py-3 text-sm font-semibold text-white transition hover:bg-[#EBD5AB] hover:text-black/70 focus:outline-none focus:ring-2 focus:ring-[#EBD5AB] focus:ring-offset-2 focus:ring-offset-[#EBD5AB] disabled:cursor-not-allowed disabled:opacity-60"
          >
            {isSubmitting ? "Creating account..." : "Create account"}
          </button>

          <p className="text-center text-sm text-black/70">
            Already have an account?{" "}
            <Link
              href="/"
              className="font-medium text-black hover:text-white"
            >
              Sign in
            </Link>
          </p>
        </form>
      </div>
    </main>
  );
}
