import { Metadata } from "next";
import HomePage from "./page.client";

export const metadata: Metadata = {
  title: 'Начало - Шумен Трафик'
}

export default async function HomePageWrapper() {
  return (
    <HomePage />
  );
}