import LinesPage from "./page.client";

type LinesPageWrapperProps = {
  searchParams?: Promise<{
    lineNumber?: string;
  }>;
};

export async function generateMetadata({ searchParams }: LinesPageWrapperProps) {
  const params = searchParams ? await searchParams : undefined;
  const selectedLineNumber = params?.lineNumber;
  if (selectedLineNumber) {
    return {
      title: `Линия ${selectedLineNumber} - Шумен Трафик`
    };
  }
  else {
    return {
      title: `Линии - Шумен Трафик`
    };
  }
}

export default async function LinesPageWrapper({ searchParams }: { searchParams: Promise<{ [key: string]: string | string[] | undefined }> }) {
  return (
    <LinesPage searchParams={searchParams} />
  );
}