'use client';

import Image from 'next/image';
import Link from 'next/link';
import { useState } from 'react';

export default function Header() {
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  };

  const navItems = [
    { label: 'Home', href: '/' },
    { label: 'Lines', href: '/lines' },
    { label: 'Schedule', href: '/schedule' },
    { label: 'Info', href: '/info' },
  ];

  return (
    <header className="sticky top-0 z-1001 bg-background border-b border-border shadow-sm">
      <div className="mx-auto px-6">
        <div className="flex justify-between items-center h-16">
          {/* Logo */}
          <Link href="/" className="flex items-center gap-2 hover:scale-105 transition-all duration-200 ease-in-out">
            <Image src="/logo.svg" alt="Shumen Traffic" width={32} height={32} />
            <span className="text-2xl font-bold text-primary font-noName37-regular">
              Shumen Traffic
            </span>
          </Link>

          {/* Desktop Navigation */}
          <nav className="hidden md:flex items-center gap-8">
            {navItems.map((item) => (
              <Link
                key={item.href}
                href={item.href}
                className="hover:text-primary"
              >
                {item.label}
              </Link>
            ))}
          </nav>

          {/* Admin Link */}
          <div className="hidden md:flex items-center gap-4">
            <Link
              href="/admin"
              className="px-4 py-2 hover:text-primary"
            >
              Admin
            </Link>
          </div>

          {/* Mobile Menu Button */}
          <button
            onClick={toggleMenu}
            className="md:hidden flex flex-col gap-1.5 p-2 hover:bg-background-light rounded-lg transition-colors"
            aria-label="Toggle menu"
          >
            <span className={`w-6 h-0.5 bg-foreground transition-all ${isMenuOpen ? 'rotate-45 translate-y-2' : ''}`} />
            <span className={`w-6 h-0.5 bg-foreground transition-all ${isMenuOpen ? 'opacity-0' : ''}`} />
            <span className={`w-6 h-0.5 bg-foreground transition-all ${isMenuOpen ? '-rotate-45 -translate-y-2' : ''}`} />
          </button>
        </div>

        {/* Mobile Navigation */}
        {isMenuOpen && (
          <nav className="md:hidden pb-4 border-t border-border">
            <div className="flex flex-col gap-2 pt-4">
              {navItems.map((item) => (
                <Link
                  key={item.href}
                  href={item.href}
                  className="px-4 py-2 text-foreground-light hover:bg-background-light rounded-lg font-medium transition-colors"
                  onClick={() => setIsMenuOpen(false)}
                >
                  {item.label}
                </Link>
              ))}
              <Link
                href="/admin"
                className="px-4 py-2 text-foreground-light hover:bg-background-light rounded-lg font-medium transition-colors"
                onClick={() => setIsMenuOpen(false)}
              >
                Admin
              </Link>
            </div>
          </nav>
        )}
      </div>
    </header>
  );
}