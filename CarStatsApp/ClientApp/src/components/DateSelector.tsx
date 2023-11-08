import React, { useState } from 'react';

type DateSelectorProps = {
    onDateSelect: (date: string) => void;
};

const DateSelector: React.FC<DateSelectorProps> = ({ onDateSelect }) => {
    const [selectedDate, setSelectedDate] = useState<string>('');

    const handleDateChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setSelectedDate(event.target.value);
        onDateSelect(event.target.value);
    };

    return (
        <div className="date-selector">
            <label htmlFor="date-input" className="date-label">Select Date:</label>
            <input
                id="date-input"
                type="date"
                value={selectedDate}
                onChange={handleDateChange}
                className="date-input"
            />
        </div>
    );
};

export default DateSelector;