import React from 'react';

type CarStat = {
    id: number;
    date: string;
    speed: number;
    registrationNumber: string;
};

type StatsTableProps = {
    stats: CarStat[];
    formatDate: (dateString: string) => string;
};

const Table: React.FC<StatsTableProps> = ({ stats, formatDate }) => {
    if (stats.length === 0) {
        return <div className="no-data-message">No data available or uploaded.</div>;
    }

    return (
        <table className="centered-table">
            <thead>
                <tr className="table-header">
                    <th className="table-cell">ID</th>
                    <th className="table-cell">Date</th>
                    <th className="table-cell">Speed</th>
                    <th className="table-cell">Registration Number</th>
                </tr>
            </thead>
            <tbody>
                {stats.map((stat) => (
                    <tr key={stat.id} className="table-row">
                        <td className="table-cell">{stat.id}</td>
                        <td className="table-cell">{formatDate(stat.date)}</td>
                        <td className="table-cell">{stat.speed}</td>
                        <td className="table-cell">{stat.registrationNumber}</td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
};

export default Table;