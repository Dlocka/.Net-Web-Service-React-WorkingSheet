import React from 'react';

function MyButton() {
    const handleButtonClick = async () => {
        try {
            const response = await fetch('http://localhost:5058/Test/PutTest', {
                method: 'PUT'
            });
            const data = await response.json();
            console.log('Success:', data);
            // You can add further logic here, e.g., display a success message to the user
        } catch (error) {
            console.error('Error:', error);
            // You can add further error handling here, e.g., display an error message
        }
    };

    return (
        <button
            onClick={handleButtonClick} // Add the onClick event handler
            className="px-6 py-3 bg-blue-600 text-white font-semibold rounded-lg shadow-md hover:bg-blue-700 transition-colors duration-300 ease-in-out transform hover:scale-105 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-75"
        >
            Send PUT Request
        </button>
    );
}

export default function MyApp() {
    return (
        <div className="bg-white p-8 rounded-xl shadow-lg max-w-md w-full text-center">
            <h1 className="text-4xl font-extrabold text-gray-800 mb-6">
                Welcome to my App!
            </h1>
            <MyButton /> {/* MyButton is now part of MyApp */}
        </div>
    );
}