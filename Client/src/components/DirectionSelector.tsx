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
      <label className="label-standard">
        Direction
      </label>
      <div className="flex gap-1 h-10">
        {directions.map(dir => (
          <button
            key={dir}
            onClick={() => onDirectionChange(dir)}
            className={`border flex-1 px-3 rounded-lg font-semibold transition-colors text-sm ${selectedDirection === dir
              ? 'bg-selected-background text-selected-foreground font-bold border-selected-background'
              : 'bg-background-secondary hover:bg-background-light border-border'
              }`}
          >
            {dir}
          </button>
        ))}
      </div>
    </div>
  );
}