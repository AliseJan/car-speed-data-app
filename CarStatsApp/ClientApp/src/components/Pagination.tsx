import React from 'react';

type PaginationProps = {
    currentPage: number;
    totalPages: number;
    onPageChange: (newPage: number) => void;
};

const Pagination: React.FC<PaginationProps> = ({ currentPage, totalPages, onPageChange }) => {
    const maxPageNumbersToShow = 5;
    let startPage: number, endPage: number;

    if (totalPages <= maxPageNumbersToShow) {
        startPage = 1;
        endPage = totalPages;
    } else {
        if (currentPage <= Math.ceil(maxPageNumbersToShow / 2)) {
            startPage = 1;
            endPage = maxPageNumbersToShow;
        } else if (currentPage + Math.floor(maxPageNumbersToShow / 2) >= totalPages) {
            startPage = totalPages - (maxPageNumbersToShow - 1);
            endPage = totalPages;
        } else {
            startPage = currentPage - Math.floor(maxPageNumbersToShow / 2);
            endPage = currentPage + Math.floor(maxPageNumbersToShow / 2);
        }
    }

    const pageNumbers: number[] = Array.from({ length: (endPage - startPage) + 1 }, (_, i) => startPage + i);

    return (
        <div className="pagination-container">
            <button
                onClick={() => onPageChange(1)}
                disabled={currentPage === 1}
                className="btn-pagination"
            >
                First
            </button>
            <button
                onClick={() => onPageChange(Math.max(1, currentPage - 1))}
                disabled={currentPage === 1}
                className="btn-pagination"
            >
                Previous
            </button>
            {pageNumbers.map((number) => (
                <button
                    key={number}
                    onClick={() => onPageChange(number)}
                    disabled={currentPage === number}
                    className={`btn-pagination ${currentPage === number ? 'btn-pagination-current' : ''}`}
                >
                    {number}
                </button>
            ))}
            <button
                onClick={() => onPageChange(Math.min(totalPages, currentPage + 1))}
                disabled={currentPage === totalPages}
                className="btn-pagination"
            >
                Next
            </button>
            <button
                onClick={() => onPageChange(totalPages)}
                disabled={currentPage === totalPages}
                className="btn-pagination"
            >
                Last
            </button>
        </div>
    );
};

export default Pagination;