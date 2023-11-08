import React, { useState, useEffect } from 'react';
import DateSelector from '../DateSelector';
import Graph from '../Graph';
import axios from 'axios';
import './AverageSpeedPage.css';

type SpeedData = {
    hour: string;
    averageSpeed: number;
};

const AverageSpeedPage: React.FC = () => {
    const [selectedDate, setSelectedDate] = useState<string>('');
    const [graphData, setGraphData] = useState<SpeedData[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string>('');
    const backendURL = process.env.REACT_APP_BACKEND_URL;

    useEffect(() => {
        const fetchData = async () => {
            if (selectedDate) {
                setLoading(true);
                setError('');
                try {
                    const dateObject = new Date(selectedDate);
                    const formattedDate = dateObject.toISOString().split('T')[0];
                    const response = await axios.get(`${backendURL}/CarStats/average-speed/${formattedDate}`);
                    const transformedData: SpeedData[] = response.data.map((record: any) => ({
                        hour: record.hour,
                        averageSpeed: record.averageSpeed
                    }));
                    setGraphData(transformedData);
                } catch (err) {
                    console.error(err);
                    setError('Failed to fetch data');
                }
                setLoading(false);
            }
        };

        fetchData();
    }, [selectedDate]);

    const handleDateSelect = (date: string) => {
        setSelectedDate(date);
    };

    return (
        <div className="average-speed-page">
            <h1 className="text-center">Average Speed Analysis</h1>
            <DateSelector onDateSelect={handleDateSelect} />
            <div className="graph-container">
                {loading && <div className="loader">Loading...</div>}
                {error && <div className="error-message">Error: {error}</div>}
                {!loading && !error && graphData.length > 0 && (
                    <Graph data={graphData} />
                )}
            </div>
        </div>
    );
};

export default AverageSpeedPage;