import BusLinesLightService from '@/services/BusLinesLightService';
import SchedulePage from './page.client';

type SchedulePageProps = {
  searchParams?: Promise<{
    lineNumber?: string;
    direction?: string;
  }>;
};

function parsePositiveNumber(value?: string, fallback = 1) {
  const parsed = Number(value);
  return Number.isFinite(parsed) && parsed > 0 ? parsed : fallback;
}

export default async function SchedulePageWrapper({ searchParams }: SchedulePageProps) {
  const params = searchParams ? await searchParams : undefined;
  const selectedLineNumber = params?.lineNumber;
  const selectedDirection = parsePositiveNumber(params?.direction, 1);

  let selectedLineId: number = 0;

  if (selectedLineNumber && selectedDirection) {
    try {
      const data = await BusLinesLightService.read({ lineNumberEquals: selectedLineNumber });
      const line = data.items[0];
      if (line) {
        selectedLineId = line.id;
      }
    } catch (error) {
      console.error('Failed to fetch bus line:', error);
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