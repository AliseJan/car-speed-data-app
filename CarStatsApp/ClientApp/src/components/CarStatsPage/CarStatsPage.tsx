import React, { useState, useEffect } from 'react';
import axios from 'axios';
import FilterForm from '../FilterForm';
import Table from '../Table';
import Pagination from '../Pagination';
import './CarStatsPage.css';

type CarStat = {
    id: number;
    date: string;
    speed: number;
    registrationNumber: string;
};

type ApiData = {
    data: CarStat[];
    totalPages: number;
    currentPage: number;
    recordsPerPage: number;
    totalRecords: number;
};

const CarStatsTable: React.FC = () => {
    const [carStats, setCarStats] = useState<ApiData | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [currentPage, setCurrentPage] = useState<number>(1);
    const [speedFrom, setSpeedFrom] = useState('');
    const [speedTo, setSpeedTo] = useState('');
    const [dateFrom, setDateFrom] = useState('');
    const [dateTo, setDateTo] = useState('');
    const [filtersActive, setFiltersActive] = useState<boolean>(false);
    const backendURL = process.env.REACT_APP_BACKEND_URL;

    useEffect(() => {
        const fetchCarStats = async () => {
            setLoading(true);
            try {
                const queryParams = filtersActive ? {
                    pageNumber: currentPage,
                    speedFrom: speedFrom || undefined,
                    speedTo: speedTo || undefined,
                    dateFrom: dateFrom || undefined,
                    dateTo: dateTo || undefined
                } : {
                    pageNumber: currentPage
                };
                const response = await axios.get<ApiData>(`${backendURL}/Carstats`, { params: queryParams });
                setCarStats(response.data);
            } catch (error) {
                console.error('Error fetching car stats', error);
            } finally {
                setLoading(false);
            }
        };

        fetchCarStats();
    }, [currentPage, filtersActive]);

    const handleApplyFilters = async () => {
        setLoading(true);
        try {
            const response = await axios.get<ApiData>(`${backendURL}/Carstats`, {
                params: {
                    pageNumber: 1,
                    speedFrom: speedFrom || undefined,
                    speedTo: speedTo || undefined,
                    dateFrom: dateFrom || undefined,
                    dateTo: dateTo || undefined,
                },
            });
            setCarStats(response.data);
            setCurrentPage(1);
            setFiltersActive(true);
        } catch (error) {
            console.error('Error fetching car stats with filters', error);
        } finally {
            setLoading(false);
        }
    };

    const handleResetFilters = () => {
        setSpeedFrom('');
        setSpeedTo('');
        setDateFrom('');
        setDateTo('');
        setFiltersActive(false);
        handleApplyFilters();
    };

    const handleSpeedFromChange = (value: string) => setSpeedFrom(value);
    const handleSpeedToChange = (value: string) => setSpeedTo(value);
    const handleDateFromChange = (value: string) => setDateFrom(value);
    const handleDateToChange = (value: string) => setDateTo(value);

    const handlePageChange = (newPage: number) => {
        setCurrentPage(newPage);
    }

    const getPageNumbers = () => {
        const pageNumbers = [];
        const totalPageCount = carStats?.totalPages ?? 0;
        for (let i = 1; i <= Math.min(5, totalPageCount); i++) {
            pageNumbers.push(i);
        }
        return pageNumbers;
    };

    const formatDate = (dateString: string) => {
        const date = new Date(dateString);
        const year = date.getFullYear();
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        const day = date.getDate().toString().padStart(2, '0');
        const hours = date.getHours().toString().padStart(2, '0');
        const minutes = date.getMinutes().toString().padStart(2, '0');
        const seconds = date.getSeconds().toString().padStart(2, '0');
        return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
    };

    if (loading) {
        return <div>Loading...</div>;
    }

    if (!carStats) {
        return <div>No data to display</div>;
    }

    return (
        <div>
            <FilterForm
                speedFrom={speedFrom}
                speedTo={speedTo}
                dateFrom={dateFrom}
                dateTo={dateTo}
                onApplyFilters={handleApplyFilters}
                onResetFilters={handleResetFilters}
                onSpeedFromChange={handleSpeedFromChange}
                onSpeedToChange={handleSpeedToChange}
                onDateFromChange={handleDateFromChange}
                onDateToChange={handleDateToChange}
            />
            <Table
                stats={carStats.data}
                formatDate={formatDate}
            />
            <Pagination
                currentPage={currentPage}
                totalPages={carStats.totalPages}
                onPageChange={handlePageChange}
            />
        </div>
    );
};

export default CarStatsTable;