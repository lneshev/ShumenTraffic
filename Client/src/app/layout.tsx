import Footer from "@/components/Footer";
import Header from "@/components/Header";
import { METADATA } from "@/constants/Metadata";
import { AuthProvider } from "@/context/AuthContext";
import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: METADATA.title,
  description: METADATA.description,
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en" data-scroll-behavior="smooth">
      <body className={`${geistSans.variable} ${geistMono.variable} antialiased flex flex-col h-screen overflow-hidden bg-white dark:bg-slate-950`}>
        <AuthProvider>
          <Header />
          <main className="flex-1 overflow-y-auto">
            {children}
          </main>
          <Footer />
        </AuthProvider>
      </body>
    </html>
  );
}