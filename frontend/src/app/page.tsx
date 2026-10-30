"use client";

import { useEffect, useState } from "react";

type HealthResponse = {
  status: string;
  message: string;
  timestamp: string;
};

type ApiState =
  | { status: "loading" }
  | { status: "success"; data: HealthResponse }
  | { status: "error"; message: string };

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

export default function Home() {
  const [apiState, setApiState] = useState<ApiState>({
    status: "loading",
  });

  useEffect(() => {
    const controller = new AbortController();

    async function checkApi() {
      try {
        const response = await fetch(`${API_URL}/api/health`, {
          signal: controller.signal,
        });

        if (!response.ok) {
          throw new Error(`HTTP error: ${response.status}`);
        }

        const data: HealthResponse = await response.json();

        setApiState({
          status: "success",
          data,
        });
      } catch (error) {
        if (error instanceof Error && error.name === "AbortError") {
          return;
        }

        setApiState({
          status: "error",
          message: error instanceof Error ? error.message : "Unknown API error",
        });
      }
    }

    void checkApi();

    return () => controller.abort();
  }, []);

  return (
    <main className="flex min-h-screen items-center justify-center bg-slate-950 px-6 text-white">
      <div className="w-full max-w-xl rounded-2xl border border-slate-800 bg-slate-900 p-8 shadow-xl">
        <p className="text-sm font-medium tracking-widest text-blue-400">
          WADATIKA
        </p>

        <h1 className="mt-3 text-4xl font-bold">
          Project management, simplified.
        </h1>

        <p className="mt-3 text-slate-400">
          Your workspace for teams, projects and tasks.
        </p>

        <div className="mt-8 rounded-xl border border-slate-800 p-5">
          <h2 className="font-semibold">
            Backend connection
          </h2>

          {apiState.status === "loading" && (
            <p className="mt-3 text-yellow-400">
              Connecting to API...
            </p>
          )}

          {apiState.status === "success" && (
            <div className="mt-3">
              <p className="text-green-400">
                ● {apiState.data.status}
              </p>

              <p className="mt-2 text-sm text-slate-400">
                {apiState.data.message}
              </p>

              <p className="mt-2 text-xs text-slate-500">
                {new Date(
                  apiState.data.timestamp
                ).toLocaleString()}
              </p>
            </div>
          )}

          {apiState.status === "error" && (
            <p className="mt-3 text-red-400">
              API connection failed: {apiState.message}
            </p>
          )}
        </div>
      </div>
    </main>
  );
}