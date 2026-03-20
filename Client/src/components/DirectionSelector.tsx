interface DirectionSelectorProps {
  selectedDirection: number;
  onDirectionChange: (direction: number) => void;
  directions?: number[];
}

export default function DirectionSelector({
  selectedDirection,
  onDirectionChange,
  directions = [1, 2]
}: DirectionSelectorProps) {
  return (
    <div className="w-25">
      <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
        Direction
      </label>
      <div className="flex gap-1 h-10">
        {directions.map(dir => (
          <button
            key={dir}
            onClick={() => onDirectionChange(dir)}
            className={`border border-gray-300 dark:border-slate-600 flex-1 px-3 rounded-lg font-semibold transition-colors text-sm ${selectedDirection === dir
              ? 'bg-blue-600 text-white'
              : 'bg-gray-200 dark:bg-slate-800 text-gray-900 dark:text-white hover:bg-gray-300 dark:hover:bg-slate-700'
              }`}
          >
            {dir}
          </button>
        ))}
      </div>
    </div>
  );
}