export default function Footer() {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="bg-gray-50 dark:bg-slate-900 border-t border-gray-200 dark:border-slate-700 mt-auto">
      <div className="mx-auto px-6 py-2">
        <div className="flex flex-col md:flex-row justify-between items-center gap-4">
          <p className="text-gray-600 dark:text-gray-400 text-sm">
            &copy; 2025-{currentYear} Shumen Traffic. All rights reserved.
          </p>
        </div>
      </div>
    </footer>
  );
}