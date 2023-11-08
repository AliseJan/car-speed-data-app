import React from 'react';

type FilterFormProps = {
    speedFrom: string;
    speedTo: string;
    dateFrom: string;
    dateTo: string;
    onApplyFilters: () => Promise<void>;
    onResetFilters: () => void;
    onSpeedFromChange: (value: string) => void;
    onSpeedToChange: (value: string) => void;
    onDateFromChange: (value: string) => void;
    onDateToChange: (value: string) => void;
};

const FilterForm: React.FC<FilterFormProps> = ({
    speedFrom,
    speedTo,
    dateFrom,
    dateTo,
    onApplyFilters,
    onResetFilters,
    onSpeedFromChange,
    onSpeedToChange,
    onDateFromChange,
    onDateToChange
}) => {
    const handleSpeedFromChange = (e: React.ChangeEvent<HTMLInputElement>) => onSpeedFromChange(e.target.value);
    const handleSpeedToChange = (e: React.ChangeEvent<HTMLInputElement>) => onSpeedToChange(e.target.value);
    const handleDateFromChange = (e: React.ChangeEvent<HTMLInputElement>) => onDateFromChange(e.target.value);
    const handleDateToChange = (e: React.ChangeEvent<HTMLInputElement>) => onDateToChange(e.target.value);

    return (
        <div className="filter-container">
            <input
                type="number"
                value={speedFrom}
                onChange={handleSpeedFromChange}
                placeholder="Speed From"
                className="filter-input"
            />
            <input
                type="number"
                value={speedTo}
                onChange={handleSpeedToChange}
                placeholder="Speed To"
                className="filter-input"
            />
            <input
                type="date"
                value={dateFrom}
                onChange={handleDateFromChange}
                className="filter-input"
            />
            <input
                type="date"
                value={dateTo}
                onChange={handleDateToChange}
                className="filter-input"
            />
            <button
                onClick={onApplyFilters}
                className="filter-button filter-button-apply"
            >
                Apply Filters
            </button>
            <button
                onClick={onResetFilters}
                className="filter-button filter-button-reset"
            >
                Reset Filters
            </button>
        </div>
    );
};

export default FilterForm;