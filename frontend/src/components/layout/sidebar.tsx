"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

import { useOrganization } from "@/features/organizations/organization-context";

type SidebarProps = {
  email: string;
  onSignOut: () => void;
};

const navigation = [
  { label: "Overview", href: "/dashboard", icon: "grid", available: true },
  { label: "My tasks", href: "/tasks", icon: "check", available: true },
  { label: "Projects", href: "/projects", icon: "folder", available: true },
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

  const { organizations, selectedOrganization, selectOrganization, isLoading } =
    useOrganization();

  return (
    <aside className="flex h-screen w-64 shrink-0 flex-col bg-[#628141] text-[#1B211A]">
      {/* Brand */}
      <div className="flex h-16 items-center font-bold  px-6">
        <span className="text-[#EBD5AB]">W</span>adatika
      </div>

      {/* Organization selector */}
      <div className="p-4">
        <div className="rounded-lg bg-[#1B211A] px-3 py-3">
          <div className="flex flex-col items-start justify-center gap-3">
            <div className="min-w-0 flex-1">
              <select
                id="organization-selector"
                value={selectedOrganization?.id ?? ""}
                onChange={(event) => selectOrganization(event.target.value)}
                disabled={isLoading || organizations.length === 0}
                className="w-full cursor-pointer truncate bg-transparent text-xs font-bold text-[#628141] outline-none disabled:cursor-not-allowed disabled:text-slate-200"
              >
                {isLoading ? (
                  <option value="">Loading organizations...</option>
                ) : organizations.length === 0 ? (
                  <option value="">No organizations</option>
                ) : (
                  organizations.map((organization) => (
                    <option
                      key={organization.id}
                      value={organization.id}
                      className="bg-[#1B211A] text-[#628141]"
                    >
                      {organization.name}
                    </option>
                  ))
                )}
              </select>
            </div>
          </div>

          {/* <p className="text-xs text-[#628141]">
            {selectedOrganization
              ? selectedOrganization.slug
              : "Create an organization to get started"}
          </p> */}
        </div>
      </div>

      {/* Navigation */}
      <nav aria-label="Main navigation" className="flex-1 space-y-1 p-4">
        <p className="mb-3 px-3 text-xs font-semibold uppercase tracking-wider text-[#1B211A]">
          Workspace
        </p>

        {navigation.map((item) => {
          const isActive =
            item.available &&
            (pathname === item.href ||
              (item.href !== "/dashboard" &&
                pathname.startsWith(`${item.href}/`)));

          const className = [
            "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm transition-colors",
            isActive
              ? "bg-[#1B211A] font-medium text-[#628141]"
              : item.available
                ? "text-[#1B211A] hover:bg-[#1B211A] hover:text-[#628141]"
                : "cursor-not-allowed text-[#1B211A]",
          ].join(" ");

          const content = (
            <>
              <NavigationIcon name={item.icon} />
              <span className="flex-1">{item.label}</span>

              {!item.available && (
                <span className="text-[10px] uppercase tracking-wide text-[#1B211A]">
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

      {/* Account */}
      <div className="p-4">
        <div className="mb-3 flex items-center gap-3 px-2">
          <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-[#1B211A] text-sm font-semibold text-[#EBD5AB]">
            {email.charAt(0).toUpperCase()}
          </div>

          <div className="min-w-0">
            <p className="truncate text-sm font-medium text-[#1B211A]">
              {email}
            </p>

            <p className="text-xs text-[#1B211A]">Account</p>
          </div>
        </div>

        <button
          type="button"
          onClick={onSignOut}
          className="w-full rounded-lg px-3 py-2 text-left text-sm text-[#1B211A] transition-colors hover:bg-[#1B211A] hover:text-white"
        >
          Sign out
        </button>
      </div>
    </aside>
  );
}
