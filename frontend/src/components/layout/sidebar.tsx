
"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

type SidebarProps = {
  email: string;
  onSignOut: () => void;
};

const navigation = [
  { label: "Overview", href: "/dashboard", icon: "grid", available: true },
  { label: "My tasks", href: "#", icon: "check", available: false },
  { label: "Projects", href: "#", icon: "folder", available: false },
  { label: "Teams", href: "#", icon: "users", available: false },
];

function NavigationIcon({ name }: { name: string }) {
  const paths: Record<string, React.ReactNode> = {
    grid: (
      <>
        <rect x="3" y="3" width="7" height="7" rx="1" />
        <rect x="14" y="3" width="7" height="7" rx="1" />
        <rect x="3" y="14" width="7" height="7" rx="1" />
        <rect x="14" y="14" width="7" height="7" rx="1" />
      </>
    ),
    check: <path d="m5 12 4 4L19 6" />,
    folder: (
      <path d="M3 7a2 2 0 0 1 2-2h5l2 2h7a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V7Z" />
    ),
    users: (
      <>
        <path d="M16 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
        <circle cx="10" cy="7" r="4" />
        <path d="M20 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75" />
      </>
    ),
  };

  return (
    <svg
      aria-hidden="true"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="1.7"
      strokeLinecap="round"
      strokeLinejoin="round"
      className="h-5 w-5 shrink-0"
    >
      {paths[name]}
    </svg>
  );
}

export function Sidebar({ email, onSignOut }: SidebarProps) {
  const pathname = usePathname();

  return (
    <aside className="flex h-screen w-64 shrink-0 flex-col border-r border-slate-800 bg-slate-950 text-slate-300">
      <div className="flex h-16 items-center gap-3 border-b border-slate-800 px-6">
        <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-indigo-500 text-sm font-bold text-white">
          W
        </div>
        <span className="text-lg font-semibold tracking-tight text-white">
          Wadatika
        </span>
      </div>

      <div className="border-b border-slate-800 p-4">
        <button
          type="button"
          className="flex w-full items-center gap-3 rounded-lg border border-slate-800 bg-slate-900 px-3 py-3 text-left hover:bg-slate-800"
        >
          <div className="flex h-8 w-8 items-center justify-center rounded-md bg-indigo-500/15 text-sm font-semibold text-indigo-300">
            W
          </div>
          <div className="min-w-0 flex-1">
            <p className="truncate text-sm font-medium text-white">
              Personal workspace
            </p>
            <p className="text-xs text-slate-500">Free plan</p>
          </div>
          <span className="text-xs text-slate-500">⌄</span>
        </button>
      </div>

      <nav aria-label="Main navigation" className="flex-1 space-y-1 p-4">
        <p className="mb-3 px-3 text-xs font-semibold uppercase tracking-wider text-slate-500">
          Workspace
        </p>

        {navigation.map((item) => {
          const isActive = item.available && pathname === item.href;

          const className = [
            "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm transition-colors",
            isActive
              ? "bg-indigo-500/10 font-medium text-indigo-300"
              : item.available
                ? "text-slate-400 hover:bg-slate-900 hover:text-white"
                : "cursor-not-allowed text-slate-600",
          ].join(" ");

          const content = (
            <>
              <NavigationIcon name={item.icon} />
              <span className="flex-1">{item.label}</span>
              {!item.available && (
                <span className="text-[10px] uppercase tracking-wide text-slate-600">
                  Soon
                </span>
              )}
            </>
          );

          return item.available ? (
            <Link key={item.label} href={item.href} className={className}>
              {content}
            </Link>
          ) : (
            <div key={item.label} aria-disabled="true" className={className}>
              {content}
            </div>
          );
        })}
      </nav>

      <div className="border-t border-slate-800 p-4">
        <div className="mb-3 flex items-center gap-3 px-2">
          <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-slate-700 text-sm font-semibold text-white">
            {email.charAt(0).toUpperCase()}
          </div>
          <div className="min-w-0">
            <p className="truncate text-sm font-medium text-white">
              {email}
            </p>
            <p className="text-xs text-slate-500">Account</p>
          </div>
        </div>

        <button
          type="button"
          onClick={onSignOut}
          className="w-full rounded-lg px-3 py-2 text-left text-sm text-slate-400 transition-colors hover:bg-slate-900 hover:text-white"
        >
          Sign out
        </button>
      </div>
    </aside>
  );
}
