export default function Footer() {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="bg-background-secondary border-t border-border mt-auto">
      <div className="mx-auto px-6 py-2">
        <div className="flex flex-col md:flex-row justify-between items-center gap-4">
          <p className="text-text-muted text-sm">
            &copy; 2025-{currentYear} Shumen Traffic. All rights reserved.
          </p>
        </div>
      </div>
    </footer>
  );
}