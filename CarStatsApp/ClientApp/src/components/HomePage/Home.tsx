import React, { useRef, useState } from 'react';
import axios from 'axios';
import './Home.css';
import { ToastContainer, toast } from 'react-toastify';

const Home: React.FC = () => {
    const [isUploading, setUploading] = useState<boolean>(false);
    const [isClearing, setClearing] = useState<boolean>(false);
    const fileInputRef = useRef<HTMLInputElement>(null);
    const backendURL = process.env.REACT_APP_BACKEND_URL;

    const onFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event.target.files && event.target.files.length > 0) {
            uploadFile(event.target.files[0]);
        }
    };

    const uploadFile = async (file: File) => {
        const formData = new FormData();
        formData.append('file', file);
        setUploading(true);

        try {
            const response = await axios.post(`${backendURL}/CarStats/upload`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });
            toast.success('File uploaded successfully');
            console.log(response.data);
        } catch (error) {
            toast.error('Error uploading file');
            console.error("Error uploading file:", error);
        } finally {
            setUploading(false);
        }
    };

    const onUploadButtonClick = () => {
        fileInputRef.current?.click();
    };

    const onClearData = async () => {
        if (!window.confirm('Are you sure you want to clear all data?')) return;
        setClearing(true);

        try {
            const response = await axios.post(`${backendURL}/CarStats/clear`);
            toast.success('All data cleared successfully');
        } catch (error) {
            toast.error('Error clearing data');
        } finally {
            setClearing(false);
        }
    };

    return (
        <div className="home-container">
            <h1>Welcome to CarStatsApp</h1>
            <div className="button-container">
                <input
                    type="file"
                    ref={fileInputRef}
                    onChange={onFileChange}
                    style={{ display: 'none' }}
                />
                <button
                    className="upload-button"
                    onClick={onUploadButtonClick}
                    disabled={isUploading || isClearing}
                >
                    {isUploading ? 'Uploading...' : 'Upload File'}
                </button>
                <button
                    className="clear-button"
                    onClick={onClearData}
                    disabled={isUploading || isClearing}
                >
                    {isClearing ? 'Clearing Data...' : 'Clear Data'}
                </button>
            </div>
            <ToastContainer position="top-center" />
        </div>
    );
};

export default Home;