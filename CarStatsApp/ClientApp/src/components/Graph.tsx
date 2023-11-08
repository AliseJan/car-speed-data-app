import React from 'react';
import { Bar } from 'react-chartjs-2';
import 'chart.js/auto';

type GraphProps = {
    data: { hour: string; averageSpeed: number }[];
};

const Graph: React.FC<GraphProps> = ({ data }) => {
    const chartData = {
        labels: data.map((item) => item.hour),
        datasets: [
            {
                label: 'Average Speed',
                data: data.map((item) => item.averageSpeed),
                backgroundColor: 'rgba(54, 162, 235, 0.5)',
            },
        ],
    };

    return <Bar data={chartData} />;
};

export default Graph;