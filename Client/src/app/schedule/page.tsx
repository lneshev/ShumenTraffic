import BusLinesLightService from '@/services/BusLinesLightService';
import { notFound } from 'next/navigation';
import SchedulePage from './page.client';

type SchedulePageProps = {
  searchParams?: Promise<{
    lineNumber?: string;
    direction?: string;
  }>;
};

const parsePositiveNumber = (value?: string, fallback = 1) => {
  const parsed = Number(value);
  return Number.isFinite(parsed) && parsed > 0 ? parsed : fallback;
}

const getBusLineId = async (lineNumber: string) => {
  try {
    const data = await BusLinesLightService.read({ lineNumberEquals: lineNumber });
    const line = data.items[0];
    if (line) {
      return line.id;
    }
    return 0;
  } catch (error) {
    console.error('Failed to fetch bus line:', error);
    return 0;
  }
}

export async function generateMetadata({ searchParams }: SchedulePageProps) {
  const params = searchParams ? await searchParams : undefined;
  const selectedLineNumber = params?.lineNumber;
  if (selectedLineNumber) {
    return {
      title: `Разписание за линия ${selectedLineNumber} - Шумен Трафик`
    };
  }
  else {
    return {
      title: `Разписания - Шумен Трафик`
    };
  }
}

export default async function SchedulePageWrapper({ searchParams }: SchedulePageProps) {
  const params = searchParams ? await searchParams : undefined;
  const selectedLineNumber = params?.lineNumber;
  const selectedDirection = parsePositiveNumber(params?.direction, 1);

  let selectedLineId: number = 0;

  if (selectedLineNumber && selectedDirection) {
    const lineId = await getBusLineId(selectedLineNumber);
    if (lineId) {
      selectedLineId = lineId;
    }
    else {
      notFound();
    }
  }

  return (
    <SchedulePage
      selectedLineId={selectedLineId}
      selectedLineNumber={selectedLineNumber || ''}
      selectedDirection={selectedDirection}
    />
  );
}